using System;
using System.Windows.Forms;

namespace IRApplication.UI
{
    public partial class SettingsForm : Form
    {
        /// <summary>
        /// 显示参数设置界面
        /// </summary>
        public Action ShowParameterSetConfigForm { get; set; }

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void button_parameter_Click(object sender, EventArgs e)
        {
            ShowParameterSetConfigForm?.Invoke();
        }
    }
}
