using IRApplication.UI;
using Miscs;
using Repository.Entities;
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

        /// <summary>
        /// 告警窗体
        /// </summary>
        public static AlarmInformationForm AlarmInfoForm = null;

        /// <summary>
        /// 滚动条位置
        /// </summary>
        private int scrollPosition;

        public AlarmInformationList()
        {
            InitializeComponent();

            // 鼠标滚轮事件
            flowLayoutPanel1.MouseWheel += new MouseEventHandler(rightPanel_MouseWheel);

            // 去除水平滚动条
            flowLayoutPanel1.AutoScroll = false;
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.WrapContents = false;
            flowLayoutPanel1.HorizontalScroll.Maximum = 0;
            flowLayoutPanel1.AutoScroll = true;

            timer = new System.Threading.Timer((state) => {
                var end = DateTime.Now;
                var start = new DateTime(end.Year, end.Month, end.Day);
                var count = Repository.Repository.GetAlarmsCount(start, end);
                var alarms = Repository.Repository.GetLastAlarms(start, end, 10);

                if (IsHandleCreated && (alarms != null)) {
                    BeginInvoke((Action)(() => {
                        flowLayoutPanel1.Controls.Clear();
                        foreach (var alarm in alarms) {
                            var item = new AlarmInformationItem();
                            item.NameLabel1.Text = alarm.detail;
                            item.pictureIrEdit.Image = ImageUtils.LoadImage(alarm.irImageUrl);
                            item.pictureIrEdit.BackgroundImageLayout = ImageLayout.Stretch;
                            item.pictureIrEdit.SizeMode = PictureBoxSizeMode.StretchImage;
                            item.pictureEdit.Image = ImageUtils.LoadImage(alarm.imageUrl);
                            item.pictureEdit.BackgroundImageLayout = ImageLayout.Stretch;
                            item.pictureEdit.SizeMode = PictureBoxSizeMode.StretchImage;
                            item.Tag = alarm;
                            item.NameLabel1.Tag = item.pictureIrEdit.Tag = item.pictureEdit.Tag = alarm;
                            item.NameLabel1.Click += new EventHandler(OnClick);
                            item.pictureEdit.Click += new EventHandler(OnClick);
                            item.pictureIrEdit.Click += new EventHandler(OnClick);
                            flowLayoutPanel1.Controls.Add(item);
                        }

                        flowLayoutPanel1.VerticalScroll.Value = scrollPosition;
                        flowLayoutPanel1.PerformLayout();
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

        private void OnClick(object sender, EventArgs e)
        {
            AlarmInformationForm.ShowAlarmInformationForm((sender as Control).Tag as Alarm);
        }

        private void flowLayoutPanel1_Scroll(object sender, ScrollEventArgs e)
        {
            scrollPosition = e.NewValue;
        }

        private void rightPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            var value = scrollPosition - e.Delta;
            // 底部滚动位置
            if (value >= flowLayoutPanel1.VerticalScroll.Maximum - flowLayoutPanel1.VerticalScroll.LargeChange) {
                scrollPosition = flowLayoutPanel1.VerticalScroll.Maximum - flowLayoutPanel1.VerticalScroll.LargeChange + 1;
            }
            else {
                scrollPosition -= e.Delta;
            }

            // 顶置位置为0
            if (scrollPosition < 0) {
                scrollPosition = 0;
            }
        }        
    }
}
