using IRService.Services.Cell;
using System;
using System.Windows.Forms;

namespace IRApplication.UI
{
    public partial class ParameterSetConfigForm : Form
    {
        public ParameterSetConfigForm(CellService cell)
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("设置完成!");
        }
    }
}
