using System;
using System.Drawing;
using System.Windows.Forms;

namespace IRApplication.Components
{
    /// <summary>
    /// 告警信息列表控件
    /// </summary>
    public partial class AlarmInformationList : UserControl
    {
        /// <summary>
        /// 定时器
        /// </summary>
        private readonly System.Threading.Timer timer;

        public AlarmInformationList()
        {
            InitializeComponent();
            timer = new System.Threading.Timer((state) => {
                var alarms = Repository.Repository.GetLastAlarms(10);
                if ((IsHandleCreated) && (alarms != null)) {
                    BeginInvoke((Action)(() => {
                        flowLayoutPanel1.Controls.Clear();
                        foreach (var alarm in alarms) {
                            var picture = new AlarmInformationItem();
                            picture.NameLabel1.Text = alarm.cellName;
                            picture.pictureEdit.Image = alarm.imageUrl != null ? Image.FromFile(alarm.imageUrl) : null;
                            picture.pictureIrEdit.Image = alarm.irImageUrl != null ? Image.FromFile(alarm.irImageUrl) : null;
                            picture.pictureEdit.BackgroundImageLayout = ImageLayout.Stretch;
                            picture.pictureEdit.SizeMode = PictureBoxSizeMode.StretchImage;
                            picture.pictureIrEdit.BackgroundImageLayout = ImageLayout.Stretch;
                            picture.pictureIrEdit.SizeMode = PictureBoxSizeMode.StretchImage;
                            flowLayoutPanel1.Controls.Add(picture);
                        }
                    }));
                }
            }, null, 0, 2000);
        }

        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
    }
}
