namespace IRApplication.UI
{
    partial class SearchVideoForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchVideoForm));
            this.checkAllBtn = new System.Windows.Forms.Button();
            this.delBtn = new System.Windows.Forms.Button();
            this.next_Page_But = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.pageIndexLab = new System.Windows.Forms.Label();
            this.totalPageLab = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.editBtnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.last_Page_But = new System.Windows.Forms.Button();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.editBtn = new System.Windows.Forms.Button();
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lowerPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.lowerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkAllBtn
            // 
            this.checkAllBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(81)))), ((int)(((byte)(82)))));
            this.checkAllBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkAllBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkAllBtn.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkAllBtn.ForeColor = System.Drawing.Color.White;
            this.checkAllBtn.Location = new System.Drawing.Point(4, 4);
            this.checkAllBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkAllBtn.Name = "checkAllBtn";
            this.checkAllBtn.Size = new System.Drawing.Size(884, 61);
            this.checkAllBtn.TabIndex = 0;
            this.checkAllBtn.Text = "全  选";
            this.checkAllBtn.UseVisualStyleBackColor = false;
            this.checkAllBtn.Click += new System.EventHandler(this.checkAllBtn_Click);
            // 
            // delBtn
            // 
            this.delBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.delBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.delBtn.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.delBtn.Location = new System.Drawing.Point(896, 4);
            this.delBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.delBtn.Name = "delBtn";
            this.delBtn.Size = new System.Drawing.Size(885, 61);
            this.delBtn.TabIndex = 1;
            this.delBtn.Text = "删  除";
            this.delBtn.UseVisualStyleBackColor = true;
            this.delBtn.Click += new System.EventHandler(this.delBtn_Click);
            // 
            // next_Page_But
            // 
            this.next_Page_But.BackColor = System.Drawing.Color.Silver;
            this.next_Page_But.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.next_Page_But.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.next_Page_But.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.next_Page_But.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.next_Page_But.Location = new System.Drawing.Point(1127, 21);
            this.next_Page_But.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.next_Page_But.Name = "next_Page_But";
            this.next_Page_But.Size = new System.Drawing.Size(160, 45);
            this.next_Page_But.TabIndex = 23;
            this.next_Page_But.Text = "下一页";
            this.next_Page_But.UseVisualStyleBackColor = false;
            this.next_Page_But.Click += new System.EventHandler(this.next_Page_But_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("宋体", 14F);
            this.label15.Location = new System.Drawing.Point(1645, 31);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(34, 24);
            this.label15.TabIndex = 22;
            this.label15.Text = "页";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("宋体", 14F);
            this.label14.Location = new System.Drawing.Point(1446, 31);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(82, 24);
            this.label14.TabIndex = 19;
            this.label14.Text = "页  共";
            // 
            // pageIndexLab
            // 
            this.pageIndexLab.AutoSize = true;
            this.pageIndexLab.Font = new System.Drawing.Font("宋体", 14F);
            this.pageIndexLab.Location = new System.Drawing.Point(1387, 31);
            this.pageIndexLab.Name = "pageIndexLab";
            this.pageIndexLab.Size = new System.Drawing.Size(22, 24);
            this.pageIndexLab.TabIndex = 20;
            this.pageIndexLab.Text = "1";
            // 
            // totalPageLab
            // 
            this.totalPageLab.AutoSize = true;
            this.totalPageLab.Font = new System.Drawing.Font("宋体", 14F);
            this.totalPageLab.Location = new System.Drawing.Point(1577, 31);
            this.totalPageLab.Name = "totalPageLab";
            this.totalPageLab.Size = new System.Drawing.Size(22, 24);
            this.totalPageLab.TabIndex = 21;
            this.totalPageLab.Text = "1";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("宋体", 14F);
            this.label12.Location = new System.Drawing.Point(1319, 31);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(34, 24);
            this.label12.TabIndex = 21;
            this.label12.Text = "第";
            // 
            // editBtnCancel
            // 
            this.editBtnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.editBtnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.editBtnCancel.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.editBtnCancel.ForeColor = System.Drawing.Color.Black;
            this.editBtnCancel.Location = new System.Drawing.Point(689, 21);
            this.editBtnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.editBtnCancel.Name = "editBtnCancel";
            this.editBtnCancel.Size = new System.Drawing.Size(160, 45);
            this.editBtnCancel.TabIndex = 18;
            this.editBtnCancel.Text = "取    消";
            this.editBtnCancel.UseVisualStyleBackColor = false;
            this.editBtnCancel.Visible = false;
            this.editBtnCancel.Click += new System.EventHandler(this.editBtnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(286, 31);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 24);
            this.label1.TabIndex = 17;
            this.label1.Text = "至";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(34, 31);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 24);
            this.label2.TabIndex = 2;
            this.label2.Text = "时间:";
            // 
            // last_Page_But
            // 
            this.last_Page_But.BackColor = System.Drawing.Color.Silver;
            this.last_Page_But.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.last_Page_But.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.last_Page_But.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.last_Page_But.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.last_Page_But.Location = new System.Drawing.Point(959, 21);
            this.last_Page_But.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.last_Page_But.Name = "last_Page_But";
            this.last_Page_But.Size = new System.Drawing.Size(160, 45);
            this.last_Page_But.TabIndex = 9;
            this.last_Page_But.Text = "上一页";
            this.last_Page_But.UseVisualStyleBackColor = false;
            this.last_Page_But.Click += new System.EventHandler(this.last_Page_But_Click);
            // 
            // buttonSearch
            // 
            this.buttonSearch.BackColor = System.Drawing.SystemColors.Control;
            this.buttonSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSearch.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonSearch.ForeColor = System.Drawing.Color.Black;
            this.buttonSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonSearch.Location = new System.Drawing.Point(517, 21);
            this.buttonSearch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(160, 45);
            this.buttonSearch.TabIndex = 9;
            this.buttonSearch.Text = "搜    索";
            this.buttonSearch.UseVisualStyleBackColor = false;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.editBtn);
            this.panel3.Controls.Add(this.dateTimePickerStart);
            this.panel3.Controls.Add(this.next_Page_But);
            this.panel3.Controls.Add(this.dateTimePickerEnd);
            this.panel3.Controls.Add(this.label15);
            this.panel3.Controls.Add(this.label14);
            this.panel3.Controls.Add(this.pageIndexLab);
            this.panel3.Controls.Add(this.totalPageLab);
            this.panel3.Controls.Add(this.label12);
            this.panel3.Controls.Add(this.editBtnCancel);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.last_Page_But);
            this.panel3.Controls.Add(this.buttonSearch);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1785, 95);
            this.panel3.TabIndex = 17;
            // 
            // editBtn
            // 
            this.editBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.editBtn.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.editBtn.Image = ((System.Drawing.Image)(resources.GetObject("editBtn.Image")));
            this.editBtn.Location = new System.Drawing.Point(878, 21);
            this.editBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.editBtn.Name = "editBtn";
            this.editBtn.Size = new System.Drawing.Size(53, 44);
            this.editBtn.TabIndex = 16;
            this.editBtn.UseVisualStyleBackColor = true;
            this.editBtn.Click += new System.EventHandler(this.editBtn_Click);
            // 
            // dateTimePickerStart
            // 
            this.dateTimePickerStart.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dateTimePickerStart.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dateTimePickerStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerStart.Location = new System.Drawing.Point(109, 26);
            this.dateTimePickerStart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.Size = new System.Drawing.Size(164, 35);
            this.dateTimePickerStart.TabIndex = 19;
            // 
            // dateTimePickerEnd
            // 
            this.dateTimePickerEnd.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dateTimePickerEnd.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dateTimePickerEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerEnd.Location = new System.Drawing.Point(331, 26);
            this.dateTimePickerEnd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dateTimePickerEnd.Name = "dateTimePickerEnd";
            this.dateTimePickerEnd.Size = new System.Drawing.Size(164, 35);
            this.dateTimePickerEnd.TabIndex = 20;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 59.93052F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.09265F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel2.Controls.Add(this.pictureTableLayoutPanel, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.panel2, 2, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 95);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1785, 701);
            this.tableLayoutPanel2.TabIndex = 23;
            // 
            // pictureTableLayoutPanel
            // 
            this.pictureTableLayoutPanel.AutoScroll = true;
            this.pictureTableLayoutPanel.ColumnCount = 4;
            this.pictureTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pictureTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pictureTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pictureTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.pictureTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureTableLayoutPanel.Location = new System.Drawing.Point(27, 64);
            this.pictureTableLayoutPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureTableLayoutPanel.Name = "pictureTableLayoutPanel";
            this.pictureTableLayoutPanel.RowCount = 2;
            this.pictureTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pictureTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pictureTableLayoutPanel.Size = new System.Drawing.Size(1029, 635);
            this.pictureTableLayoutPanel.TabIndex = 15;
            // 
            // panel2
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.panel2, 2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(1064, 67);
            this.panel2.Margin = new System.Windows.Forms.Padding(5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(682, 629);
            this.panel2.TabIndex = 16;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel2);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1785, 796);
            this.panel1.TabIndex = 27;
            // 
            // lowerPanel
            // 
            this.lowerPanel.ColumnCount = 2;
            this.lowerPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.lowerPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.lowerPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.lowerPanel.Controls.Add(this.checkAllBtn, 0, 0);
            this.lowerPanel.Controls.Add(this.delBtn, 1, 0);
            this.lowerPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lowerPanel.Location = new System.Drawing.Point(0, 796);
            this.lowerPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lowerPanel.Name = "lowerPanel";
            this.lowerPanel.RowCount = 1;
            this.lowerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.lowerPanel.Size = new System.Drawing.Size(1785, 69);
            this.lowerPanel.TabIndex = 28;
            // 
            // SearchVideoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1785, 865);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lowerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "SearchVideoForm";
            this.Text = "SearchVideoForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.lowerPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button checkAllBtn;
        private System.Windows.Forms.Button delBtn;
        private System.Windows.Forms.Button editBtn;
        private System.Windows.Forms.Button next_Page_But;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label pageIndexLab;
        private System.Windows.Forms.Label totalPageLab;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button editBtnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button last_Page_But;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel lowerPanel;
        private System.Windows.Forms.TableLayoutPanel pictureTableLayoutPanel;
        private System.Windows.Forms.Panel panel2;
    }
}