using System.Drawing;
using System.Windows.Forms;

namespace IRApplication.Components
{
    /// <summary>
    /// 人流量信息控件
    /// </summary>
    public partial class PeopleInformationItem : UserControl
    {
        public PeopleInformationItem()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="image">可见光图像</param>
        public void SetData(Image image)
        {
            pictureBox1.Image = image;
        }
    }
}


