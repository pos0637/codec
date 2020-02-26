using OpenCvSharp;
using OpenGL;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IRApplication.Components
{
    /// <summary>
    /// 可渲染控件
    /// </summary>
    public class OpenGLRenderableControl : IRenderableControl
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
        /// 纹理
        /// </summary>
        private uint texture;

        public bool InitializeComponent(Control parent, int width, int stride, int height)
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
                    TabIndex = 0,
                    ContextSharingGroup = ""
                };
                glControl.ContextCreated += new EventHandler<GlControlEventArgs>(glControl_ContextCreated);
                glControl.Render += new EventHandler<GlControlEventArgs>(glControl_Render);
                parent.Controls.Add(glControl);

                return true;
            }
            catch {
                return false;
            }
        }

        public CreateParams GetParams(CreateParams createParams)
        {
            return createParams;
        }

        public void Dispose()
        {
            glControl.Dispose();
        }

        public unsafe void DrawYV12Image(IntPtr image, int length)
        {
            System.Buffer.MemoryCopy(image.ToPointer(), yuv.Data.ToPointer(), length, length);
            Cv2.CvtColor(yuv, bgra, ColorConversionCodes.YUV2BGRA_YV12);
        }

        public unsafe void DrawYImage(IntPtr image, int length)
        {
            System.Buffer.MemoryCopy(image.ToPointer(), yuv.Data.ToPointer(), length, length);
            Cv2.CvtColor(yuv, bgra, ColorConversionCodes.YUV2BGRA_YV12);
        }

        public unsafe void DrawYImage(byte[] image, int length)
        {
            Marshal.Copy(image, 0, yuv.Data, length);
            Cv2.CvtColor(yuv, bgra, ColorConversionCodes.YUV2BGRA_YV12);
        }

        public unsafe void DrawRGBAImage(IntPtr image, int length)
        {
            System.Buffer.MemoryCopy(image.ToPointer(), bgra.Data.ToPointer(), length, length);
        }

        public unsafe void Render(Control control)
        {
            try {
                Gl.BindTexture(TextureTarget.Texture2d, texture);
                Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, viewWidth, viewHeight, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bgra.Data);
                Gl.GenerateMipmap(TextureTarget.Texture2d);
            }
            catch {
            }
        }

        public virtual void OnSizeChanged(Control control)
        {
            area = new Rectangle(control.PointToScreen(control.ClientRectangle.Location), control.ClientSize);
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
                Gl.BindTexture(TextureTarget.Texture2d, texture);

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
