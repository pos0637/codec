using IRApplication.Common;
using IRApplication.Components;
using IRService.Services.Cell;
using Miscs;
using System.Drawing;
using System.Windows.Forms;

namespace IRApplication.UI
{
    public partial class RealtimeForm : Form
    {
        /// <summary>
        /// 设备单元服务
        /// </summary>
        private CellService cell;

        /// <summary>
        /// 告警列表
        /// </summary>
        private AlarmInformationList alarmInformationList;

        /// <summary>
        /// 设备单元服务
        /// </summary>
        /// <param name="cell"></param>
        public RealtimeForm(CellService cell)
        {
            InitializeComponent();

            this.cell = cell;
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

        private void buttonAutoFocus_Click(object sender, System.EventArgs e)
        {
            EventEmitter.Instance.Publish("EVENT_SERVICE_ON_ALARM", cell, cell.devices[0], Repository.Entities.Alarm.Type.HumanHighTemperature, new RectangleF(), "test");
        }
    }
}
