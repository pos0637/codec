using System;
using System.Windows.Forms;

namespace IRApplication.UI
{
    public partial class SettingsForm : Form
    {
        /// <summary>
        /// 显示参数设置窗体
        /// </summary>
        public Action ShowParameterSetConfigForm { get; set; }

        /// <summary>
        /// 显示配置窗体
        /// </summary>
        public Action ShowConfigForm { get; set; }

        /// <summary>
        /// 显示用户手册窗体
        /// </summary>
        public Action ShowUserManualForm { get; set; }

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void button_parameter_Click(object sender, EventArgs e)
        {
            ShowParameterSetConfigForm?.Invoke();
        }

        private void button_deviceInfo_Click(object sender, EventArgs e)
        {
            ShowConfigForm?.Invoke();
        }

        private void button_handbook_Click(object sender, EventArgs e)
        {
            ShowUserManualForm?.Invoke();
        }
    }
}
