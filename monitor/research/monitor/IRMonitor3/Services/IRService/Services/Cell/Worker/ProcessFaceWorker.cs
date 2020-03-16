using Common;
using Devices;
using IRService.Common;
using Miscs;
using OpenCvSharp;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace IRService.Services.Cell.Worker
{
    /// <summary>
    /// 处理可见光人脸数据工作线程 
    /// </summary>
    public class ProcessFaceWorker : BaseWorker
    {
        /// <summary>
        /// 设备单元服务
        /// </summary>
        private CellService cell;

        /// <summary>
        /// 设备
        /// </summary>
        private IDevice device;

        /// <summary>
        /// 可见光图像
        /// </summary>
        private PinnedBuffer<byte> image;

        /// <summary>
        /// ROI区域
        /// </summary>
        private Rectangle roi;

        /// <summary>
        /// 接收可见光图像事件处理函数
        /// </summary>
        private EventEmitter.EventHandler onReceiveImage;

        /// <summary>
        /// 接收ROI事件处理函数
        /// </summary>
        private EventEmitter.EventHandler onChangedROI;

        /// <summary>
        /// 人脸识别频率
        /// </summary>
        private int frameRate;

        public override ARESULT Initialize(Dictionary<string, object> arguments)
        {
            cell = arguments["cell"] as CellService;
            device = arguments["device"] as IDevice;
            frameRate = (int)arguments["frameRate"];

            // 启动人脸检测引擎参数
            if (!FaceDetector.StartEngine()) {
                return ARESULT.E_FAIL;
            }

            // 读取配置信息
            if (device.Read(ReadMode.CameraParameters, null, out object outData, out _)) {
                // 创建资源
                Configuration.CameraParameters cameraParameters = outData as Configuration.CameraParameters;
                image = PinnedBuffer<byte>.Alloc(cameraParameters.width * cameraParameters.height * 3 / 2, cameraParameters.width, cameraParameters.height);
            }

            // 读取配置信息
            if (device.Read(ReadMode.FaceThermometryRegion, null, out outData, out _)) {
                // 创建资源
                Dictionary<string, object> faceThermometryRegion = outData as Dictionary<string, object>;
                roi = (Rectangle)faceThermometryRegion["rectangle"];
            }

            // 声明事件处理函数
            onReceiveImage = (args) => {
                if ((args[0] == cell) && (args[1] == device)) {
                    image = Arrays.Clone(args[2] as PinnedBuffer<byte>, image, sizeof(byte));
                }
            };

            // 声明事件处理函数
            onChangedROI = (args) => {
                if ((args[0] == cell) && (args[1] == device)) {
                    lock (this) {
                        roi = (Rectangle)args[2];
                    }
                }
            };

            return base.Initialize(arguments);
        }

        public override ARESULT Start()
        {
            EventEmitter.Instance.Subscribe(Constants.EVENT_SERVICE_RECEIVE_IMAGE, onReceiveImage);
            EventEmitter.Instance.Subscribe(Constants.EVENT_SERVICE_ROI_CHANGED, onChangedROI);

            return base.Start();
        }

        public override void Discard()
        {
            EventEmitter.Instance.Unsubscribe(Constants.EVENT_SERVICE_RECEIVE_IMAGE, onReceiveImage);
            EventEmitter.Instance.Unsubscribe(Constants.EVENT_SERVICE_ROI_CHANGED, onChangedROI);

            base.Discard();
        }

        protected unsafe override void Run()
        {
            var duration = 1000 / frameRate;
            var yuv = new Mat(this.image.height + image.height / 2, image.width, MatType.CV_8UC1);
            var bgra = new Mat(this.image.height, image.width, MatType.CV_8UC4);
            var rectangle = new Rectangle();

            while (!IsTerminated()) {
                Buffer.MemoryCopy(image.ptr.ToPointer(), yuv.Data.ToPointer(), image.Length, image.Length);
                Cv2.CvtColor(yuv, bgra, ColorConversionCodes.YUV2BGRA_YV12);

                lock (this) {
                    rectangle = roi;
                }

                var rect = new Rectangle(rectangle.X * image.width / 1000, rectangle.Y * image.height / 1000, rectangle.Width * image.width / 1000, rectangle.Height * image.height / 1000);
                // 1、在yml中配置参数 可见光ROI 是否开启人脸比对 人脸比对阈值
                var resultType = FaceDetector.DetectFace(image.ptr, image.width, image.height, image.Length, rect);
                resultType?.addRectangles.ForEach(r => AddPeople(null, new Mat(bgra, new Rect(r.X, r.Y, r.Width, r.Height))));

                Thread.Sleep(duration);
            }
        }

        /// <summary>
        /// 添加人员记录
        /// </summary>
        /// <param name="selection">选区</param>
        /// <param name="mat">告警</param>
        private void AddPeople(Models.Selections.Selection selection, Mat mat)
        {
            var data = new People() {
                cellName = cell.cell.name,
                selectionName = selection?.Entity.name ?? null,
                startTime = DateTime.Now,
                url = Repository.Repository.SaveImage(mat)
            };

            Repository.Repository.AddPeople(data);
        }
    }
}
