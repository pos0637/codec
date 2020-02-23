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
        /// 红外图像
        /// </summary>
        private PinnedBuffer<byte> irImage;

        /// <summary>
        /// 接收红外图像事件处理函数
        /// </summary>
        private readonly EventEmitter.EventHandler onReceiveIrImage;

        /// <summary>
        /// 海康播放组件
        /// </summary>
        private readonly HIKPlayComponent hikPlayComponent = new HIKPlayComponent();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cell">设备单元服务</param>
        /// <param name="device">设备</param>
        public IrCameraDeviceForm(CellService cell, IDevice device)
        {
            InitializeComponent();

            if (HIKPlayComponent.UseHIKDevice) {
                hikPlayComponent.Initialize();
                hikPlayComponent.StartRealPlay(2, this.Handle);
                return;
            }

            // 初始化窗体
            if (device.Read(ReadMode.IrCameraParameters, null, out object outData, out _)) {
                var irCameraParameters = outData as Repository.Entities.Configuration.IrCameraParameters;
                InitializeComponent(irCameraParameters.width, irCameraParameters.stride, irCameraParameters.height);
            }
            else {
                InitializeComponent(Width, Width, Height);
            }

            onReceiveIrImage = (args) => {
                if ((args[0] == cell) && (args[1] == device)) {
                    irImage = Arrays.Clone(args[2] as PinnedBuffer<byte>, irImage, sizeof(byte));
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
            EventEmitter.Instance.Subscribe(Constants.EVENT_SERVICE_RECEIVE_IRIMAGE, onReceiveIrImage);
        }

        private void IrCameraDeviceForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            EventEmitter.Instance.Unsubscribe(Constants.EVENT_SERVICE_RECEIVE_IRIMAGE, onReceiveIrImage);
            if (HIKPlayComponent.UseHIKDevice) {
                hikPlayComponent.StopRealPlay();
            }
        }
    }
}
