using Miscs;
using System;
using System.Windows.Forms;

namespace IRApplication.UI
{
    /// <summary>
    /// 主界面
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// 登录界面
        /// </summary>
        private LoginForm loginForm = new LoginForm();

        /// <summary>
        /// 主界面
        /// </summary>
        private HomeForm homeForm;

        /// <summary>
        /// 浏览器界面
        /// </summary>
        private WebViewForm webViewForm = new WebViewForm();

        public MainForm()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            EventEmitter.Instance.Subscribe(Common.Constants.EVENT_APP_CLOSE_APPLICATION, (arguments) => {
                Close();
            });

            EventEmitter.Instance.Subscribe(Common.Constants.EVENT_APP_SHOW_LOGIN_FORM, (arguments) => {
                if (homeForm != null) {
                    homeForm.Close();
                    homeForm = null;
                }

                ShowForm(loginForm);
            });

            EventEmitter.Instance.Subscribe(Common.Constants.EVENT_APP_SHOW_HOME_FORM, (arguments) => {
                if (homeForm == null) {
                    homeForm = new HomeForm();
                    ShowForm(homeForm);
                }
                else {
                    homeForm.RefreshChildForm();
                }
            });

            EventEmitter.Instance.Subscribe(Common.Constants.EVENT_APP_SHOW_WEBVIEW_FORM, (arguments) => {
                var configuration = Repository.Repository.LoadConfiguation();
                System.Diagnostics.Process.Start(configuration.information.serverUrl);
            });
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ShowForm(loginForm);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (loginForm != null) {
                loginForm.Close();
                loginForm = null;
            }

            if (homeForm != null) {
                homeForm.Close();
                homeForm = null;
            }

            if (webViewForm != null) {
                webViewForm.Close();
                webViewForm = null;
            }
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="form">窗口</param>
        private void ShowForm(Form form)
        {
            Controls.Clear();
            form.TopLevel = false;
            form.Dock = DockStyle.Fill;
            Controls.Add(form);
            form.Show();
        }
    }
}
