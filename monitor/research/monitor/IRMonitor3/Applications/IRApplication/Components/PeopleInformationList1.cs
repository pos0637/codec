using IRApplication.UI;
using Miscs;
using Repository.Entities;
using System;
using System.Threading;
using System.Windows.Forms;

namespace IRApplication.Components
{
    /// <summary>
    /// 人流量信息列表控件
    /// </summary>
    public partial class PeopleInformationList1 : UserControl
    {
        /// <summary>
        /// 人数标签
        /// </summary>
        public Label labelPeopleCount;

        /// <summary>
        /// 定时器
        /// </summary>
        private System.Threading.Timer timer;

        /// <summary>
        /// 滚动条位置
        /// </summary>
        private int scrollPosition;

        /// <summary>
        /// 通过人员数量
        /// </summary>
        private int count;

        public PeopleInformationList1()
        {
            InitializeComponent();
            // 鼠标滚轮事件
            flowLayoutPanel1.MouseWheel += new MouseEventHandler(rightPanel_MouseWheel);

            // 去除垂直滚动条
            flowLayoutPanel1.AutoScroll = false;
            flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel1.WrapContents = false;
            flowLayoutPanel1.VerticalScroll.Maximum = 0;
            flowLayoutPanel1.AutoScroll = true;

            timer = new System.Threading.Timer((state) => {
                var end = DateTime.Now;
                var start = new DateTime(end.Year, end.Month, end.Day);
                count = Repository.Repository.GetPeoplesCount();
                var people = Repository.Repository.GetPeoples(start, end, 10);
                if (IsHandleCreated && (people != null)) {
                    BeginInvoke((Action)(() => {
                        flowLayoutPanel1.Controls.Clear();
                        foreach (var person in people) {
                            var item = new PeopleInformationItem();
                            item.SetData(ImageUtils.LoadImage(person.url));
                            flowLayoutPanel1.Controls.Add(item);
                        }

                        if (labelPeopleCount != null) {
                            labelPeopleCount.Text = count.ToString();
                        }
                    }));
                }
            }, null, 0, 1000);

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

