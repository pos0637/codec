using System;
using System.Windows.Forms;

namespace IRApplication.Components
{
    /// <summary>
    /// 可渲染控件接口
    /// </summary>
    public interface IRenderableControl : IDisposable
    {
        /// <summary>
        /// 初始化控件
        /// </summary>
        /// <param name="parent">父控件</param>
        /// <param name="width">视图宽度</param>
        /// <param name="stride">视图对齐宽度</param>
        /// <param name="height">视图高度</param>
        bool InitializeComponent(Control parent, int width, int stride, int height);

        /// <summary>
        /// 获取控件创建参数
        /// </summary>
        /// <param name="createParams">创建参数</param>
        /// <returns>创建参数</returns>
        CreateParams GetParams(CreateParams createParams);

        /// <summary>
        /// 绘制图像
        /// </summary>
        /// <param name="image">YV12图像</param>
        /// <param name="length">长度</param>
        void DrawYV12Image(IntPtr image, int length);

        /// <summary>
        /// 绘制图像
        /// </summary>
        /// <param name="image">Y图像</param>
        /// <param name="length">长度</param>
        void DrawYImage(IntPtr image, int length);

        /// <summary>
        /// 绘制图像
        /// </summary>
        /// <param name="image">Y图像</param>
        /// <param name="length">长度</param>
        void DrawYImage(byte[] image, int length);

        /// <summary>
        /// 绘制图像
        /// </summary>
        /// <param name="mat">RGBA图像</param>
        /// <param name="length">长度</param>
        void DrawRGBAImage(IntPtr image, int length);

        /// <summary>
        /// 渲染
        /// </summary>
        /// <param name="control">控件</param> 
        void Render(Control control);

        /// <summary>
        /// 控件大小改变事件处理函数
        /// </summary>
        /// <param name="control">控件</param> 
        void OnSizeChanged(Control control);        
    }
}
