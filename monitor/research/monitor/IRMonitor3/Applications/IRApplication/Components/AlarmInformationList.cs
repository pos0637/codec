using Miscs;
using System;
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
        private System.Threading.Timer timer;

        public AlarmInformationList()
        {
            InitializeComponent();

            timer = new System.Threading.Timer((state) => {
                var end = DateTime.Now;
                var start = new DateTime(end.Year, end.Month, end.Day);
                var count = Repository.Repository.GetAlarmsCount(start, end);
                var alarms = Repository.Repository.GetLastAlarms(start, end, 10);
                if (IsHandleCreated && (alarms != null)) {
                    BeginInvoke((Action)(() => {
                        flowLayoutPanel1.Controls.Clear();
                        foreach (var alarm in alarms) {
                            var picture = new AlarmInformationItem();
                            picture.NameLabel1.Text = alarm.detail;
                            picture.pictureEdit.Image = ImageUtils.LoadImage(alarm.imageUrl);
                            picture.pictureIrEdit.Image = ImageUtils.LoadImage(alarm.irImageUrl);
                            picture.pictureEdit.BackgroundImageLayout = ImageLayout.Stretch;
                            picture.pictureEdit.SizeMode = PictureBoxSizeMode.StretchImage;
                            picture.pictureIrEdit.BackgroundImageLayout = ImageLayout.Stretch;
                            picture.pictureIrEdit.SizeMode = PictureBoxSizeMode.StretchImage;
                            flowLayoutPanel1.Controls.Add(picture);
                        }

                        label_alarm_count.Text = count.ToString();
                    }));
                }
            }, null, 0, 2000);
        }

        /// <summary>
        /// 防止控件闪烁
        /// </summary>
        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (timer != null) {
                timer.Dispose();
                timer = null;
            }

            base.OnHandleDestroyed(e);
        }
    }
}
