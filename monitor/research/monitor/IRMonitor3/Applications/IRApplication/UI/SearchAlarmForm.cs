using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace IRApplication.UI
{
    public partial class SearchAlarmForm : Form
    {
        /// <summary>
        /// 页码
        /// </summary>
        private int page;

        /// <summary>
        /// 数量
        /// </summary>
        private const int num = 10;

        public SearchAlarmForm()
        {
            InitializeComponent();
            lowerPanel.Visible = false;
            alarmDataGridView.RowsDefaultCellStyle.BackColor = Color.Gray;
            alarmDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

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
        /// 获取告警信息
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="num">数量</param>
        private void GetPage(int page, int num)
        {
            if (page == 0) {
                page = 1;
            }

            // 清空控件
            alarmPicture.Image = null;
            irAlarmPicture.Image = null;
            last_Page_But.Enabled = false;
            next_Page_But.Enabled = false;
            alarmDataGridView.Rows.Clear();

            // 查询
            var count = Repository.Repository.GetAlarmsCount(dateTimePickerStart.Value, dateTimePickerEnd.Value);
            if (count == 0) {
                return;
            }

            var alarms = Repository.Repository.GetAlarms(dateTimePickerStart.Value, dateTimePickerEnd.Value, page, num);
            if (alarms.Count == 0) {
                GetPage(page - 1, num);
                return;
            }

            last_Page_But.Enabled = false;
            next_Page_But.Enabled = (count > num);
            pageIndexLab.Text = page.ToString();
            totalPageLab.Text = ((count / num) + 1).ToString();

            foreach (var alarm in alarms) {
                try {
                    var row = new DataGridViewRow {
                        Height = 40,
                        Tag = alarm
                    };

                    var id = alarmDataGridView.Rows.Add(row);
                    alarmDataGridView.Rows[id].Cells[0].Value = false;
                    alarmDataGridView.Rows[id].Cells[1].Value = alarm.id;
                    alarmDataGridView.Rows[id].Cells[2].Value = alarm.deviceName;
                    alarmDataGridView.Rows[id].Cells[3].Value = alarm.startTime;
                    alarmDataGridView.Rows[id].Cells[4].Value = alarm.detail;
                    alarmDataGridView.Rows[id].Cells[5].Value = alarm.comment;
                    alarmDataGridView.Rows[id].DefaultCellStyle.BackColor = Color.White;
                }
                catch {
                }
            }

            this.page = page;
        }

        private void SearchAlarmForm_Load(object sender, EventArgs e)
        {
            GetPage(1, num);
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if (dateTimePickerStart.Value >= dateTimePickerEnd.Value) {
                MessageBox.Show("时间选择错误!");
                return;
            }

            GetPage(1, num);
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkAllBtn_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in alarmDataGridView.Rows) {
                row.Cells[0].Value = true;
            }

            checkAllBtn.Text = $"全选({alarmDataGridView.Rows.Count})项";
            delBtn.Text = $"删除({alarmDataGridView.Rows.Count})项";
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void delBtn_Click(object sender, EventArgs e)
        {
            var list = new List<int>();
            foreach (DataGridViewRow row in alarmDataGridView.Rows) {
                if ((bool)row.Cells[0].Value) {
                    list.Add((int)row.Cells[1].Value);
                }
            }

            Repository.Repository.DeleteAlarms(list);
            GetPage(page, num);
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            if (alarmDataGridView.Rows.Count > 0) {
                alarmDataGridView.Columns[0].Visible = true;
                editBtn.Visible = false;
                editBtnCancel.Visible = true;
                lowerPanel.Visible = true;
            }
        }

        private void editBtnCancel_Click(object sender, EventArgs e)
        {
            alarmDataGridView.Columns[0].Visible = false;
            editBtn.Visible = true;
            editBtnCancel.Visible = false;
            lowerPanel.Visible = false;
            checkAllBtn.Text = "全选";

            foreach (DataGridViewRow row in alarmDataGridView.Rows) {
                row.Cells[0].Value = false;
            }
        }

        private void last_Page_But_Click(object sender, EventArgs e)
        {
            GetPage(page - 1, num);
        }

        private void next_Page_But_Click_1(object sender, EventArgs e)
        {
            GetPage(page + 1, num);
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
        }

        private void alarmDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((alarmDataGridView.CurrentCell != null)
                && (alarmDataGridView.CurrentCell.ColumnIndex == 0)
                || (alarmDataGridView.Rows.Count == 0)) {
                return;
            }

            if ((alarmDataGridView.CurrentRow == null)
                || (alarmDataGridView.CurrentRow.Tag == null)) {
                return;
            }

            var alarm = alarmDataGridView.CurrentRow.Tag as Alarm;
            alarmPicture.Tag = alarm.id;
            alarmPicture.Image = alarm.imageUrl != null ? System.Drawing.Image.FromFile(alarm.imageUrl) : null;
            irAlarmPicture.Tag = alarm.id;
            irAlarmPicture.Image = alarm.irImageUrl != null ? System.Drawing.Image.FromFile(alarm.irImageUrl) : null;
        }

        private void alarmDataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (alarmDataGridView.IsCurrentRowDirty) {
                alarmDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
    }
}
