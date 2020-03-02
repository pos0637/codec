using IRApplication.Common;
using Miscs;
using System;
using System.Windows.Forms;

namespace IRApplication.UI
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void buttonCloseClient_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("是否退出系统?", "提示", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK) {
                EventEmitter.Instance.Publish(Constants.EVENT_APP_CLOSE_APPLICATION);
            }
        }

        private void buttonSearchWifi_Click(object sender, EventArgs e)
        {
            EventEmitter.Instance.Publish(Constants.EVENT_APP_SHOW_HOME_FORM);
        }

        private void buttonConnectCloud_Click(object sender, EventArgs e)
        {
            EventEmitter.Instance.Publish(Constants.EVENT_APP_SHOW_WEBVIEW_FORM);
        }
    }
}
