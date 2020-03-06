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
    public partial class CameraDeviceForm : Form
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
        /// 可渲染控件
        /// </summary>
        protected IRenderableControl renderableControl;
        protected virtual IRenderableControl RenderableControl {
            get {
                if (renderableControl == null) {
                    renderableControl = HIKVisionRenderableControl.UseHIKDevice ? new HIKVisionRenderableControl(1) as IRenderableControl : new OpenGLRenderableControl() as IRenderableControl;
                }

                return renderableControl;
            }
        }

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
                RenderableControl.InitializeComponent(this, cameraParameters.width, cameraParameters.stride, cameraParameters.height);
            }
            else {
                RenderableControl.InitializeComponent(this, Width, Width, Height);
            }

            // 声明事件处理函数
            onReceiveImage = (args) => {
                if ((args[0] == cell) && (args[1] == device)) {
                    image = Arrays.Clone(args[2] as PinnedBuffer<byte>, image, sizeof(byte));
                    if (IsHandleCreated) {
                        BeginInvoke((Action)(() => {
                            RenderableControl.DrawYV12Image(image.ptr, image.Length);
                            RenderableControl.Render(this);
                        }));
                    }
                }
            };
        }

        protected override CreateParams CreateParams {
            get { return RenderableControl.GetParams(base.CreateParams); }
        }

        private void CameraDeviceForm_Load(object sender, System.EventArgs e)
        {
            EventEmitter.Instance.Subscribe(Constants.EVENT_SERVICE_RECEIVE_IMAGE, onReceiveImage);
        }

        private void CameraDeviceForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            EventEmitter.Instance.Unsubscribe(Constants.EVENT_SERVICE_RECEIVE_IMAGE, onReceiveImage);
            renderableControl?.Dispose();
        }

        private void CameraDeviceForm_SizeChanged(object sender, EventArgs e)
        {
            renderableControl.OnSizeChanged(this);
        }
    }
}
