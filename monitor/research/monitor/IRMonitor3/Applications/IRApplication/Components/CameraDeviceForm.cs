using Common;
using Devices;
using IRService.Common;
using IRService.Services.Cell;
using Miscs;
using System;

namespace IRApplication.Components
{
    public partial class CameraDeviceForm : RenderableForm
    {
        /// <summary>
        /// 可见光图像
        /// </summary>
        private PinnedBuffer<byte> image;

        /// <summary>
        /// 接收可见光图像事件处理函数
        /// </summary>
        private readonly EventEmitter.EventHandler onReceiveImage;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cell">设备单元服务</param>
        /// <param name="device">设备</param>
        public CameraDeviceForm(CellService cell, IDevice device)
        {
            InitializeComponent();

            // 初始化窗体
            if (device.Read(ReadMode.CameraParameters, null, out object outData, out _)) {
                var cameraParameters = outData as Repository.Entities.Configuration.CameraParameters;
                InitializeComponent(cameraParameters.width, cameraParameters.stride, cameraParameters.height);
            }
            else {
                InitializeComponent(Width, Width, Height);
            }

            // 声明事件处理函数
            onReceiveImage = (args) => {
                if ((args[0] == cell) && (args[1] == device)) {
                    image = Arrays.Clone(args[2] as PinnedBuffer<byte>, image);
                    if (IsHandleCreated) {
                        BeginInvoke((Action)(() => {
                            DrawYV12Image(image.ptr, image.Length);
                            Render();
                        }));
                    }
                }
            };
        }

        private void CameraDeviceForm_Load(object sender, System.EventArgs e)
        {
            EventEmitter.Instance.Subscribe(Constants.EVENT_RECEIVE_IMAGE, onReceiveImage);
        }

        private void CameraDeviceForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            EventEmitter.Instance.Unsubscribe(Constants.EVENT_RECEIVE_IMAGE, onReceiveImage);
        }
    }
}
