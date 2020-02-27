using Devices;
using DrawTools;
using IRApplication.Components;
using IRService.Services.Cell;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace IRApplication.UI
{
    public partial class SetParameterForm : Form
    {
        private class GDICameraDeviceForm : CameraDeviceForm
        {
            protected override IRenderableControl RenderableControl {
                get {
                    if (renderableControl == null) {
                        renderableControl = new GDIRenderableControl();
                    }

                    return renderableControl;
                }
            }

            public GDICameraDeviceForm(CellService cell, IDevice device) : base(cell, device)
            {
            }
        }

        private class GDIIrCameraDeviceForm : IrCameraDeviceForm
        {
            protected override IRenderableControl RenderableControl {
                get {
                    if (renderableControl == null) {
                        renderableControl = new GDIRenderableControl();
                    }

                    return renderableControl;
                }
            }

            public GDIIrCameraDeviceForm(CellService cell, IDevice device) : base(cell, device)
            {
            }
        }

        /// <summary>
        /// 设备单元服务
        /// </summary>
        private readonly CellService cell;

        /// <summary>
        /// 绘制工具栏
        /// </summary>
        private DrawToolbar drawToolbar;

        /// <summary>
        /// 红外绘制区域
        /// </summary>
        private DrawArea drawAreaIRCamera;

        /// <summary>
        /// 可见光绘制区域
        /// </summary>
        private DrawArea drawAreaCamera;

        /// <summary>
        /// 人脸测温配置规则参数
        /// </summary>
        private readonly Dictionary<string, object> faceThermometryBasicParameter;

        /// <summary>
        /// 人脸测温基本配置参数
        /// </summary>
        private readonly Dictionary<string, object> faceThermometryRegion;

        /// <summary>
        /// 体温温度补偿配置参数
        /// </summary>
        private readonly Dictionary<string, object> bodyTemperatureCompensation;

        /// <summary>
        /// 黑体配置参数
        /// </summary>
        private readonly Dictionary<string, object> blackBody;

        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="cell">设备单元服务</param>
        public SetParameterForm(CellService cell)
        {
            InitializeComponent();

            this.cell = cell;

            faceThermometryBasicParameter = cell.GetFaceThermometryBasicParameter(null);
            if (faceThermometryBasicParameter != null) {
                text_facetbp_emissivity.Text = faceThermometryBasicParameter["emissivity"].ToString().ToLower();
                text_facetbp_distance.Text = faceThermometryBasicParameter["distance"].ToString().ToLower();
            }

            faceThermometryRegion = cell.GetFaceThermometryRegion(null);
            if (faceThermometryRegion != null) {
                text_facetr_targetSpeed.Text = faceThermometryRegion["targetSpeed"].ToString().ToLower();
                text_facetr_sensitivity.Text = faceThermometryRegion["sensitivity"].ToString().ToLower();
                text_facetr_alarmTemperature.Text = faceThermometryRegion["alarmTemperature"].ToString().ToLower();
            }

            bodyTemperatureCompensation = cell.GetBodyTemperatureCompensation(null);
            if (bodyTemperatureCompensation != null) {
                com_bodytc_type.Text = bodyTemperatureCompensation["type"].ToString().ToLower() == "auto" ? "自动" : "手动";
                text_bodytc_compensationValue.Text = bodyTemperatureCompensation["compensationValue"].ToString().ToLower();
                text_bodytc_smartCorrection.Text = bodyTemperatureCompensation["smartCorrection"].ToString().ToLower();
                com_bodytc_mode.Text = bodyTemperatureCompensation["environmentalTemperatureMode"].ToString().ToLower() == "auto" ? "自动" : "手动";
                text_bodytc_environ.Text = bodyTemperatureCompensation["environmentalTemperature"].ToString().ToLower();
            }

            blackBody = cell.GetBlackBody(null);
            if (blackBody != null) {
                check_black_enabled.Checked = blackBody["enabled"].ToString().ToLower() == "true" ? true : false;
                com_black_distance.Text = blackBody["distance"].ToString().ToLower();
                text_black_emissivity.Text = blackBody["emissivity"].ToString().ToLower();
                text_black_temp.Text = blackBody["temperature"].ToString().ToLower();
                CheckedEnabled();
            }
        }

        /// <summary>
        /// 绘画控件初始化
        /// </summary>
        private void InitializeDrawArea()
        {
            drawToolbar = new DrawToolbar {
                PointVisible = true,
                PolygonVisible = false,
                RectangleVisible = true,
                EllipseVisible = false,
                LineVisible = false,
                LineColorVisible = false,
                LineSizeVisible = false,
                PenStyleVisible = false,
                UndoVisible = false,
                RotateVisible = false
            };

            drawAreaCamera = new DrawArea();
            drawAreaIRCamera = new DrawArea();

            panelTool.Controls.Clear();
            panel_draw_camera.Controls.Clear();
            panel_draw_ircamera.Controls.Clear();

            var cameraDeviceForm = new GDICameraDeviceForm(cell, cell.devices[0]);
            var irCameraDeviceForm = new GDIIrCameraDeviceForm(cell, cell.devices[0]);
            ShowControl(panel_draw_camera, cameraDeviceForm);
            ShowControl(panel_draw_ircamera, irCameraDeviceForm);

            panelTool.Controls.Add(drawToolbar);
            cameraDeviceForm.Controls.Add(drawAreaCamera);
            irCameraDeviceForm.Controls.Add(drawAreaIRCamera);

            drawAreaCamera.BackgroundRenderEnabled = false;
            drawAreaCamera.Dock = DockStyle.Fill;
            drawAreaCamera.BackColor = Color.Transparent;
            drawAreaCamera.BringToFront();

            drawAreaIRCamera.BackgroundRenderEnabled = false;
            drawAreaIRCamera.Dock = DockStyle.Fill;
            drawAreaIRCamera.BackColor = Color.Transparent;
            drawAreaIRCamera.BringToFront();

            drawToolbar.Dock = DockStyle.Fill;
            drawToolbar.BackColor = Color.Transparent;
            drawToolbar.BringToFront();

            drawToolbar.PointEnabled = true;
            drawToolbar.RectangleEnabled = true;
            drawToolbar.CanControl = true;
            drawAreaCamera.CanControl = true;
            drawAreaIRCamera.CanControl = true;
            drawToolbar.DrawArea = drawAreaCamera;
            drawAreaCamera.Initialize(this, drawToolbar, new Size(cameraDeviceForm.Width, cameraDeviceForm.Height));
            drawAreaIRCamera.Initialize(this, drawToolbar, new Size(irCameraDeviceForm.Width, irCameraDeviceForm.Height));
            drawToolbar.SetDrawArea(drawAreaCamera, drawAreaIRCamera);
        }

        /// <summary>
        /// 判断黑体是否打开使能
        /// </summary>
        private void CheckedEnabled()
        {
            if (check_black_enabled.Checked) {
                com_black_distance.Enabled = true;
                text_black_emissivity.Enabled = true;
                text_black_temp.Enabled = true;
            }
            else {
                com_black_distance.Enabled = false;
                text_black_emissivity.Enabled = false;
                text_black_temp.Enabled = false;
            }
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

        private void OnTextChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb.Name == "com_bodytc_type") {
                if (cb.Text == "自动") {
                    text_bodytc_smartCorrection.Enabled = false;
                }
                else {
                    text_bodytc_smartCorrection.Enabled = true;
                }
            }
            else {
                if (cb.Text == "自动") {
                    text_bodytc_environ.Enabled = false;
                }
                else {
                    text_bodytc_environ.Enabled = true;
                }
            }
        }

        private void ParameterSetConfigForm_SizeChanged(object sender, EventArgs e)
        {
            InitializeDrawArea();
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            blackBody["enabled"] = check_black_enabled.Checked.ToString();
            blackBody["distance"] = com_black_distance.Text;
            blackBody["emissivity"] = text_black_emissivity.Text;
            blackBody["temperature"] = text_black_temp.Text;

            faceThermometryRegion["targetSpeed"] = text_facetr_targetSpeed.Text;
            faceThermometryRegion["sensitivity"] = text_facetr_sensitivity.Text;
            faceThermometryRegion["alarmTemperature"] = text_facetr_alarmTemperature.Text;

            var coutours = drawToolbar.coutours;
            if (coutours != null) {
                var getCoutours = drawToolbar.DrawArea.GetParameters();
                if (getCoutours != null) {
                    coutours.AddRange(getCoutours);
                }
            }

            foreach (var coutour in coutours) {
                if (coutour.Count == 4) {
                    faceThermometryRegion["rectangle"] = new Rectangle(coutour[3].X * 1000 / drawAreaCamera.Width, coutour[3].Y * 1000 / drawAreaCamera.Height, (coutour[1].X - coutour[0].X) * 1000 / drawAreaCamera.Width, -(coutour[2].Y - coutour[1].Y) * 1000 / drawAreaCamera.Height);
                }
                else if (coutour.Count == 1) {
                    blackBody["point"] = new Point(coutour[0].X * 1000 / drawAreaIRCamera.Width, coutour[0].Y * 1000 / drawAreaIRCamera.Height);
                }
            }

            faceThermometryBasicParameter["emissivity"] = text_facetbp_emissivity.Text;
            faceThermometryBasicParameter["distance"] = Convert.ToSingle(text_facetbp_distance.Text);

            bodyTemperatureCompensation["type"] = com_bodytc_type.Text == "自动" ? "auto" : "manual";
            bodyTemperatureCompensation["compensationValue"] = text_bodytc_compensationValue.Text;
            bodyTemperatureCompensation["smartCorrection"] = text_bodytc_smartCorrection.Text;
            bodyTemperatureCompensation["environmentalTemperatureMode"] = com_bodytc_mode.Text == "自动" ? "auto" : "manual";
            bodyTemperatureCompensation["environmentalTemperature"] = text_bodytc_environ.Text;

            if (!cell.SetFaceThermometryBasicParameter(null, faceThermometryBasicParameter) || !cell.SetFaceThermometryRegion(null, faceThermometryRegion) || !cell.SetBlackBody(null, blackBody) || !cell.SetBodyTemperatureCompensation(null, bodyTemperatureCompensation)) {
                MessageBox.Show("设置失败!");
                return;
            }

            MessageBox.Show("设置完成!");
        }

        private void CheckedChanged(object sender, EventArgs e)
        {
            CheckedEnabled();
        }
    }
}


