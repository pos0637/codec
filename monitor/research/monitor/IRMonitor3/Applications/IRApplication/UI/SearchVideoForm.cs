using AxWMPLib;
using IRApplication.Components;
using Miscs;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IRApplication.UI
{
    public partial class SearchVideoForm : Form
    {
        /// <summary>
        /// 播放器
        /// </summary>
        private readonly AxWindowsMediaPlayer axWindowsMediaPlayer;

        /// <summary>
        /// 页码
        /// </summary>
        private int page;

        /// <summary>
        /// 数量
        /// </summary>
        private const int num = 10;

        public SearchVideoForm()
        {
            InitializeComponent();
            axWindowsMediaPlayer = new AxWindowsMediaPlayer {
                Dock = DockStyle.Fill
            };
            panel2.Controls.Add(axWindowsMediaPlayer);

            lowerPanel.Visible = false;

            // 初始化查询时间
            var now = DateTime.Now;
            var startTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            dateTimePickerStart.Value = startTime.AddDays(-10);
            dateTimePickerEnd.Value = startTime.AddDays(2).AddSeconds(-1);

            // 初始化翻页按钮
            last_Page_But.Enabled = false;
            next_Page_But.Enabled = false;
        }

        /// <summary>
        /// 获取录像信息
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="num">数量</param>
        private void GetPage(int page, int num)
        {
            if (page == 0) {
                page = 1;
            }

            // 清空控件
            last_Page_But.Enabled = false;
            next_Page_But.Enabled = false;
            pictureTableLayoutPanel.Controls.Clear();

            // 查询
            var count = Repository.Repository.GetRecordingsCount(dateTimePickerStart.Value, dateTimePickerEnd.Value);
            if (count == 0) {
                return;
            }

            var recordings = Repository.Repository.GetRecordings(dateTimePickerStart.Value, dateTimePickerEnd.Value, page, num);
            if (recordings.Count == 0) {
                GetPage(page - 1, num);
                return;
            }

            last_Page_But.Enabled = false;
            next_Page_But.Enabled = (count > num);
            pageIndexLab.Text = page.ToString();
            totalPageLab.Text = ((count / num) + 1).ToString();

            pictureTableLayoutPanel.Controls.Clear();
            foreach (var recording in recordings) {
                var videoItem = new VideoItem();
                videoItem.pictureBox1.Image = ImageUtils.LoadImage(recording.snapshotUrl);
                videoItem.pictureBox1.Tag = recording;
                videoItem.video_name_label.Text = recording.cellName;
                videoItem.pictureBox1.Click += new EventHandler(pictureBox1_Click);
                pictureTableLayoutPanel.Controls.Add(videoItem);
            }

            this.page = page;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer.URL = ((sender as Control).Tag as Recording).url;
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            GetPage(1, num);
        }

        private void next_Page_But_Click(object sender, EventArgs e)
        {
            GetPage(page + 1, num);
        }

        private void last_Page_But_Click(object sender, EventArgs e)
        {
            GetPage(page - 1, num);
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            if (pictureTableLayoutPanel.Controls.Count > 0) {
                editBtn.Visible = false;
                editBtnCancel.Visible = true;
                lowerPanel.Visible = true;
            }

            foreach (VideoItem row in pictureTableLayoutPanel.Controls) {
                row.checkBox.Visible = true;
            }
        }

        private void editBtnCancel_Click(object sender, EventArgs e)
        {
            editBtn.Visible = true;
            editBtnCancel.Visible = false;
            lowerPanel.Visible = false;
            checkAllBtn.Text = "全选";

            foreach (VideoItem row in pictureTableLayoutPanel.Controls) {
                row.checkBox.Visible = false;
            }
        }

        private void checkAllBtn_Click(object sender, EventArgs e)
        {
            foreach (VideoItem row in pictureTableLayoutPanel.Controls) {
                row.checkBox.Checked = true;
            }

            checkAllBtn.Text = $"全选({pictureTableLayoutPanel.Controls.Count})项";
            delBtn.Text = $"删除({pictureTableLayoutPanel.Controls.Count})项";
        }

        private void delBtn_Click(object sender, EventArgs e)
        {
            var list = new List<int>();
            foreach (VideoItem row in pictureTableLayoutPanel.Controls) {
                if (row.checkBox.Checked) {
                    list.Add((row.pictureBox1.Tag as Recording).id);
                }
            }

            Repository.Repository.DeleteRecordings(list);
            GetPage(page, num);
        }
    }
}
