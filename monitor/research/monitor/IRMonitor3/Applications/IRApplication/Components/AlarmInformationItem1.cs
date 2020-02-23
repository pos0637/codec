using System.Drawing;
using System.Windows.Forms;

namespace IRApplication.Components
{
    /// <summary>
    /// 告警信息控件
    /// </summary>
    public partial class AlarmInformationItem1 : UserControl
    {
        public AlarmInformationItem1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="image">可见光图像</param>
        /// <param name="irImage">红外图像</param>
        /// <param name="detail">详情</param>
        public void SetData(Image image, Image irImage, string detail)
        {
            pictureBox_image.Image = image;
            pictureBox_irimage.Image = irImage;
            label_detail.Text = detail;
        }
    }
}
