using Miscs;
using Repository.Entities;
using System;
using System.Windows.Forms;

namespace IRApplication.UI
{
    public partial class AlarmInformationForm : Form
    {
        /// <summary>
        /// 告警信息窗体
        /// </summary>
        private static AlarmInformationForm form;

        /// <summary>
        /// 告警
        /// </summary>
        private Alarm alarm;

        /// <summary>
        /// 显示告警窗体
        /// </summary>
        /// <param name="alarm">告警信息</param>
        public static void ShowAlarmInformationForm(Alarm alarm)
        {
            if (form == null) {
                form = new AlarmInformationForm();
            }

            form.SetData(alarm);
            form.Activate();
            form.Show();
        }

        public AlarmInformationForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设置告警
        /// </summary>
        /// <param name="alarm">告警</param>
        public void SetData(Alarm alarm)
        {
            this.alarm = alarm;

            info_listBox.Items.Clear();
            pictureBox.Image = null;
            ir_pictureBox.Image = null;

            pictureBox.Image = ImageUtils.LoadImage(alarm.imageUrl);
            ir_pictureBox.Image = ImageUtils.LoadImage(alarm.irImageUrl);
            info_listBox.Items.Add($"设备单元名称: {alarm.cellName}");
            info_listBox.Items.Add($"设备名称: {alarm.deviceName}");
            info_listBox.Items.Add($"选区名称: {alarm.selectionName}");
            info_listBox.Items.Add($"开始时间: {alarm.startTime.Value.ToString()}");
            info_listBox.Items.Add($"详细信息: {alarm.detail}");
            info_listBox.Items.Add($"处理意见：{alarm.comment}");
            set_info_richTextBox.Text = alarm.comment;
            checkBox1.Checked = true;
        }

        private void save_button_Click(object sender, EventArgs e)
        {
            alarm.comment = set_info_richTextBox.Text;
            alarm.solved = checkBox1.Checked;
            Repository.Repository.UpdateAlarm(alarm);

            form = null;
            Close();
        }

        private void close_button_Click(object sender, EventArgs e)
        {
            form = null;
            Close();
        }

        private void AlarmInfoForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            form = null;
        }
    }
}
