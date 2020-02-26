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
        /// 视图模式
        /// </summary>
        private enum DisplayMode
        {
            OneViews = 0,
            TwoViews,
            FourViews,
            SixteenViews,
            Max
        }

        /// <summary>
        /// 调色板模式
        /// </summary>
        private enum PaletteMode
        {
            WhiteHot = 0,
            BlackHot,
            Fusion1,
            Rainbow,
            Fusion2,
            Ironbow1,
            Ironbow2,
            Sepia,
            Color1,
            Color2,
            IceFire,
            Rain,
            RedHot,
            GreenHot,
            DeepBlue,
            Max
        }

        /// <summary>
        /// 设备单元服务
        /// </summary>
        private readonly CellService cell;

        /// <summary>
        /// 视图模式
        /// </summary>
        private DisplayMode displayMode;

        /// <summary>
        /// 调色板模式
        /// </summary>
        private PaletteMode paletteMode;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cell">设备单元服务</param>
        public RealtimeForm(CellService cell)
        {
            InitializeComponent();

            this.cell = cell;
            SetPaletteMode(PaletteMode.Ironbow1);
            SetDisplayMode(DisplayMode.TwoViews);
            panelAlarmList.Controls.Add(new AlarmInformationList { Dock = DockStyle.Fill });
        }

        /// <summary>
        /// 显示控件
        /// </summary>
        /// <param name="parent">父控件</param>
        /// <param name="id">控件索引</param>
        /// <param name="control">控件</param>
        private void ShowControl(Control parent, int id, Control control)
        {
            foreach (Control container in parent.Controls) {
                if (int.Parse(container.Tag.ToString()) == id) {
                    if (control is Form) {
                        (control as Form).TopLevel = false;
                    }
                    control.Dock = DockStyle.Fill;
                    container.Controls.Add(control);
                    control.Show();
                }
            }
        }

        /// <summary>
        /// 清理子控件
        /// </summary>
        /// <param name="parent">父控件</param>
        private void ClearControl(Control parent)
        {
            foreach (Control control in parent.Controls) {
                foreach (Form form in control.Controls) {
                    form.Close();
                    form.Dispose();
                }
                control.Controls.Clear();
            }
        }

        /// <summary>
        /// 设置显示模式
        /// </summary>
        /// <param name="mode">显示模式</param>
        private void SetDisplayMode(DisplayMode mode)
        {
            tableLayoutPanel_1view.Visible = false;
            ClearControl(tableLayoutPanel_1view);
            tableLayoutPanel_2views.Visible = false;
            ClearControl(tableLayoutPanel_2views);
            tableLayoutPanel_4views.Visible = false;
            ClearControl(tableLayoutPanel_4views);
            tableLayoutPanel_16views.Visible = false;
            ClearControl(tableLayoutPanel_16views);

            switch (mode) {
                case DisplayMode.OneViews:
                    tableLayoutPanel_1view.Visible = true;
                    ShowControl(tableLayoutPanel_1view, 1, new CameraDeviceForm(cell, cell.devices[0]));
                    buttonDisplayMode.Text = "单视图";
                    break;
                case DisplayMode.TwoViews:
                    tableLayoutPanel_2views.Visible = true;
                    ShowControl(tableLayoutPanel_2views, 1, new CameraDeviceForm(cell, cell.devices[0]));
                    ShowControl(tableLayoutPanel_2views, 2, new IrCameraDeviceForm(cell, cell.devices[0]));
                    buttonDisplayMode.Text = "双视图";
                    break;
                case DisplayMode.FourViews:
                    tableLayoutPanel_4views.Visible = true;
                    ShowControl(tableLayoutPanel_4views, 1, new CameraDeviceForm(cell, cell.devices[0]));
                    ShowControl(tableLayoutPanel_4views, 2, new IrCameraDeviceForm(cell, cell.devices[0]));
                    buttonDisplayMode.Text = "四视图";
                    break;
                case DisplayMode.SixteenViews:
                    tableLayoutPanel_16views.Visible = true;
                    ShowControl(tableLayoutPanel_16views, 1, new CameraDeviceForm(cell, cell.devices[0]));
                    ShowControl(tableLayoutPanel_16views, 2, new IrCameraDeviceForm(cell, cell.devices[0]));
                    buttonDisplayMode.Text = "电视墙";
                    break;
                default:
                    return;
            }

            displayMode = mode;
        }

        private void SetPaletteMode(PaletteMode mode)
        {
            // TODO: delete it
            cell.SetPaletteMode(null, mode.ToString());
            paletteMode = mode;
        }

        private void buttonDisplayMode_Click(object sender, System.EventArgs e)
        {
            SetDisplayMode((DisplayMode)(((int)displayMode + 1) % (int)DisplayMode.Max));
        }

        private void buttonPalette_Click(object sender, System.EventArgs e)
        {
            SetPaletteMode((PaletteMode)(((int)paletteMode + 1) % (int)PaletteMode.Max));
        }

        private void buttonSaveBitmap_Click(object sender, System.EventArgs e)
        {
            // TODO: delete it
            EventEmitter.Instance.Publish(IRService.Common.Constants.EVENT_SERVICE_ON_ALARM, cell, cell.devices[0], Repository.Entities.Alarm.AlarmType.Manual, new RectangleF(), "用户触发告警");
        }
    }
}
