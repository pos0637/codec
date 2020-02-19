using Common;
using Devices;
using IRService.Common;
using IRService.Services.Cell;
using Miscs;
using System;

namespace IRApplication.Components
{
    public partial class IrCameraDeviceForm : RenderableForm
    {
        /// <summary>
        /// 温度矩阵
        /// </summary>
        private float[] temperature;

        /// <summary>
        /// 温度图像
        /// </summary>
        private byte[] temperatureImage;

        /// <summary>
        /// 红外图像
        /// </summary>
        private PinnedBuffer<byte> irImage;

        /// <summary>
        /// 接收温度事件处理函数
        /// </summary>
        private readonly EventEmitter.EventHandler onReceiveTemperature;

        /// <summary>
        /// 接收红外图像事件处理函数
        /// </summary>
        private readonly EventEmitter.EventHandler onReceiveIrImage;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cell">设备单元服务</param>
        /// <param name="device">设备</param>
        public IrCameraDeviceForm(CellService cell, IDevice device)
        {
            InitializeComponent();

            // 初始化窗体
            if (device.Read(ReadMode.IrCameraParameters, null, out object outData, out _)) {
                var irCameraParameters = outData as Repository.Entities.Configuration.IrCameraParameters;
                InitializeComponent(irCameraParameters.width, irCameraParameters.stride, irCameraParameters.height);
            }
            else {
                InitializeComponent(Width, Width, Height);
            }

            // 声明事件处理函数
            onReceiveTemperature = (args) => {
                if ((args[0] == cell) && (args[1] == device)) {
                    temperature = Arrays.Clone(args[2] as PinnedBuffer<float>, temperature);
                    if (IsHandleCreated) {
                        BeginInvoke((Action)(() => {
                            temperatureImage = ImageUtils.GetIrImage(temperature, temperatureImage);
                            DrawYImage(temperatureImage, temperatureImage.Length);
                            Render();
                        }));
                    }
                }
            };

            onReceiveIrImage = (args) => {
                if ((args[0] == cell) && (args[1] == device)) {
                    irImage = Arrays.Clone(args[2] as PinnedBuffer<byte>, irImage);
                    if (IsHandleCreated) {
                        BeginInvoke((Action)(() => {
                            DrawYV12Image(irImage.ptr, irImage.Length);
                            Render();
                        }));
                    }
                }
            };
        }

        private void IrCameraDeviceForm_Load(object sender, System.EventArgs e)
        {
            EventEmitter.Instance.Subscribe(Constants.EVENT_RECEIVE_TEMPERATURE, onReceiveTemperature);
            EventEmitter.Instance.Subscribe(Constants.EVENT_RECEIVE_IRIMAGE, onReceiveIrImage);
        }

        private void IrCameraDeviceForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            EventEmitter.Instance.Unsubscribe(Constants.EVENT_RECEIVE_TEMPERATURE, onReceiveTemperature);
            EventEmitter.Instance.Unsubscribe(Constants.EVENT_RECEIVE_IRIMAGE, onReceiveIrImage);
        }
    }
}
