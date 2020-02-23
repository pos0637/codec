namespace IRApplication.UI
{
    partial class HomeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HomeForm));
            this.tableLayoutPanelBottom = new System.Windows.Forms.TableLayoutPanel();
            this.buttonCloseClient = new System.Windows.Forms.Button();
            this.buttonSidebarDownload = new System.Windows.Forms.Button();
            this.buttonSidebarShutdown = new System.Windows.Forms.Button();
            this.panelSidebarBottom = new System.Windows.Forms.Panel();
            this.tableLayoutPanelSidebar = new System.Windows.Forms.TableLayoutPanel();
            this.buttonSidebarHome = new System.Windows.Forms.Button();
            this.buttonSidebarConfig = new System.Windows.Forms.Button();
            this.buttonSidebarRealtime = new System.Windows.Forms.Button();
            this.buttonSidebarSecondaryAnalysis = new System.Windows.Forms.Button();
            this.buttonSidebarPlaybackVideo = new System.Windows.Forms.Button();
            this.buttonSidebarAlarmSearch = new System.Windows.Forms.Button();
            this.buttonCloseSidebar = new System.Windows.Forms.Button();
            this.panelSidebar = new System.Windows.Forms.Panel();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.buttonRecord = new System.Windows.Forms.Button();
            this.buttonSecondaryAnalysis = new System.Windows.Forms.Button();
            this.buttonConfig = new System.Windows.Forms.Button();
            this.buttonAlarm = new System.Windows.Forms.Button();
            this.buttonRealtime = new System.Windows.Forms.Button();
            this.panelMain = new System.Windows.Forms.Panel();
            this.sidebarBtn = new System.Windows.Forms.Button();
            this.buttonReturn = new System.Windows.Forms.Button();
            this.titleLabel = new System.Windows.Forms.Label();
            this.panelTopButton = new System.Windows.Forms.Panel();
            this.tableLayoutPanelTittle = new System.Windows.Forms.TableLayoutPanel();
            this.panelTittle = new System.Windows.Forms.Panel();
            this.tableLayoutPanelBottom.SuspendLayout();
            this.panelSidebarBottom.SuspendLayout();
            this.tableLayoutPanelSidebar.SuspendLayout();
            this.panelSidebar.SuspendLayout();
            this.tableLayoutPanelMain.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.panelTopButton.SuspendLayout();
            this.tableLayoutPanelTittle.SuspendLayout();
            this.panelTittle.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelBottom
            // 
            this.tableLayoutPanelBottom.ColumnCount = 3;
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.tableLayoutPanelBottom.Controls.Add(this.buttonCloseClient, 0, 0);
            this.tableLayoutPanelBottom.Controls.Add(this.buttonSidebarDownload, 2, 0);
            this.tableLayoutPanelBottom.Controls.Add(this.buttonSidebarShutdown, 1, 0);
            this.tableLayoutPanelBottom.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelBottom.Name = "tableLayoutPanelBottom";
            this.tableLayoutPanelBottom.RowCount = 1;
            this.tableLayoutPanelBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelBottom.Size = new System.Drawing.Size(217, 53);
            this.tableLayoutPanelBottom.TabIndex = 0;
            // 
            // buttonCloseClient
            // 
            this.buttonCloseClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonCloseClient.FlatAppearance.BorderSize = 0;
            this.buttonCloseClient.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCloseClient.ForeColor = System.Drawing.Color.Transparent;
            this.buttonCloseClient.Image = global::IRApplication.Properties.Resources.SidebarCloseClient;
            this.buttonCloseClient.Location = new System.Drawing.Point(3, 3);
            this.buttonCloseClient.Name = "buttonCloseClient";
            this.buttonCloseClient.Size = new System.Drawing.Size(65, 47);
            this.buttonCloseClient.TabIndex = 1;
            this.buttonCloseClient.UseVisualStyleBackColor = true;
            this.buttonCloseClient.Click += new System.EventHandler(this.buttonCloseClient_Click);
            // 
            // buttonSidebarDownload
            // 
            this.buttonSidebarDownload.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonSidebarDownload.FlatAppearance.BorderSize = 0;
            this.buttonSidebarDownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSidebarDownload.ForeColor = System.Drawing.Color.Transparent;
            this.buttonSidebarDownload.Image = ((System.Drawing.Image)(resources.GetObject("buttonSidebarDownload.Image")));
            this.buttonSidebarDownload.Location = new System.Drawing.Point(145, 3);
            this.buttonSidebarDownload.Name = "buttonSidebarDownload";
            this.buttonSidebarDownload.Size = new System.Drawing.Size(69, 47);
            this.buttonSidebarDownload.TabIndex = 0;
            this.buttonSidebarDownload.UseVisualStyleBackColor = true;
            // 
            // buttonSidebarShutdown
            // 
            this.buttonSidebarShutdown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonSidebarShutdown.FlatAppearance.BorderSize = 0;
            this.buttonSidebarShutdown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSidebarShutdown.ForeColor = System.Drawing.Color.Transparent;
            this.buttonSidebarShutdown.Image = ((System.Drawing.Image)(resources.GetObject("buttonSidebarShutdown.Image")));
            this.buttonSidebarShutdown.Location = new System.Drawing.Point(74, 3);
            this.buttonSidebarShutdown.Name = "buttonSidebarShutdown";
            this.buttonSidebarShutdown.Size = new System.Drawing.Size(65, 47);
            this.buttonSidebarShutdown.TabIndex = 0;
            this.buttonSidebarShutdown.UseVisualStyleBackColor = true;
            // 
            // panelSidebarBottom
            // 
            this.panelSidebarBottom.Controls.Add(this.tableLayoutPanelBottom);
            this.panelSidebarBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSidebarBottom.Location = new System.Drawing.Point(3, 519);
            this.panelSidebarBottom.Name = "panelSidebarBottom";
            this.panelSidebarBottom.Size = new System.Drawing.Size(217, 53);
            this.panelSidebarBottom.TabIndex = 3;
            // 
            // tableLayoutPanelSidebar
            // 
            this.tableLayoutPanelSidebar.ColumnCount = 1;
            this.tableLayoutPanelSidebar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelSidebar.Controls.Add(this.buttonSidebarHome, 0, 1);
            this.tableLayoutPanelSidebar.Controls.Add(this.buttonSidebarConfig, 0, 6);
            this.tableLayoutPanelSidebar.Controls.Add(this.buttonSidebarRealtime, 0, 2);
            this.tableLayoutPanelSidebar.Controls.Add(this.buttonSidebarSecondaryAnalysis, 0, 5);
            this.tableLayoutPanelSidebar.Controls.Add(this.buttonSidebarPlaybackVideo, 0, 3);
            this.tableLayoutPanelSidebar.Controls.Add(this.buttonSidebarAlarmSearch, 0, 4);
            this.tableLayoutPanelSidebar.Controls.Add(this.panelSidebarBottom, 0, 8);
            this.tableLayoutPanelSidebar.Controls.Add(this.buttonCloseSidebar, 0, 0);
            this.tableLayoutPanelSidebar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelSidebar.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelSidebar.Name = "tableLayoutPanelSidebar";
            this.tableLayoutPanelSidebar.RowCount = 9;
            this.tableLayoutPanelSidebar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanelSidebar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.5F));
            this.tableLayoutPanelSidebar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.5F));
            this.tableLayoutPanelSidebar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.5F));
            this.tableLayoutPanelSidebar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.5F));
            this.tableLayoutPanelSidebar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.5F));
            this.tableLayoutPanelSidebar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.5F));
            this.tableLayoutPanelSidebar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 23.5F));
            this.tableLayoutPanelSidebar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.5F));
            this.tableLayoutPanelSidebar.Size = new System.Drawing.Size(223, 575);
            this.tableLayoutPanelSidebar.TabIndex = 4;
            // 
            // buttonSidebarHome
            // 
            this.buttonSidebarHome.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonSidebarHome.FlatAppearance.BorderSize = 0;
            this.buttonSidebarHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSidebarHome.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonSidebarHome.ForeColor = System.Drawing.Color.White;
            this.buttonSidebarHome.Image = ((System.Drawing.Image)(resources.GetObject("buttonSidebarHome.Image")));
            this.buttonSidebarHome.Location = new System.Drawing.Point(3, 60);
            this.buttonSidebarHome.Name = "buttonSidebarHome";
            this.buttonSidebarHome.Size = new System.Drawing.Size(217, 48);
            this.buttonSidebarHome.TabIndex = 2;
            this.buttonSidebarHome.Text = "    首  页";
            this.buttonSidebarHome.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonSidebarHome.UseVisualStyleBackColor = true;
            this.buttonSidebarHome.Click += new System.EventHandler(this.buttonSidebarHome_Click);
            // 
            // buttonSidebarConfig
            // 
            this.buttonSidebarConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonSidebarConfig.FlatAppearance.BorderSize = 0;
            this.buttonSidebarConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSidebarConfig.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonSidebarConfig.ForeColor = System.Drawing.Color.White;
            this.buttonSidebarConfig.Image = ((System.Drawing.Image)(resources.GetObject("buttonSidebarConfig.Image")));
            this.buttonSidebarConfig.Location = new System.Drawing.Point(3, 330);
            this.buttonSidebarConfig.Name = "buttonSidebarConfig";
            this.buttonSidebarConfig.Size = new System.Drawing.Size(217, 48);
            this.buttonSidebarConfig.TabIndex = 0;
            this.buttonSidebarConfig.Text = "    配置界面";
            this.buttonSidebarConfig.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonSidebarConfig.UseVisualStyleBackColor = true;
            this.buttonSidebarConfig.Click += new System.EventHandler(this.buttonSidebarConfig_Click);
            // 
            // buttonSidebarRealtime
            // 
            this.buttonSidebarRealtime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonSidebarRealtime.FlatAppearance.BorderSize = 0;
            this.buttonSidebarRealtime.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSidebarRealtime.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonSidebarRealtime.ForeColor = System.Drawing.Color.White;
            this.buttonSidebarRealtime.Image = ((System.Drawing.Image)(resources.GetObject("buttonSidebarRealtime.Image")));
            this.buttonSidebarRealtime.Location = new System.Drawing.Point(3, 114);
            this.buttonSidebarRealtime.Name = "buttonSidebarRealtime";
            this.buttonSidebarRealtime.Size = new System.Drawing.Size(217, 48);
            this.buttonSidebarRealtime.TabIndex = 0;
            this.buttonSidebarRealtime.Text = "    实  时";
            this.buttonSidebarRealtime.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonSidebarRealtime.UseVisualStyleBackColor = true;
            this.buttonSidebarRealtime.Click += new System.EventHandler(this.buttonSidebarRealtime_Click);
            // 
            // buttonSidebarSecondaryAnalysis
            // 
            this.buttonSidebarSecondaryAnalysis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonSidebarSecondaryAnalysis.FlatAppearance.BorderSize = 0;
            this.buttonSidebarSecondaryAnalysis.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSidebarSecondaryAnalysis.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonSidebarSecondaryAnalysis.ForeColor = System.Drawing.Color.White;
            this.buttonSidebarSecondaryAnalysis.Image = ((System.Drawing.Image)(resources.GetObject("buttonSidebarSecondaryAnalysis.Image")));
            this.buttonSidebarSecondaryAnalysis.Location = new System.Drawing.Point(3, 276);
            this.buttonSidebarSecondaryAnalysis.Name = "buttonSidebarSecondaryAnalysis";
            this.buttonSidebarSecondaryAnalysis.Size = new System.Drawing.Size(217, 48);
            this.buttonSidebarSecondaryAnalysis.TabIndex = 0;
            this.buttonSidebarSecondaryAnalysis.Text = "    数据展示";
            this.buttonSidebarSecondaryAnalysis.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonSidebarSecondaryAnalysis.UseVisualStyleBackColor = true;
            this.buttonSidebarSecondaryAnalysis.Click += new System.EventHandler(this.buttonSidebarSecondaryAnalysis_Click);
            // 
            // buttonSidebarPlaybackVideo
            // 
            this.buttonSidebarPlaybackVideo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonSidebarPlaybackVideo.FlatAppearance.BorderSize = 0;
            this.buttonSidebarPlaybackVideo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSidebarPlaybackVideo.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonSidebarPlaybackVideo.ForeColor = System.Drawing.Color.White;
            this.buttonSidebarPlaybackVideo.Image = global::IRApplication.Properties.Resources.SidebarPlaybackVideo;
            this.buttonSidebarPlaybackVideo.Location = new System.Drawing.Point(3, 168);
            this.buttonSidebarPlaybackVideo.Name = "buttonSidebarPlaybackVideo";
            this.buttonSidebarPlaybackVideo.Size = new System.Drawing.Size(217, 48);
            this.buttonSidebarPlaybackVideo.TabIndex = 0;
            this.buttonSidebarPlaybackVideo.Text = "    回放录像";
            this.buttonSidebarPlaybackVideo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonSidebarPlaybackVideo.UseVisualStyleBackColor = true;
            // 
            // buttonSidebarAlarmSearch
            // 
            this.buttonSidebarAlarmSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonSidebarAlarmSearch.FlatAppearance.BorderSize = 0;
            this.buttonSidebarAlarmSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSidebarAlarmSearch.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonSidebarAlarmSearch.ForeColor = System.Drawing.Color.White;
            this.buttonSidebarAlarmSearch.Image = global::IRApplication.Properties.Resources.SidebarAlarmSearch;
            this.buttonSidebarAlarmSearch.Location = new System.Drawing.Point(3, 222);
            this.buttonSidebarAlarmSearch.Name = "buttonSidebarAlarmSearch";
            this.buttonSidebarAlarmSearch.Size = new System.Drawing.Size(217, 48);
            this.buttonSidebarAlarmSearch.TabIndex = 0;
            this.buttonSidebarAlarmSearch.Text = "    告警查询";
            this.buttonSidebarAlarmSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonSidebarAlarmSearch.UseVisualStyleBackColor = true;
            this.buttonSidebarAlarmSearch.Click += new System.EventHandler(this.buttonSidebarAlarmSearch_Click);
            // 
            // buttonCloseSidebar
            // 
            this.buttonCloseSidebar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonCloseSidebar.FlatAppearance.BorderSize = 0;
            this.buttonCloseSidebar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCloseSidebar.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonCloseSidebar.ForeColor = System.Drawing.Color.White;
            this.buttonCloseSidebar.Image = global::IRApplication.Properties.Resources.LogoSide;
            this.buttonCloseSidebar.Location = new System.Drawing.Point(3, 3);
            this.buttonCloseSidebar.Name = "buttonCloseSidebar";
            this.buttonCloseSidebar.Size = new System.Drawing.Size(217, 51);
            this.buttonCloseSidebar.TabIndex = 1;
            this.buttonCloseSidebar.Text = "   国化智能";
            this.buttonCloseSidebar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonCloseSidebar.UseVisualStyleBackColor = true;
            this.buttonCloseSidebar.Click += new System.EventHandler(this.buttonCloseSidebar_Click);
            // 
            // panelSidebar
            // 
            this.panelSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(34)))), ((int)(((byte)(68)))));
            this.panelSidebar.Controls.Add(this.tableLayoutPanelSidebar);
            this.panelSidebar.Location = new System.Drawing.Point(0, -42);
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Size = new System.Drawing.Size(223, 575);
            this.panelSidebar.TabIndex = 14;
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 5;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 58F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 61F));
            this.tableLayoutPanelMain.Controls.Add(this.buttonRecord, 2, 1);
            this.tableLayoutPanelMain.Controls.Add(this.buttonSecondaryAnalysis, 3, 1);
            this.tableLayoutPanelMain.Controls.Add(this.buttonConfig, 3, 2);
            this.tableLayoutPanelMain.Controls.Add(this.buttonAlarm, 1, 1);
            this.tableLayoutPanelMain.Controls.Add(this.buttonRealtime, 1, 2);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 4;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(1049, 597);
            this.tableLayoutPanelMain.TabIndex = 10;
            // 
            // buttonRecord
            // 
            this.buttonRecord.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(158)))), ((int)(((byte)(234)))));
            this.buttonRecord.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonRecord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonRecord.Enabled = false;
            this.buttonRecord.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonRecord.ForeColor = System.Drawing.Color.White;
            this.buttonRecord.Image = global::IRApplication.Properties.Resources.HomeFormRecord;
            this.buttonRecord.Location = new System.Drawing.Point(371, 39);
            this.buttonRecord.Name = "buttonRecord";
            this.buttonRecord.Size = new System.Drawing.Size(304, 256);
            this.buttonRecord.TabIndex = 1;
            this.buttonRecord.Text = "回放录像";
            this.buttonRecord.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.buttonRecord.UseVisualStyleBackColor = false;
            // 
            // buttonSecondaryAnalysis
            // 
            this.buttonSecondaryAnalysis.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(138)))), ((int)(((byte)(137)))));
            this.buttonSecondaryAnalysis.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonSecondaryAnalysis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonSecondaryAnalysis.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonSecondaryAnalysis.ForeColor = System.Drawing.Color.White;
            this.buttonSecondaryAnalysis.Image = global::IRApplication.Properties.Resources.HomeFormSecondaryAnalysis;
            this.buttonSecondaryAnalysis.Location = new System.Drawing.Point(681, 39);
            this.buttonSecondaryAnalysis.Name = "buttonSecondaryAnalysis";
            this.buttonSecondaryAnalysis.Size = new System.Drawing.Size(304, 256);
            this.buttonSecondaryAnalysis.TabIndex = 2;
            this.buttonSecondaryAnalysis.Text = "数据展示";
            this.buttonSecondaryAnalysis.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.buttonSecondaryAnalysis.UseVisualStyleBackColor = false;
            this.buttonSecondaryAnalysis.Click += new System.EventHandler(this.buttonSecondaryAnalysis_Click);
            // 
            // buttonConfig
            // 
            this.buttonConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(102)))), ((int)(((byte)(153)))));
            this.buttonConfig.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonConfig.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonConfig.ForeColor = System.Drawing.Color.White;
            this.buttonConfig.Image = global::IRApplication.Properties.Resources.HomeFormConfig;
            this.buttonConfig.Location = new System.Drawing.Point(681, 301);
            this.buttonConfig.Name = "buttonConfig";
            this.buttonConfig.Size = new System.Drawing.Size(304, 256);
            this.buttonConfig.TabIndex = 4;
            this.buttonConfig.Text = "配置界面";
            this.buttonConfig.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.buttonConfig.UseVisualStyleBackColor = false;
            this.buttonConfig.Click += new System.EventHandler(this.buttonConfig_Click);
            // 
            // buttonAlarm
            // 
            this.buttonAlarm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.buttonAlarm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonAlarm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonAlarm.Enabled = false;
            this.buttonAlarm.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonAlarm.ForeColor = System.Drawing.Color.White;
            this.buttonAlarm.Image = global::IRApplication.Properties.Resources.HomeFormAlarm;
            this.buttonAlarm.Location = new System.Drawing.Point(61, 39);
            this.buttonAlarm.Name = "buttonAlarm";
            this.buttonAlarm.Size = new System.Drawing.Size(304, 256);
            this.buttonAlarm.TabIndex = 3;
            this.buttonAlarm.Text = "告警查询";
            this.buttonAlarm.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.buttonAlarm.UseVisualStyleBackColor = false;
            // 
            // buttonRealtime
            // 
            this.buttonRealtime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(122)))), ((int)(((byte)(148)))));
            this.buttonRealtime.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanelMain.SetColumnSpan(this.buttonRealtime, 2);
            this.buttonRealtime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonRealtime.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonRealtime.ForeColor = System.Drawing.Color.White;
            this.buttonRealtime.Image = global::IRApplication.Properties.Resources.HomeFormRealtime;
            this.buttonRealtime.Location = new System.Drawing.Point(61, 301);
            this.buttonRealtime.Name = "buttonRealtime";
            this.buttonRealtime.Size = new System.Drawing.Size(614, 256);
            this.buttonRealtime.TabIndex = 0;
            this.buttonRealtime.Text = "实时";
            this.buttonRealtime.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.buttonRealtime.UseVisualStyleBackColor = false;
            this.buttonRealtime.Click += new System.EventHandler(this.buttonRealtime_Click);
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.tableLayoutPanelMain);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 48);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1049, 597);
            this.panelMain.TabIndex = 13;
            // 
            // sidebarBtn
            // 
            this.sidebarBtn.FlatAppearance.BorderSize = 0;
            this.sidebarBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sidebarBtn.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.sidebarBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.sidebarBtn.Image = global::IRApplication.Properties.Resources.LogoSide;
            this.sidebarBtn.Location = new System.Drawing.Point(0, 0);
            this.sidebarBtn.Name = "sidebarBtn";
            this.sidebarBtn.Size = new System.Drawing.Size(50, 42);
            this.sidebarBtn.TabIndex = 7;
            this.sidebarBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.sidebarBtn.UseVisualStyleBackColor = true;
            this.sidebarBtn.Click += new System.EventHandler(this.sidebarBtn_Click);
            // 
            // buttonReturn
            // 
            this.buttonReturn.FlatAppearance.BorderSize = 0;
            this.buttonReturn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonReturn.Location = new System.Drawing.Point(0, 0);
            this.buttonReturn.Name = "buttonReturn";
            this.buttonReturn.Size = new System.Drawing.Size(42, 42);
            this.buttonReturn.TabIndex = 5;
            this.buttonReturn.UseVisualStyleBackColor = true;
            // 
            // titleLabel
            // 
            this.titleLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.titleLabel.ForeColor = System.Drawing.Color.White;
            this.titleLabel.Location = new System.Drawing.Point(491, 11);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(69, 26);
            this.titleLabel.TabIndex = 6;
            this.titleLabel.Text = "主界面";
            // 
            // panelTopButton
            // 
            this.panelTopButton.Controls.Add(this.sidebarBtn);
            this.panelTopButton.Controls.Add(this.buttonReturn);
            this.panelTopButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelTopButton.Location = new System.Drawing.Point(3, 3);
            this.panelTopButton.Name = "panelTopButton";
            this.panelTopButton.Size = new System.Drawing.Size(50, 42);
            this.panelTopButton.TabIndex = 12;
            // 
            // tableLayoutPanelTittle
            // 
            this.tableLayoutPanelTittle.ColumnCount = 3;
            this.tableLayoutPanelTittle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanelTittle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tableLayoutPanelTittle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanelTittle.Controls.Add(this.titleLabel, 1, 0);
            this.tableLayoutPanelTittle.Controls.Add(this.panelTopButton, 2, 0);
            this.tableLayoutPanelTittle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelTittle.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelTittle.Name = "tableLayoutPanelTittle";
            this.tableLayoutPanelTittle.RowCount = 1;
            this.tableLayoutPanelTittle.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelTittle.Size = new System.Drawing.Size(1049, 48);
            this.tableLayoutPanelTittle.TabIndex = 13;
            // 
            // panelTittle
            // 
            this.panelTittle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(34)))), ((int)(((byte)(68)))));
            this.panelTittle.Controls.Add(this.tableLayoutPanelTittle);
            this.panelTittle.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTittle.Location = new System.Drawing.Point(0, 0);
            this.panelTittle.Name = "panelTittle";
            this.panelTittle.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.panelTittle.Size = new System.Drawing.Size(1049, 48);
            this.panelTittle.TabIndex = 12;
            // 
            // HomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1049, 645);
            this.Controls.Add(this.panelSidebar);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTittle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "HomeForm";
            this.Text = "HomeForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.HomeForm_Load);
            this.tableLayoutPanelBottom.ResumeLayout(false);
            this.panelSidebarBottom.ResumeLayout(false);
            this.tableLayoutPanelSidebar.ResumeLayout(false);
            this.panelSidebar.ResumeLayout(false);
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            this.panelTopButton.ResumeLayout(false);
            this.tableLayoutPanelTittle.ResumeLayout(false);
            this.tableLayoutPanelTittle.PerformLayout();
            this.panelTittle.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCloseSidebar;
        private System.Windows.Forms.Button buttonCloseClient;
        private System.Windows.Forms.Button buttonSidebarDownload;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBottom;
        private System.Windows.Forms.Button buttonSidebarShutdown;
        private System.Windows.Forms.Button buttonSidebarHome;
        private System.Windows.Forms.Button buttonSidebarConfig;
        private System.Windows.Forms.Button buttonSidebarRealtime;
        private System.Windows.Forms.Button buttonSidebarSecondaryAnalysis;
        private System.Windows.Forms.Button buttonSidebarPlaybackVideo;
        private System.Windows.Forms.Button buttonSidebarAlarmSearch;
        private System.Windows.Forms.Panel panelSidebarBottom;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSidebar;
        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Button buttonRecord;
        private System.Windows.Forms.Button buttonSecondaryAnalysis;
        private System.Windows.Forms.Button buttonConfig;
        private System.Windows.Forms.Button buttonAlarm;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.Button buttonRealtime;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Button sidebarBtn;
        private System.Windows.Forms.Button buttonReturn;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Panel panelTopButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelTittle;
        private System.Windows.Forms.Panel panelTittle;
    }
}