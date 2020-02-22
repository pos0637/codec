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
        /// 查询告警窗体
        /// </summary>
        private readonly SearchAlarmForm searchAlarmForm = new SearchAlarmForm();

        /// <summary>
        /// 设置窗体
        /// </summary>
        private readonly SettingsForm settingsForm = new SettingsForm();

        public HomeForm()
        {
            InitializeComponent();
        }

        public void RefreshChildForm()
        {
        }

        private void ShowRealtimeForm()
        {
            var configuartion = Repository.Repository.LoadConfiguation();
            if (configuartion.cells.Length == 0) {
                return;
            }

            if (realtimeForm == null) {
                var cell = CellServiceManager.Instance.GetService(configuartion.cells[0].name) as CellService;
                realtimeForm = new RealtimeForm(cell);
            }

            ShowControl(realtimeForm, "实时");
        }

        /// <summary>
        /// 显示控件
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="title">标题</param>
        private void ShowControl(Control control, string title)
        {
            // 关闭实时预览窗体
            if ((realtimeForm != null) && (realtimeForm != control)) {
                realtimeForm.Close();
                realtimeForm = null;
            }

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
            ShowControl(tableLayoutPanelMain, "主界面");
        }

        private void buttonRealtime_Click(object sender, EventArgs e)
        {
            ShowRealtimeForm();
        }

        private void buttonSidebarRealtime_Click(object sender, EventArgs e)
        {
            ShowRealtimeForm();
        }

        private void buttonSidebarAlarmSearch_Click(object sender, EventArgs e)
        {
            ShowControl(searchAlarmForm, "告警查询");
        }

        private void buttonConfig_Click(object sender, EventArgs e)
        {
            ShowControl(settingsForm, "配置");
        }

        private void buttonSidebarConfig_Click(object sender, EventArgs e)
        {
            ShowControl(settingsForm, "配置");
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
