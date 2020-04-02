using System;
using System.Drawing;
using System.Windows.Forms;

namespace DrawTools2
{
    /// <summary>
    /// 绘图工具栏
    /// </summary>
    public partial class DrawToolbar : UserControl
    {
        #region Properties
        /// <summary>
        /// 绘制区域
        /// </summary>
        public DrawArea DrawArea { get; set; }

        /// <summary>
        /// 是否能使用
        /// </summary>
        public Boolean CanControl { get; set; }

        /// <summary>
        /// 点绘图工具是否可用
        /// </summary>
        public Boolean PointEnabled { get; set; }

        /// <summary>
        /// 矩形绘图工具是否可用
        /// </summary>
        public Boolean RectangleEnabled { get; set; }

        /// <summary>
        /// 椭圆绘图工具是否可用
        /// </summary>
        public Boolean EllipseEnabled { get; set; }

        /// <summary>
        /// 线绘图工具是否可用
        /// </summary>
        public Boolean LineEnabled { get; set; }

        /// <summary>
        /// 多边形绘图工具是否可用
        /// </summary>
        public Boolean PolygonEnabled { get; set; }

        /// <summary>
        /// 点绘图工具是否可见
        /// </summary>
        public Boolean PointVisible {
            get {
                return this.toolStripButtonPoint.Visible;
            }
            set {
                this.toolStripButtonPoint.Visible = value;
            }
        }

        /// <summary>
        /// 矩形绘图工具是否可见
        /// </summary>
        public Boolean RectangleVisible {
            get {
                return this.toolStripButtonRectangle.Visible;
            }
            set {
                this.toolStripButtonRectangle.Visible = value;
            }
        }

        /// <summary>
        /// 椭圆绘图工具是否可见
        /// </summary>
        public Boolean EllipseVisible {
            get {
                return this.toolStripButtonEllipse.Visible;
            }
            set {
                this.toolStripButtonEllipse.Visible = value;
            }
        }

        /// <summary>
        /// 线绘图工具是否可见
        /// </summary>
        public Boolean LineVisible {
            get {
                return this.toolStripButtonLine.Visible;
            }
            set {
                this.toolStripButtonLine.Visible = value;
            }
        }

        /// <summary>
        /// 多边形绘图工具是否可见
        /// </summary>
        public Boolean PolygonVisible {
            get {
                return this.tsbPolygon.Visible;
            }
            set {
                this.tsbPolygon.Visible = value;
            }
        }

        /// <summary>
        /// 绘图线颜色是否可见
        /// </summary>
        public Boolean LineColorVisible {
            get {
                return this.tsbLineColor.Visible;
            }
            set {
                this.tsbLineColor.Visible = value;
                this.toolStripSeparator6.Visible = value;
            }
        }

        /// <summary>
        /// 绘图线尺寸是否可见
        /// </summary>
        public Boolean LineSizeVisible {
            get {
                return this.toolStripDropDownButton1.Visible;
            }
            set {
                this.toolStripDropDownButton1.Visible = value;
            }
        }

        /// <summary>
        /// 绘图笔样式是否可见
        /// </summary>
        public Boolean PenStyleVisible {
            get {
                return this.toolStripDropDownButton2.Visible;
            }
            set {
                this.toolStripDropDownButton2.Visible = value;
            }
        }

        /// <summary>
        /// 撤销/重做是否可见
        /// </summary>
        public Boolean UndoVisible {
            get {
                return this.toolStripButtonUndo.Visible;
            }
            set {
                this.toolStripButtonUndo.Visible = value;
                this.toolStripButtonRedo.Visible = value;
                this.toolStripSeparator3.Visible = value;
            }
        }

        /// <summary>
        /// 放大缩小是否可见
        /// </summary>
        public Boolean ZoomVisible {
            get {
                return this.tsbZoomIn.Visible;
            }
            set {
                this.tsbZoomIn.Visible = value;
                this.tsbZoomOut.Visible = value;
                this.tsbZoomReset.Visible = value;
                this.toolStripSeparator4.Visible = value;
            }
        }

        /// <summary>
        /// 旋转是否可见
        /// </summary>
        public Boolean RotateVisible {
            get {
                return this.tsbRotateLeft.Visible;
            }
            set {
                this.tsbRotateLeft.Visible = value;
                this.tsbRotateRight.Visible = value;
                this.tsbRotateReset.Visible = value;
                this.toolStripSeparator5.Visible = value;
            }
        }

        /// <summary>
        /// 移动图片是否可见
        /// </summary>
        public Boolean PanVisible {
            get {
                return this.tsbPanMode.Visible;
            }
            set {
                this.tsbPanMode.Visible = value;
                this.tsbPanReset.Visible = value;
            }
        }

