using IRApplication.Components;
using IRService.Services.Cell;
using System.Windows.Forms;

namespace IRApplication.UI
{
    public partial class RealtimeForm : Form
    {
        private AlarmInformationList alarmInformationList;

        /// <summary>
        /// 设备单元服务
        /// </summary>
        /// <param name="cell"></param>
        public RealtimeForm(CellService cell)
        {
            InitializeComponent();

            alarmInformationList = new AlarmInformationList {
                Dock = DockStyle.Fill
            };
            panelAlarmForm.Controls.Add(alarmInformationList);

            ShowForm(new CameraDeviceForm(cell, cell.devices[0]));
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
