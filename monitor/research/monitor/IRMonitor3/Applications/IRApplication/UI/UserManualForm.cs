using Common;
using System;
using System.Windows.Forms;

namespace IRApplication.UI
{
    public partial class UserManualForm : Form
    {
        public UserManualForm()
        {
            InitializeComponent();
        }

        private void Start_Butn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UserManualForm_Load(object sender, EventArgs e)
        {
            try {
                webBrowser1.Navigate($"{AppDomain.CurrentDomain.BaseDirectory}help.mht");
            }
            catch (Exception ex) {
                Tracker.LogE(ex);
            }
        }
    }
}