        #endregion

        private bool _panMode = false;

        public DrawToolbar()
        {
            InitializeComponent();

            // Submit to Idle event to set controls state at idle time
            Application.Idle += delegate { SetStateOfControls(); };
        }

        #region Toolbar Event Handlers
        private void toolStripButtonPointer_Click(object sender, EventArgs e)
        {
            CommandPointer();
        }

        private void toolStripButtonPoint_Click(object sender, EventArgs e)
        {
            if (!CanControl || !PointEnabled) {
                CommandPointer();
                return;
            }

            CommandPoint();
        }

        private void toolStripButtonRectangle_Click(object sender, EventArgs e)
        {
            if (!CanControl || !RectangleEnabled) {
                CommandPointer();
                return;
            }

            CommandRectangle();
        }

        private void toolStripButtonEllipse_Click(object sender, EventArgs e)
        {
            if (!CanControl || !EllipseEnabled) {
                CommandPointer();
                return;
            }

            CommandEllipse();
        }

        private void toolStripButtonLine_Click(object sender, EventArgs e)
        {
            if (!CanControl || !LineEnabled) {
                CommandPointer();
                return;
            }

            CommandLine();
        }

        private void tsbPolygon_Click(object sender, EventArgs e)
        {
            if (!CanControl || !PolygonEnabled) {
                CommandPointer();
                return;
            }

            CommandPolygon();
        }

        private void toolStripButtonDel_Click(object sender, EventArgs e)
        {
            DeleteSelection();
        }

        private void tsbLineColor_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            dlgColor.AllowFullOpen = true;
            dlgColor.AnyColor = true;
            if (dlgColor.ShowDialog() ==
                DialogResult.OK) {
                DrawArea.LineColor = Color.FromArgb(255, dlgColor.Color);
                tsbLineColor.BackColor = Color.FromArgb(255, dlgColor.Color);
            }
        }

