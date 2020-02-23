using IRApplication.Components;
using IRService.Services.Cell;
using System.Windows.Forms;

namespace IRApplication.UI
{
    public partial class DataVisualizationForm1 : Form
    {
        /// <summary>
        /// 设备单元服务
        /// </summary>
        private CellService cell;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cell">设备单元服务</param>
        public DataVisualizationForm1(CellService cell)
        {
            InitializeComponent();

            this.cell = cell;
            ShowControl(panel_camera, new CameraDeviceForm(cell, cell.devices[0]));
            ShowControl(panel_ircamera, new IrCameraDeviceForm(cell, cell.devices[0]));
            panel_alarms.Controls.Add(new AlarmInformationList1 { Dock = DockStyle.Fill, labelAlarmCount = label_alarm_count });
        }

        /// <summary>
        /// 显示控件
        /// </summary>
        /// <param name="parent">父控件</param>
        /// <param name="control">控件</param>
        private void ShowControl(Control parent, Control control)
        {
            ClearControl(parent);
            if (control is Form) {
                (control as Form).TopLevel = false;
            }
            control.Dock = DockStyle.Fill;
            parent.Controls.Add(control);
            control.Show();
        }

        /// <summary>
        /// 清理子控件
        /// </summary>
        /// <param name="parent">父控件</param>
        private void ClearControl(Control parent)
        {
            foreach (Control control in parent.Controls) {
                (control as Form)?.Close();
                control.Dispose();
            }

            parent.Controls.Clear();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
