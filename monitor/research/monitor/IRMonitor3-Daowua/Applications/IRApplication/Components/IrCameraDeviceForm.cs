using Common;
using Devices;
using IRApplication.Components.HIKDevice;
using IRService.Common;
using IRService.Services.Cell;
using Miscs;
using System;
using System.Windows.Forms;

namespace IRApplication.Components
{
    public partial class IrCameraDeviceForm : Form
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
        /// 可渲染控件
        /// </summary>
        protected IRenderableControl renderableControl;
        protected virtual IRenderableControl RenderableControl {
            get {
                if (renderableControl == null) {
                    renderableControl = HIKVisionRenderableControl.UseHIKDevice ? new HIKVisionRenderableControl(2) as IRenderableControl : new OpenGLRenderableControl() as IRenderableControl;
                }

                return renderableControl;
            }
        }

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
                RenderableControl.InitializeComponent(this, irCameraParameters.width, irCameraParameters.stride, irCameraParameters.height);
            }
            else {
                RenderableControl.InitializeComponent(this, Width, Width, Height);
            }

            onReceiveIrImage = (args) => {
                if ((args[0] == cell) && (args[1] == device)) {
                    irImage = Arrays.Clone(args[2] as PinnedBuffer<byte>, irImage, sizeof(byte));
                    if (IsHandleCreated) {
                        BeginInvoke((Action)(() => {
                            RenderableControl.DrawYV12Image(irImage.ptr, irImage.Length);
                            RenderableControl.Render(this);
                        }));
                    }
                }
            };
        }

        protected override CreateParams CreateParams {
            get { return RenderableControl.GetParams(base.CreateParams); }
        }

        private void IrCameraDeviceForm_Load(object sender, System.EventArgs e)
        {
            EventEmitter.Instance.Subscribe(Constants.EVENT_SERVICE_RECEIVE_IRIMAGE, onReceiveIrImage);
        }

        private void IrCameraDeviceForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            EventEmitter.Instance.Unsubscribe(Constants.EVENT_SERVICE_RECEIVE_IRIMAGE, onReceiveIrImage);
            RenderableControl?.Dispose();
        }

        private void IrCameraDeviceForm_SizeChanged(object sender, EventArgs e)
        {
            renderableControl.OnSizeChanged(this);
        }
    }
}
