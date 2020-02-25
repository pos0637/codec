namespace IRApplication.Components
{
    partial class AlarmInformationItem
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.NameLabel1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureEdit = new System.Windows.Forms.PictureBox();
            this.pictureIrEdit = new System.Windows.Forms.PictureBox();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureIrEdit)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.NameLabel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 178);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(395, 37);
            this.panel2.TabIndex = 5;
            // 
            // NameLabel1
            // 
            this.NameLabel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NameLabel1.AutoSize = true;
            this.NameLabel1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.NameLabel1.Location = new System.Drawing.Point(162, 5);
            this.NameLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.NameLabel1.Name = "NameLabel1";
            this.NameLabel1.Size = new System.Drawing.Size(0, 24);
            this.NameLabel1.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(395, 215);
            this.panel1.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.17065F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.82935F));
            this.tableLayoutPanel1.Controls.Add(this.pictureEdit, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureIrEdit, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 81.04575F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(395, 178);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // pictureEdit
            // 
            this.pictureEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureEdit.Location = new System.Drawing.Point(202, 4);
            this.pictureEdit.Margin = new System.Windows.Forms.Padding(4);
            this.pictureEdit.Name = "pictureEdit";
            this.pictureEdit.Size = new System.Drawing.Size(189, 170);
            this.pictureEdit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureEdit.TabIndex = 2;
            this.pictureEdit.TabStop = false;
            // 
            // pictureIrEdit
            // 
            this.pictureIrEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureIrEdit.Location = new System.Drawing.Point(3, 2);
            this.pictureIrEdit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureIrEdit.Name = "pictureIrEdit";
            this.pictureIrEdit.Size = new System.Drawing.Size(192, 174);
            this.pictureIrEdit.TabIndex = 3;
            this.pictureIrEdit.TabStop = false;
            // 
            // AlarmInformationItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "AlarmInformationItem";
            this.Size = new System.Drawing.Size(395, 215);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureIrEdit)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        public System.Windows.Forms.Label NameLabel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public System.Windows.Forms.PictureBox pictureEdit;
        public System.Windows.Forms.PictureBox pictureIrEdit;
    }
}
