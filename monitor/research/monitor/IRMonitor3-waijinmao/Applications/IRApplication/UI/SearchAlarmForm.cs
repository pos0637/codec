using Miscs;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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

            last_Page_But.Enabled = page > 1;
            next_Page_But.Enabled = page < (count / num) + 1;
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

        private void checkAllBtn_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in alarmDataGridView.Rows) {
                row.Cells[0].Value = true;
            }

            checkAllBtn.Text = $"全选({alarmDataGridView.Rows.Count})项";
            delBtn.Text = $"删除({alarmDataGridView.Rows.Count})项";
        }

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
            alarmPicture.Image = ImageUtils.LoadImage(alarm.imageUrl);
            irAlarmPicture.Tag = alarm.id;
            irAlarmPicture.Image = ImageUtils.LoadImage(alarm.irImageUrl);
        }

        private void alarmDataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (alarmDataGridView.IsCurrentRowDirty) {
                alarmDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void alarmDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
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
            AlarmInformationForm.ShowAlarmInformationForm(alarm);
        }

        private void buttonReport_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件保存的路径";
            var list = Repository.Repository.GetAlarms(dateTimePickerStart.Value, dateTimePickerEnd.Value);

            try {
                HSSFWorkbook workbook = new HSSFWorkbook();
                //创建一个sheet
                ISheet sheet1 = workbook.CreateSheet("sheet1");
                // 设置列宽,excel列宽每个像素是1/256
                sheet1.SetColumnWidth(0, 18 * 256);
                sheet1.SetColumnWidth(1, 18 * 256);
                sheet1.SetColumnWidth(2, 30 * 256);
                sheet1.SetColumnWidth(3, 30 * 256);
                sheet1.SetColumnWidth(4, 18 * 256);
                sheet1.SetColumnWidth(5, 18 * 256);

                IRow rowHeader = sheet1.CreateRow(0);//创建表头行
                rowHeader.CreateCell(0, CellType.String).SetCellValue("设备名称");
                rowHeader.CreateCell(1, CellType.String).SetCellValue("开始时间");
                rowHeader.CreateCell(2, CellType.String).SetCellValue("告警原因");
                rowHeader.CreateCell(3, CellType.String).SetCellValue("处理建议");
                rowHeader.CreateCell(4, CellType.String).SetCellValue("可见光图片");
                rowHeader.CreateCell(5, CellType.String).SetCellValue("外红图片");

                if (list.Count > 0) {
                    int rowline = 1;//从第二行开始(索引从0开始)
                    foreach (Alarm alarm in list) {
                        IRow row = sheet1.CreateRow(rowline);
                        //设置行高 ,excel行高度每个像素点是1/20
                        row.Height = 80 * 20;
                        //填入生产单号
                        row.CreateCell(0, CellType.String).SetCellValue(alarm.deviceName);
                        row.CreateCell(1, CellType.String).SetCellValue(alarm.startTime.ToString());
                        row.CreateCell(2, CellType.String).SetCellValue(alarm.detail);
                        row.CreateCell(3, CellType.String).SetCellValue(alarm.comment);

                        //将图片文件读入一个字符串
                        byte[] bytes = System.IO.File.ReadAllBytes(alarm.imageUrl);
                        int pictureIdx = workbook.AddPicture(bytes, PictureType.JPEG);

                        HSSFPatriarch patriarch = (HSSFPatriarch)sheet1.CreateDrawingPatriarch();
                        // 插图片的位置 
                        HSSFClientAnchor anchor = new HSSFClientAnchor(0, 0, 0, 0, 4, rowline, 5, rowline + 1);
                        //把图片插到相应的位置
                        HSSFPicture pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);

                        bytes = System.IO.File.ReadAllBytes(alarm.irImageUrl);
                        pictureIdx = workbook.AddPicture(bytes, PictureType.JPEG);
                        patriarch = (HSSFPatriarch)sheet1.CreateDrawingPatriarch();
                        // 插图片的位置 
                        anchor = new HSSFClientAnchor(0, 0, 0, 0, 5, rowline, 6, rowline + 1);
                        //把图片插到相应的位置
                        pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
                        rowline++;
                    }
                }

                if (dialog.ShowDialog() == DialogResult.OK) {
                    string foldPath = dialog.SelectedPath;
                    DirectoryInfo theFolder = new DirectoryInfo(foldPath);
                    using (Stream stream = File.OpenWrite(foldPath + "\\告警报表.xls")) {
                        workbook.Write(stream);
                    }
                }

                MessageBox.Show("下载成功");
            }
            catch (Exception ea) {
                MessageBox.Show("下载失败");
            }
        }
    }
}
