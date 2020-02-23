using OpenCvSharp;
using OpenGL;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IRApplication.Components
{
    /// <summary>
    /// 可渲染窗体
    /// </summary>
    public class RenderableForm : Form
    {
        /// <summary>
		/// Vertex position array.
		/// </summary>
		private static readonly float[] ArrayPosition = new float[] {
            0.0f, 1.0f,
            1.0f, 1.0f,
            0.0f, 0.0f,
            1.0f, 0.0f
        };

        /// <summary>
        /// Vertex color array.
        /// </summary>
        private static readonly float[] ArrayTexCoord = new float[] {
            0.0f, 0.0f,
            1.0f, 0.0f,
            0.0f, 1.0f,
            1.0f, 1.0f
        };

        /// <summary>
        /// 视图宽度
        /// </summary>
        protected int viewWidth;

        /// <summary>
        /// 视图对齐宽度
        /// </summary>
        protected int viewStride;

        /// <summary>
        /// 视图高度
        /// </summary>
        protected int viewHeight;

        /// <summary>
        /// 显示区域
        /// </summary>
        protected Rectangle area = new Rectangle();

        /// <summary>
        /// YV12位图
        /// </summary>
        protected Mat yuv;

        /// <summary>
        /// BGRA位图
        /// </summary>
        protected Mat bgra;

        /// <summary>
        /// OpenGL控件
        /// </summary>
        private GlControl glControl;

        /// <summary>
        /// 设备上下文
        /// </summary>
        private DeviceContext deviceContext;

        /// <summary>
        /// 纹理
        /// </summary>
        private uint texture;

        /// <summary>
        /// 初始化控件
        /// </summary>
        /// <param name="displayId">显示索引</param>
        /// <param name="width">视图宽度</param>
        /// <param name="stride">视图对齐宽度</param>
        /// <param name="height">视图高度</param>
        public void InitializeComponent(int width, int stride, int height)
        {
            viewWidth = width;
            viewStride = stride;
            viewHeight = height;

            try {
                yuv = new Mat(viewHeight + viewHeight / 2, viewWidth, MatType.CV_8UC1);
                bgra = new Mat(viewHeight, viewWidth, MatType.CV_8UC4);

                glControl = new GlControl {
                    Animation = true,
                    ColorBits = ((uint)(24u)),
                    DepthBits = ((uint)(0u)),
                    Dock = DockStyle.Fill,
                    Location = new System.Drawing.Point(0, 0),
                    MultisampleBits = ((uint)(0u)),
                    Name = "glControl",
                    StencilBits = ((uint)(0u)),
                    TabIndex = 0
                };
                glControl.ContextCreated += new EventHandler<GlControlEventArgs>(glControl_ContextCreated);
                glControl.Render += new EventHandler<GlControlEventArgs>(glControl_Render);
                Controls.Add(glControl);
                deviceContext = DeviceContext.Create();
            }
            catch {
            }
        }

        /// <summary>
        /// 绘制图像
        /// </summary>
        /// <param name="image">YV12图像</param>
        /// <param name="length">长度</param>
        protected unsafe void DrawYV12Image(IntPtr image, int length)
        {
            System.Buffer.MemoryCopy(image.ToPointer(), yuv.Data.ToPointer(), length, length);
            Cv2.CvtColor(yuv, bgra, ColorConversionCodes.YUV2BGRA_YV12);
        }

        /// <summary>
        /// 绘制图像
        /// </summary>
        /// <param name="image">Y图像</param>
        /// <param name="length">长度</param>
        protected unsafe void DrawYImage(IntPtr image, int length)
        {
            System.Buffer.MemoryCopy(image.ToPointer(), yuv.Data.ToPointer(), length, length);
            Cv2.CvtColor(yuv, bgra, ColorConversionCodes.YUV2BGRA_YV12);
        }

        /// <summary>
        /// 绘制图像
        /// </summary>
        /// <param name="image">Y图像</param>
        /// <param name="length">长度</param>
        protected unsafe void DrawYImage(byte[] image, int length)
        {
            Marshal.Copy(image, 0, yuv.Data, length);
            Cv2.CvtColor(yuv, bgra, ColorConversionCodes.YUV2BGRA_YV12);
        }

        /// <summary>
        /// 绘制图像
        /// </summary>
        /// <param name="mat">RGBA图像</param>
        /// <param name="length">长度</param>
        protected unsafe void DrawRGBAImage(IntPtr image, int length)
        {
            System.Buffer.MemoryCopy(image.ToPointer(), bgra.Data.ToPointer(), length, length);
        }

        /// <summary>
        /// 渲染
        /// </summary>
        protected unsafe void Render()
        {
            try {
                Gl.BindTexture(TextureTarget.Texture2d, texture);
                Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, viewWidth, viewHeight, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bgra.Data);
            }
            catch {
            }
        }

        /// <summary>
        /// 大小改变事件处理函数
        /// </summary>
        protected virtual void OnSizeChanged()
        {
            area = new Rectangle(PointToScreen(ClientRectangle.Location), ClientSize);
        }

        private void glControl_ContextCreated(object sender, GlControlEventArgs e)
        {
            try {
                Gl.MatrixMode(MatrixMode.Projection);
                Gl.LoadIdentity();
                Gl.Ortho(0.0, 1.0f, 0.0, 1.0, 0.0, 1.0);

                Gl.MatrixMode(MatrixMode.Modelview);
                Gl.LoadIdentity();

                Gl.Enable(EnableCap.Texture2d);

                Gl.VertexPointer(2, VertexPointerType.Float, 0, ArrayPosition);
                Gl.EnableClientState(EnableCap.VertexArray);
                Gl.TexCoordPointer(2, TexCoordPointerType.Float, 0, ArrayTexCoord);
                Gl.EnableClientState(EnableCap.TextureCoordArray);

                texture = Gl.GenTexture();
                Gl.BindTexture(TextureTarget.Texture2d, texture);
                Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, Gl.LINEAR);
                Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, Gl.LINEAR);
                Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, Gl.CLAMP_TO_EDGE);
                Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, Gl.CLAMP_TO_EDGE);
            }
            catch {
            }
        }

        private void glControl_Render(object sender, GlControlEventArgs e)
        {
            try {
                Gl.VertexPointer(2, VertexPointerType.Float, 0, ArrayPosition);
                Gl.EnableClientState(EnableCap.VertexArray);
                Gl.TexCoordPointer(2, TexCoordPointerType.Float, 0, ArrayTexCoord);
                Gl.EnableClientState(EnableCap.TextureCoordArray);

                var control = sender as Control;
                Gl.Viewport(0, 0, control.ClientSize.Width, control.ClientSize.Height);
                Gl.Clear(ClearBufferMask.ColorBufferBit);

                if (Gl.CurrentVersion >= Gl.Version_110) {
                    Gl.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
                }
                else {
                    Gl.Begin(PrimitiveType.TriangleStrip);
                    for (var i = 0; i < ArrayPosition.Length; i += 2) {
                        Gl.TexCoord2(ArrayTexCoord[i], ArrayTexCoord[i + 1]);
                        Gl.Vertex2(ArrayPosition[i], ArrayPosition[i + 1]);
                    }
                    Gl.End();
                }
            }
            catch {
            }
        }
    }
}
