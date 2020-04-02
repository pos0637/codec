namespace DrawTools2
{
    partial class DrawToolsControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DrawToolsControl));
            this.panel1 = new System.Windows.Forms.Panel();
            this.drawToolbar1 = new DrawTools2.DrawToolbar();
            this.panel2 = new System.Windows.Forms.Panel();
            this.drawArea1 = new DrawTools2.DrawArea();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.drawToolbar1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(761, 56);
            this.panel1.TabIndex = 0;
            // 
            // drawToolbar1
            // 
            this.drawToolbar1.CanControl = false;
            this.drawToolbar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.drawToolbar1.DrawArea = null;
            this.drawToolbar1.EllipseEnabled = false;
            this.drawToolbar1.EllipseVisible = true;
            this.drawToolbar1.LineColorVisible = true;
            this.drawToolbar1.LineEnabled = false;
            this.drawToolbar1.LineSizeVisible = true;
            this.drawToolbar1.LineVisible = true;
            this.drawToolbar1.Location = new System.Drawing.Point(0, 0);
            this.drawToolbar1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.drawToolbar1.Name = "drawToolbar1";
            this.drawToolbar1.PanVisible = true;
            this.drawToolbar1.PenStyleVisible = true;
            this.drawToolbar1.PointEnabled = false;
            this.drawToolbar1.PointVisible = true;
            this.drawToolbar1.PolygonEnabled = false;
            this.drawToolbar1.PolygonVisible = true;
            this.drawToolbar1.RectangleEnabled = false;
            this.drawToolbar1.RectangleVisible = true;
            this.drawToolbar1.RotateVisible = true;
            this.drawToolbar1.Size = new System.Drawing.Size(761, 56);
            this.drawToolbar1.TabIndex = 0;
            this.drawToolbar1.UndoVisible = true;
            this.drawToolbar1.ZoomVisible = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.drawArea1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 56);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(761, 385);
            this.panel2.TabIndex = 1;
            // 
            // drawArea1
            // 
            this.drawArea1.ActiveTool = DrawTools2.DrawArea.DrawToolType.Pointer;
            this.drawArea1.BackColor = System.Drawing.Color.White;
            this.drawArea1.BackgroundRenderEnabled = false;
            this.drawArea1.CanControl = false;
            this.drawArea1.CurrentPen = null;
            this.drawArea1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.drawArea1.DrawFilled = false;
            this.drawArea1.DrawNetRectangle = false;
            this.drawArea1.FillColor = System.Drawing.Color.White;
            this.drawArea1.LineColor = System.Drawing.Color.Black;
            this.drawArea1.LineWidth = -1;
            this.drawArea1.Location = new System.Drawing.Point(0, 0);
            this.drawArea1.Name = "drawArea1";
            this.drawArea1.NetRectangle = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.drawArea1.OriginalPanY = 0;
            this.drawArea1.Panning = false;
            this.drawArea1.PanX = 0;
            this.drawArea1.PanY = 0;
            this.drawArea1.PenType = DrawTools2.DrawingPens.PenType.Generic;
            this.drawArea1.Size = new System.Drawing.Size(761, 385);
            this.drawArea1.TabIndex = 0;
            this.drawArea1.TheLayers = ((DrawTools2.Layers)(resources.GetObject("drawArea1.TheLayers")));
            this.drawArea1.Zoom = 1F;
            // 
            // DrawToolsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "DrawToolsControl";
            this.Size = new System.Drawing.Size(761, 441);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private DrawToolbar drawToolbar1;
        private System.Windows.Forms.Panel panel2;
        private DrawArea drawArea1;
    }
}
