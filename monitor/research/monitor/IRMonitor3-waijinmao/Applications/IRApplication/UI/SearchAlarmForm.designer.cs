namespace IRApplication.UI
{
    partial class SearchAlarmForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchAlarmForm));
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.alarmDataGridView = new System.Windows.Forms.DataGridView();
            this.Column5 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.告警Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Image = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonReport = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.alarmPicture = new System.Windows.Forms.PictureBox();
            this.irAlarmPicture = new System.Windows.Forms.PictureBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.editBtn = new System.Windows.Forms.Button();
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
            this.checkAllBtn = new System.Windows.Forms.Button();
            this.lowerPanel = new System.Windows.Forms.TableLayoutPanel();
            this.delBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alarmDataGridView)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alarmPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.irAlarmPicture)).BeginInit();
            this.panel3.SuspendLayout();
            this.lowerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // dateTimePickerStart
            // 
            this.dateTimePickerStart.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dateTimePickerStart.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dateTimePickerStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerStart.Location = new System.Drawing.Point(91, 24);
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
            this.dateTimePickerEnd.Location = new System.Drawing.Point(313, 24);
            this.dateTimePickerEnd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dateTimePickerEnd.Name = "dateTimePickerEnd";
            this.dateTimePickerEnd.Size = new System.Drawing.Size(164, 35);
            this.dateTimePickerEnd.TabIndex = 20;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel2);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1803, 912);
            this.panel1.TabIndex = 25;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 13F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel2.Controls.Add(this.alarmDataGridView, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonReport, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonDown, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 2, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 95);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1803, 817);
            this.tableLayoutPanel2.TabIndex = 23;
            // 
            // alarmDataGridView
            // 
            this.alarmDataGridView.AllowUserToAddRows = false;
            this.alarmDataGridView.AllowUserToResizeColumns = false;
            this.alarmDataGridView.AllowUserToResizeRows = false;
            this.alarmDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.alarmDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.alarmDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.alarmDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.alarmDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column5,
            this.告警Id,
            this.Column1,
            this.Column2,
            this.Column4,
            this.Column6,
            this.Image});
            this.alarmDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alarmDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
            this.alarmDataGridView.EnableHeadersVisualStyles = false;
            this.alarmDataGridView.Location = new System.Drawing.Point(17, 4);
            this.alarmDataGridView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.alarmDataGridView.MultiSelect = false;
            this.alarmDataGridView.Name = "alarmDataGridView";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.alarmDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.alarmDataGridView.RowHeadersVisible = false;
            this.alarmDataGridView.RowHeadersWidth = 51;
            this.tableLayoutPanel2.SetRowSpan(this.alarmDataGridView, 2);
            this.alarmDataGridView.RowTemplate.Height = 23;
            this.alarmDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.alarmDataGridView.Size = new System.Drawing.Size(1048, 809);
            this.alarmDataGridView.TabIndex = 13;
            this.alarmDataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.alarmDataGridView_CellClick);
            this.alarmDataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.alarmDataGridView_CellDoubleClick);
            this.alarmDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.alarmDataGridView_CellClick);
            this.alarmDataGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this.alarmDataGridView_CurrentCellDirtyStateChanged);
            // 
            // Column5
            // 
            this.Column5.HeaderText = "";
            this.Column5.MinimumWidth = 6;
            this.Column5.Name = "Column5";
            this.Column5.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column5.Visible = false;
            this.Column5.Width = 30;
            // 
            // 告警Id
            // 
            this.告警Id.HeaderText = "Id";
            this.告警Id.MinimumWidth = 6;
            this.告警Id.Name = "告警Id";
            this.告警Id.Visible = false;
            this.告警Id.Width = 20;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column1.DefaultCellStyle = dataGridViewCellStyle2;
            this.Column1.FillWeight = 58.47717F;
            this.Column1.HeaderText = "设备名称";
            this.Column1.MinimumWidth = 6;
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column2.DefaultCellStyle = dataGridViewCellStyle3;
            this.Column2.FillWeight = 66.51405F;
            this.Column2.HeaderText = "开始时间";
            this.Column2.MinimumWidth = 6;
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column4.DefaultCellStyle = dataGridViewCellStyle4;
            this.Column4.FillWeight = 170.4173F;
            this.Column4.HeaderText = "告警原因";
            this.Column4.MinimumWidth = 6;
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            // 
            // Column6
            // 
            this.Column6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column6.HeaderText = "处理意见";
            this.Column6.MinimumWidth = 6;
            this.Column6.Name = "Column6";
            // 
            // Image
            // 
            this.Image.HeaderText = "Image";
            this.Image.MinimumWidth = 6;
            this.Image.Name = "Image";
            this.Image.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Image.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Image.Visible = false;
            this.Image.Width = 125;
            // 
            // buttonReport
            // 
            this.buttonReport.BackColor = System.Drawing.SystemColors.Control;
            this.buttonReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonReport.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonReport.ForeColor = System.Drawing.Color.Black;
            this.buttonReport.Location = new System.Drawing.Point(1425, 4);
            this.buttonReport.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonReport.Name = "buttonReport";
            this.buttonReport.Size = new System.Drawing.Size(344, 54);
            this.buttonReport.TabIndex = 11;
            this.buttonReport.Text = "生成报表";
            this.buttonReport.UseVisualStyleBackColor = false;
            this.buttonReport.Click += new System.EventHandler(this.buttonReport_Click);
            // 
            // buttonDown
            // 
            this.buttonDown.BackColor = System.Drawing.SystemColors.Control;
            this.buttonDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonDown.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonDown.ForeColor = System.Drawing.Color.Black;
            this.buttonDown.Location = new System.Drawing.Point(1073, 4);
            this.buttonDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(344, 54);
            this.buttonDown.TabIndex = 12;
            this.buttonDown.Text = "下载图片";
            this.buttonDown.UseVisualStyleBackColor = false;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel2.SetColumnSpan(this.tableLayoutPanel1, 2);
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.alarmPicture, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.irAlarmPicture, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(1072, 64);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(698, 751);
            this.tableLayoutPanel1.TabIndex = 14;
            // 
            // alarmPicture
            // 
            this.alarmPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alarmPicture.Location = new System.Drawing.Point(3, 2);
            this.alarmPicture.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.alarmPicture.Name = "alarmPicture";
            this.alarmPicture.Size = new System.Drawing.Size(692, 371);
            this.alarmPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.alarmPicture.TabIndex = 0;
            this.alarmPicture.TabStop = false;
            // 
            // irAlarmPicture
            // 
            this.irAlarmPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.irAlarmPicture.Location = new System.Drawing.Point(3, 377);
            this.irAlarmPicture.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.irAlarmPicture.Name = "irAlarmPicture";
            this.irAlarmPicture.Size = new System.Drawing.Size(692, 372);
            this.irAlarmPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.irAlarmPicture.TabIndex = 1;
            this.irAlarmPicture.TabStop = false;
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
            this.panel3.Size = new System.Drawing.Size(1803, 95);
            this.panel3.TabIndex = 17;
            // 
            // editBtn
            // 
            this.editBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.editBtn.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.editBtn.Image = ((System.Drawing.Image)(resources.GetObject("editBtn.Image")));
            this.editBtn.Location = new System.Drawing.Point(860, 21);
            this.editBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.editBtn.Name = "editBtn";
            this.editBtn.Size = new System.Drawing.Size(53, 44);
            this.editBtn.TabIndex = 16;
            this.editBtn.UseVisualStyleBackColor = true;
            this.editBtn.Click += new System.EventHandler(this.editBtn_Click);
            // 
            // next_Page_But
            // 
            this.next_Page_But.BackColor = System.Drawing.Color.Silver;
            this.next_Page_But.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.next_Page_But.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.next_Page_But.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.next_Page_But.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.next_Page_But.Location = new System.Drawing.Point(1109, 21);
            this.next_Page_But.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.next_Page_But.Name = "next_Page_But";
            this.next_Page_But.Size = new System.Drawing.Size(160, 45);
            this.next_Page_But.TabIndex = 23;
            this.next_Page_But.Text = "下一页";
            this.next_Page_But.UseVisualStyleBackColor = false;
            this.next_Page_But.Click += new System.EventHandler(this.next_Page_But_Click_1);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("宋体", 14F);
            this.label15.Location = new System.Drawing.Point(1627, 32);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(34, 24);
            this.label15.TabIndex = 22;
            this.label15.Text = "页";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("宋体", 14F);
            this.label14.Location = new System.Drawing.Point(1428, 32);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(82, 24);
            this.label14.TabIndex = 19;
            this.label14.Text = "页  共";
            // 
            // pageIndexLab
            // 
            this.pageIndexLab.AutoSize = true;
            this.pageIndexLab.Font = new System.Drawing.Font("宋体", 14F);
            this.pageIndexLab.Location = new System.Drawing.Point(1369, 32);
            this.pageIndexLab.Name = "pageIndexLab";
            this.pageIndexLab.Size = new System.Drawing.Size(22, 24);
            this.pageIndexLab.TabIndex = 20;
            this.pageIndexLab.Text = "1";
            // 
            // totalPageLab
            // 
            this.totalPageLab.AutoSize = true;
            this.totalPageLab.Font = new System.Drawing.Font("宋体", 14F);
            this.totalPageLab.Location = new System.Drawing.Point(1559, 32);
            this.totalPageLab.Name = "totalPageLab";
            this.totalPageLab.Size = new System.Drawing.Size(22, 24);
            this.totalPageLab.TabIndex = 21;
            this.totalPageLab.Text = "1";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("宋体", 14F);
            this.label12.Location = new System.Drawing.Point(1301, 32);
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
            this.editBtnCancel.Location = new System.Drawing.Point(671, 21);
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
            this.label1.Location = new System.Drawing.Point(268, 32);
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
            this.label2.Location = new System.Drawing.Point(16, 30);
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
            this.last_Page_But.Location = new System.Drawing.Point(941, 21);
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
            this.buttonSearch.Location = new System.Drawing.Point(499, 21);
            this.buttonSearch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(160, 45);
            this.buttonSearch.TabIndex = 9;
            this.buttonSearch.Text = "搜    索";
            this.buttonSearch.UseVisualStyleBackColor = false;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
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
            this.checkAllBtn.Size = new System.Drawing.Size(893, 61);
            this.checkAllBtn.TabIndex = 0;
            this.checkAllBtn.Text = "全  选";
            this.checkAllBtn.UseVisualStyleBackColor = false;
            this.checkAllBtn.Click += new System.EventHandler(this.checkAllBtn_Click);
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
            this.lowerPanel.Location = new System.Drawing.Point(0, 843);
            this.lowerPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lowerPanel.Name = "lowerPanel";
            this.lowerPanel.RowCount = 1;
            this.lowerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.lowerPanel.Size = new System.Drawing.Size(1803, 69);
            this.lowerPanel.TabIndex = 26;
            // 
            // delBtn
            // 
            this.delBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.delBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.delBtn.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.delBtn.Location = new System.Drawing.Point(905, 4);
            this.delBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.delBtn.Name = "delBtn";
            this.delBtn.Size = new System.Drawing.Size(894, 61);
            this.delBtn.TabIndex = 1;
            this.delBtn.Text = "删  除";
            this.delBtn.UseVisualStyleBackColor = true;
            this.delBtn.Click += new System.EventHandler(this.delBtn_Click);
            // 
            // SearchAlarmForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1803, 912);
            this.Controls.Add(this.lowerPanel);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "SearchAlarmForm";
            this.Text = "SearchAlarmForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.SearchAlarmForm_Load);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.alarmDataGridView)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.alarmPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.irAlarmPicture)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.lowerPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button editBtn;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label pageIndexLab;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button last_Page_But;
        private System.Windows.Forms.Button checkAllBtn;
        private System.Windows.Forms.TableLayoutPanel lowerPanel;
        private System.Windows.Forms.Label totalPageLab;
        private System.Windows.Forms.Button next_Page_But;
        private System.Windows.Forms.Button editBtnCancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.DataGridView alarmDataGridView;
        private System.Windows.Forms.Button buttonReport;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox alarmPicture;
        private System.Windows.Forms.Button delBtn;
        private System.Windows.Forms.PictureBox irAlarmPicture;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn 告警Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Image;
    }
}