        private void thinnestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            DrawArea.LineWidth = -1;
        }

        private void thinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            DrawArea.LineWidth = 2;
        }

        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            DrawArea.LineWidth = 5;
        }

        private void thickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            DrawArea.LineWidth = 10;
        }

        private void thickestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            DrawArea.LineWidth = 15;
        }

        private void toolStripMenuItemGenericPen_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            DrawArea.PenType = DrawingPens.PenType.Generic;
            DrawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.Generic);
        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            DrawArea.PenType = DrawingPens.PenType.RedPen;
            DrawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.RedPen);
        }

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            DrawArea.PenType = DrawingPens.PenType.BluePen;
            DrawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.BluePen);
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            DrawArea.PenType = DrawingPens.PenType.GreenPen;
            DrawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.GreenPen);
        }

        private void redDottedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            DrawArea.PenType = DrawingPens.PenType.RedDottedPen;
            DrawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.RedDottedPen);
        }

        private void redDotDashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            DrawArea.PenType = DrawingPens.PenType.RedDotDashPen;
            DrawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.RedDotDashPen);
        }

        private void doubleLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            DrawArea.PenType = DrawingPens.PenType.DoubleLinePen;
            DrawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.DoubleLinePen);
        }

        private void dashedArrowLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            DrawArea.PenType = DrawingPens.PenType.DashedArrowPen;
            DrawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.DashedArrowPen);
        }

        private void toolStripButtonUndo_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            CommandUndo();
        }

        private void toolStripButtonRedo_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            CommandRedo();
        }

        private void tsbZoomIn_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            AdjustZoom(.1f);
        }

        private void tsbZoomOut_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            AdjustZoom(-.1f);
        }

        private void tsbZoomReset_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            DrawArea.Zoom = 1.0f;
            DrawArea.Invalidate();
        }

        private void tsbRotateLeft_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            int al = DrawArea.TheLayers.ActiveLayerIndex;
            if (DrawArea.TheLayers[al].Graphics.SelectionCount > 0)
                RotateObject(10);
        }

        private void tsbRotateRight_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            int al = DrawArea.TheLayers.ActiveLayerIndex;
            if (DrawArea.TheLayers[al].Graphics.SelectionCount > 0)
                RotateObject(-10);
        }

        private void tsbRotateReset_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            int al = DrawArea.TheLayers.ActiveLayerIndex;
            if (DrawArea.TheLayers[al].Graphics.SelectionCount > 0)
                RotateObject(0);
        }

        private void tsbPanMode_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            _panMode = !_panMode;
            if (_panMode)
                tsbPanMode.Checked = true;
            else
                tsbPanMode.Checked = false;
            DrawArea.ActiveTool = DrawArea.DrawToolType.Pointer;
            DrawArea.Panning = _panMode;
        }

        private void tsbPanReset_Click(object sender, EventArgs e)
        {
            if (!CanControl) {
                CommandPointer();
                return;
            }

            _panMode = false;
            if (tsbPanMode.Checked)
                tsbPanMode.Checked = false;
            DrawArea.Panning = false;
            DrawArea.PanX = 0;
            DrawArea.PanY = DrawArea.OriginalPanY;
            DrawArea.Invalidate();
        }
        #endregion

        #region Other Functions
        internal void SetStateOfControls()
        {
            if (DrawArea == null)
                return;

            // Select active tool
            toolStripButtonPointer.Checked = (DrawArea.ActiveTool == DrawArea.DrawToolType.Pointer);
            toolStripButtonPoint.Checked = (DrawArea.ActiveTool == DrawArea.DrawToolType.Point);
            toolStripButtonRectangle.Checked = (DrawArea.ActiveTool == DrawArea.DrawToolType.Rectangle);
            toolStripButtonEllipse.Checked = (DrawArea.ActiveTool == DrawArea.DrawToolType.Ellipse);
            toolStripButtonLine.Checked = (DrawArea.ActiveTool == DrawArea.DrawToolType.Line);
            tsbPolygon.Checked = (DrawArea.ActiveTool == DrawArea.DrawToolType.Polygon);

            int x = DrawArea.TheLayers.ActiveLayerIndex;
            bool selectedObjects = (DrawArea.TheLayers[x].Graphics.SelectionCount > 0);

            // Undo, Redo
            toolStripButtonUndo.Enabled = DrawArea.CanUndo;
            toolStripButtonRedo.Enabled = DrawArea.CanRedo;

            // Pan button
            tsbPanMode.Checked = DrawArea.Panning;
        }

        /// <summary>
        /// Set Pointer draw tool
        /// </summary>
        private void CommandPointer()
        {
            DrawArea.ActiveTool = DrawArea.DrawToolType.Pointer;
        }

        /// <summary>
        /// Set Point draw tool
        /// </summary>
        private void CommandPoint()
        {
            DrawArea.ActiveTool = DrawArea.DrawToolType.Point;
            DrawArea.DrawFilled = false;
        }

        /// <summary>
        /// Set Rectangle draw tool
        /// </summary>
        private void CommandRectangle()
        {
            DrawArea.ActiveTool = DrawArea.DrawToolType.Rectangle;
            DrawArea.DrawFilled = false;
        }

        /// <summary>
        /// Set Ellipse draw tool
        /// </summary>
        private void CommandEllipse()
        {
            DrawArea.ActiveTool = DrawArea.DrawToolType.Ellipse;
            DrawArea.DrawFilled = false;
        }

        /// <summary>
        /// Set Line draw tool
        /// </summary>
        private void CommandLine()
        {
            DrawArea.ActiveTool = DrawArea.DrawToolType.Line;
        }

        /// <summary>
        /// Set Polygon draw tool
        /// </summary>
        private void CommandPolygon()
        {
            DrawArea.ActiveTool = DrawArea.DrawToolType.Polygon;
        }

        /// <summary>
        /// Delete selected items
        /// </summary>
        private void DeleteSelection()
        {
            DrawArea.DeleteSelection();
        }

        /// <summary>
        /// Undo
        /// </summary>
        private void CommandUndo()
        {
            DrawArea.Undo();
        }

        /// <summary>
        /// Redo
        /// </summary>
        private void CommandRedo()
        {
            DrawArea.Redo();
        }


        /// <summary>
        /// Adjust the zoom by the amount given, within reason
        /// </summary>
        /// <param name="_amount">float value to adjust zoom by - may be positive or negative</param>
        private void AdjustZoom(float _amount)
        {
            DrawArea.Zoom += _amount;
            if (DrawArea.Zoom < .1f)
                DrawArea.Zoom = .1f;
            if (DrawArea.Zoom > 10)
                DrawArea.Zoom = 10f;

            DrawArea.Invalidate();
            SetStateOfControls();
        }

        /// <summary>
        /// Rotate the selected Object(s)
        /// </summary>
        /// <param name="p">Amount to rotate. Negative is Left, Positive is Right, Zero indicates Reset to zero</param>
        private void RotateObject(int p)
        {
            int al = DrawArea.TheLayers.ActiveLayerIndex;
            for (int i = 0; i < DrawArea.TheLayers[al].Graphics.Count; i++) {
                if (DrawArea.TheLayers[al].Graphics[i].Selected)
                    if (p == 0)
                        DrawArea.TheLayers[al].Graphics[i].Rotation = 0;
                    else
                        DrawArea.TheLayers[al].Graphics[i].Rotation += p;
            }

            DrawArea.Invalidate();
            SetStateOfControls();
        }
        #endregion
    }
}
