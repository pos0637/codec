namespace IRApplication.UI
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonSearchWifi = new System.Windows.Forms.Button();
            this.buttonConnectCloud = new System.Windows.Forms.Button();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.buttonCloseClient = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.tableLayoutPanel1.Controls.Add(this.buttonSearchWifi, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonConnectCloud, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.pictureBoxLogo, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonCloseClient, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1046, 555);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // buttonSearchWifi
            // 
            this.buttonSearchWifi.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonSearchWifi.Image = ((System.Drawing.Image)(resources.GetObject("buttonSearchWifi.Image")));
            this.buttonSearchWifi.Location = new System.Drawing.Point(252, 147);
            this.buttonSearchWifi.Name = "buttonSearchWifi";
            this.buttonSearchWifi.Size = new System.Drawing.Size(216, 230);
            this.buttonSearchWifi.TabIndex = 0;
            this.buttonSearchWifi.UseVisualStyleBackColor = true;
            this.buttonSearchWifi.Click += new System.EventHandler(this.buttonSearchWifi_Click);
            // 
            // buttonConnectCloud
            // 
            this.buttonConnectCloud.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonConnectCloud.Image = ((System.Drawing.Image)(resources.GetObject("buttonConnectCloud.Image")));
            this.buttonConnectCloud.Location = new System.Drawing.Point(577, 147);
            this.buttonConnectCloud.Name = "buttonConnectCloud";
            this.buttonConnectCloud.Size = new System.Drawing.Size(213, 230);
            this.buttonConnectCloud.TabIndex = 3;
            this.buttonConnectCloud.UseVisualStyleBackColor = true;
            this.buttonConnectCloud.Click += new System.EventHandler(this.buttonConnectCloud_Click);
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel1.SetColumnSpan(this.pictureBoxLogo, 3);
            this.pictureBoxLogo.Image = global::IRApplication.Properties.Resources.LogoHome;
            this.pictureBoxLogo.Location = new System.Drawing.Point(392, 42);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(260, 80);
            this.pictureBoxLogo.TabIndex = 1;
            this.pictureBoxLogo.TabStop = false;
            // 
            // buttonCloseClient
            // 
            this.buttonCloseClient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCloseClient.BackColor = System.Drawing.Color.Transparent;
            this.buttonCloseClient.FlatAppearance.BorderSize = 0;
            this.buttonCloseClient.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCloseClient.ForeColor = System.Drawing.Color.Transparent;
            this.buttonCloseClient.Image = ((System.Drawing.Image)(resources.GetObject("buttonCloseClient.Image")));
            this.buttonCloseClient.Location = new System.Drawing.Point(3, 505);
            this.buttonCloseClient.Name = "buttonCloseClient";
            this.buttonCloseClient.Size = new System.Drawing.Size(64, 47);
            this.buttonCloseClient.TabIndex = 4;
            this.buttonCloseClient.UseVisualStyleBackColor = false;
            this.buttonCloseClient.Click += new System.EventHandler(this.buttonCloseClient_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1046, 555);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LoginForm";
            this.Text = "LoginForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonSearchWifi;
        private System.Windows.Forms.Button buttonConnectCloud;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.Button buttonCloseClient;
    }
}