using Common;
using IRApplication.Common;
using IRService.Services.Cell;
using Miscs;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace IRApplication.UI
{
    public partial class HomeForm : Form
    {
        /// <summary>
        /// 实时窗体
        /// </summary>
        private RealtimeForm realtimeForm;

        /// <summary>
        /// 数据可视化窗体
        /// </summary>
        private DataVisualizationForm1 dataVisualizationForm1;

        /// <summary>
        /// 查询告警窗体
        /// </summary>
        private readonly SearchAlarmForm searchAlarmForm = new SearchAlarmForm();

        /// <summary>
        /// 设置窗体
        /// </summary>
        private readonly SettingsForm settingsForm = new SettingsForm();

        /// <summary>
        /// 参数设置窗体
        /// </summary>
        private ParameterSetConfigForm parameterSetConfigForm;

        /// <summary>
        /// 用户手册窗体
        /// </summary>
        private readonly UserManualForm userManualForm = new UserManualForm();

        public HomeForm()
        {
            InitializeComponent();
            settingsForm.ShowParameterSetConfigForm = ShowParameterSetConfigForm;
            settingsForm.ShowUserManualForm = ShowUserManualForm;
        }

        public void RefreshChildForm()
        {
        }

        /// <summary>
        /// 显示实时窗体
        /// </summary>
        private void ShowRealtimeForm()
        {
            var configuartion = Repository.Repository.LoadConfiguation();
            if (configuartion.cells.Length == 0) {
                MessageBox.Show("无可连接的设备!");
                return;
            }

            CloseRealPlayControl();
            var cell = CellServiceManager.Instance.GetService(configuartion.cells[0].name) as CellService;
            realtimeForm = new RealtimeForm(cell);
            ShowControl(realtimeForm, "实时");
        }

        /// <summary>
        /// 显示数据可视化窗体
        /// </summary>
        private void ShowDataVisualizationForm()
        {
            var configuartion = Repository.Repository.LoadConfiguation();
            if (configuartion.cells.Length == 0) {
                MessageBox.Show("无可连接的设备!");
                return;
            }

            CloseRealPlayControl();
            ShowNormalControl(tableLayoutPanelMain, "主界面");

            var cell = CellServiceManager.Instance.GetService(configuartion.cells[0].name) as CellService;
            dataVisualizationForm1 = new DataVisualizationForm1(cell);
            dataVisualizationForm1.Show();
        }

        /// <summary>
        /// 显示参数设置窗体
        /// </summary>
        private void ShowParameterSetConfigForm()
        {
            var configuartion = Repository.Repository.LoadConfiguation();
            if (configuartion.cells.Length == 0) {
                MessageBox.Show("无可连接的设备!");
                return;
            }

            CloseRealPlayControl();
            var cell = CellServiceManager.Instance.GetService(configuartion.cells[0].name) as CellService;
            parameterSetConfigForm = new ParameterSetConfigForm(cell);
            ShowControl(parameterSetConfigForm, "参数设置");
        }

        /// <summary>
        /// 显示用户手册窗体
        /// </summary>
        private void ShowUserManualForm()
        {
            ShowNormalControl(userManualForm, "用户手册");
        }

        /// <summary>
        /// 显示控件
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="title">标题</param>
        private void ShowNormalControl(Control control, string title)
        {
            // 关闭实时播放控件
            CloseRealPlayControl();
            ShowControl(control, title);
        }

        /// <summary>
        /// 显示实时播放控件
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="title">标题</param>
        private void ShowControl(Control control, string title)
        {
            panelMain.Controls.Clear();
            if (control is Form) {
                (control as Form).TopLevel = false;
            }
            control.Dock = DockStyle.Fill;
            panelMain.Controls.Add(control);
            control.Show();

            SetTitle(title);
            CloseSidebar();
            sidebarBtn.Visible = true;
            buttonReturn.Visible = false;
            panelSidebar.Visible = false;
        }

        /// <summary>
        /// 设置标题栏
        /// </summary>
        /// <param name="title">标题</param>
        private void SetTitle(string title)
        {
            titleLabel.Text = title;
            titleLabel.Location = new Point(Width / 2 - titleLabel.Width / 2, titleLabel.Location.Y);
        }

        /// <summary>
        /// 关闭实时播放控件
        /// </summary>
        private void CloseRealPlayControl()
        {
            if (realtimeForm != null) {
                realtimeForm.Close();
                realtimeForm = null;
            }

            if (dataVisualizationForm1 != null) {
                dataVisualizationForm1.Close();
                dataVisualizationForm1 = null;
            }

            if (parameterSetConfigForm != null) {
                parameterSetConfigForm.Close();
                parameterSetConfigForm = null;
            }
        }

        #region 侧边栏

        /// <summary>
        /// 侧拉框宽度
        /// </summary>
        private int mSidebarWidth;

        private void InitializeSideBar()
        {
            panelSidebar.Location = new Point(0, 0);
            panelSidebar.Size = new Size(panelSidebar.Width, this.Height);
            panelSidebar.BringToFront();

            for (var i = 0; i < panelSidebar.Controls.Count; i++) {
                panelSidebar.Controls[i].Tag = panelSidebar.Width - panelSidebar.Controls[i].Right;
            }

            mSidebarWidth = panelSidebar.Width;
            CloseSidebar();
            sidebarBtn.Visible = false;
        }

        private void SetPanelVisible(Panel panel, bool visiable, int width)
        {
            try {
                panel.Width = width;
                foreach (Control item in panel.Controls) {
                    int border = (int)item.Tag;
                    item.Location = new Point(width - border - item.Width, item.Location.Y);
                }

                panel.Visible = visiable;
            }
            catch (Exception ex) {
                Tracker.LogE(ex);
            }
        }

        private void OpenSidebar()
        {
            if (panelSidebar.Width != 0) {
                return;
            }

            for (var i = 0; i <= mSidebarWidth; i += 20) {
                SetPanelVisible(panelSidebar, true, i);
                panelSidebar.Refresh();
            }

            SetPanelVisible(panelSidebar, true, mSidebarWidth);
        }

        private void CloseSidebar()
        {
            if (panelSidebar.Width == 0) {
                return;
            }

            for (var i = mSidebarWidth; i >= 0; i -= 20) {
                SetPanelVisible(panelSidebar, false, i);
                panelSidebar.Refresh();
            }

            SetPanelVisible(panelSidebar, false, 0);
        }

        #endregion

        private void HomeForm_Load(object sender, EventArgs e)
        {
            InitializeSideBar();
        }

        private void buttonSidebarHome_Click(object sender, EventArgs e)
        {
            ShowNormalControl(tableLayoutPanelMain, "主界面");
        }

        private void buttonRealtime_Click(object sender, EventArgs e)
        {
            ShowRealtimeForm();
        }

        private void buttonSidebarRealtime_Click(object sender, EventArgs e)
        {
            ShowRealtimeForm();
        }

        private void buttonSecondaryAnalysis_Click(object sender, EventArgs e)
        {
            ShowDataVisualizationForm();
        }

        private void buttonSidebarSecondaryAnalysis_Click(object sender, EventArgs e)
        {
            ShowDataVisualizationForm();
        }

        private void buttonSidebarAlarmSearch_Click(object sender, EventArgs e)
        {
            ShowNormalControl(searchAlarmForm, "告警查询");
        }

        private void buttonConfig_Click(object sender, EventArgs e)
        {
            ShowNormalControl(settingsForm, "配置");
        }

        private void buttonSidebarConfig_Click(object sender, EventArgs e)
        {
            ShowNormalControl(settingsForm, "配置");
        }

        private void sidebarBtn_Click(object sender, EventArgs e)
        {
            OpenSidebar();
        }

        private void buttonCloseSidebar_Click(object sender, EventArgs e)
        {
            CloseSidebar();
        }

        private void buttonCloseClient_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("是否退出系统?", "提示", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK) {
                EventEmitter.Instance.Publish(Constants.EVENT_APP_CLOSE_APPLICATION);
            }
        }
    }
}
