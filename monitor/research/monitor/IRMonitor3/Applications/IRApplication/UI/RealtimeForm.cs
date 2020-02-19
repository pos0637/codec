using IRApplication.Components;
using IRService.Services.Cell;
using System.Windows.Forms;

namespace IRApplication.UI
{
    public partial class RealtimeForm : Form
    {
        /// <summary>
        /// 设备单元服务
        /// </summary>
        /// <param name="cell"></param>
        public RealtimeForm(CellService cell)
        {
            InitializeComponent();

            ShowForm(new IrCameraDeviceForm(cell, cell.devices[0]));
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="form">窗口</param>
        private void ShowForm(Form form)
        {
            panelIRViewForm.Controls.Clear();
            form.TopLevel = false;
            form.Dock = DockStyle.Fill;
            panelIRViewForm.Controls.Add(form);
            form.Show();
        }
    }
}
