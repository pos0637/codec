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
        private RealtimeForm mRealtimeForm;

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

            if (mRealtimeForm == null) {
                var cell = CellServiceManager.Instance.GetService(configuartion.cells[0].name) as CellService;
                mRealtimeForm = new RealtimeForm(cell);
            }

            ShowForm(mRealtimeForm, "实时");
        }

        private void ShowForm(Form form, string title)
        {
            panelMain.Controls.Clear();
            form.TopLevel = false;
            form.Dock = DockStyle.Fill;
            panelMain.Controls.Add(form);
            form.Show();

            SetTitle(title);
            sidebarBtn.Visible = true;
            buttonReturn.Visible = false;
            panelSidebar.Visible = false;
        }

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

        private void HomeForm_Load(object sender, System.EventArgs e)
        {
            InitializeSideBar();
        }

        private void buttonRealtime_Click(object sender, System.EventArgs e)
        {
            ShowRealtimeForm();
        }

        private void buttonSidebarRealtime_Click(object sender, System.EventArgs e)
        {
            ShowRealtimeForm();
        }

        private void sidebarBtn_Click(object sender, System.EventArgs e)
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
