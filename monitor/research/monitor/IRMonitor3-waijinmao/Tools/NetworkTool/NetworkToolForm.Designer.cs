namespace NetworkTool
{
    partial class NetworkToolForm
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxSdkCfg = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.textBoxDns = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.textBoxSubMask = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.textBoxGateWay = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.textBoxIPAddr = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.btnNetCfgGet = new System.Windows.Forms.Button();
            this.btnNetCfgSet = new System.Windows.Forms.Button();
            this.comboBox_cells = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxSdkCfg);
            this.groupBox2.Controls.Add(this.label25);
            this.groupBox2.Controls.Add(this.textBoxDns);
            this.groupBox2.Controls.Add(this.label23);
            this.groupBox2.Controls.Add(this.textBoxSubMask);
            this.groupBox2.Controls.Add(this.label22);
            this.groupBox2.Controls.Add(this.textBoxGateWay);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.textBoxIPAddr);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Location = new System.Drawing.Point(12, 47);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(457, 105);
            this.groupBox2.TabIndex = 49;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "网络参数 Network Parameter";
            // 
            // textBoxSdkCfg
            // 
            this.textBoxSdkCfg.Location = new System.Drawing.Point(302, 48);
            this.textBoxSdkCfg.Name = "textBoxSdkCfg";
            this.textBoxSdkCfg.Size = new System.Drawing.Size(130, 21);
            this.textBoxSdkCfg.TabIndex = 77;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(220, 52);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(71, 12);
            this.label25.TabIndex = 76;
            this.label25.Text = "设备端口号:";
            // 
            // textBoxDns
            // 
            this.textBoxDns.Location = new System.Drawing.Point(302, 21);
            this.textBoxDns.Name = "textBoxDns";
            this.textBoxDns.Size = new System.Drawing.Size(130, 21);
            this.textBoxDns.TabIndex = 63;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(243, 25);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(53, 12);
            this.label23.TabIndex = 62;
            this.label23.Text = "首选DNS:";
            // 
            // textBoxSubMask
            // 
            this.textBoxSubMask.Location = new System.Drawing.Point(65, 74);
            this.textBoxSubMask.Name = "textBoxSubMask";
            this.textBoxSubMask.Size = new System.Drawing.Size(130, 21);
            this.textBoxSubMask.TabIndex = 61;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(6, 78);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(59, 12);
            this.label22.TabIndex = 60;
            this.label22.Text = "子网掩码:";
            // 
            // textBoxGateWay
            // 
            this.textBoxGateWay.Location = new System.Drawing.Point(65, 47);
            this.textBoxGateWay.Name = "textBoxGateWay";
            this.textBoxGateWay.Size = new System.Drawing.Size(130, 21);
            this.textBoxGateWay.TabIndex = 59;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(6, 51);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(59, 12);
            this.label20.TabIndex = 58;
            this.label20.Text = "默认网关:";
            // 
            // textBoxIPAddr
            // 
            this.textBoxIPAddr.Location = new System.Drawing.Point(65, 20);
            this.textBoxIPAddr.Name = "textBoxIPAddr";
            this.textBoxIPAddr.Size = new System.Drawing.Size(130, 21);
            this.textBoxIPAddr.TabIndex = 57;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(6, 24);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(59, 12);
            this.label19.TabIndex = 56;
            this.label19.Text = "IPv4地址:";
            // 
            // btnNetCfgGet
            // 
            this.btnNetCfgGet.Location = new System.Drawing.Point(278, 10);
            this.btnNetCfgGet.Name = "btnNetCfgGet";
            this.btnNetCfgGet.Size = new System.Drawing.Size(75, 23);
            this.btnNetCfgGet.TabIndex = 57;
            this.btnNetCfgGet.Text = "刷新";
            this.btnNetCfgGet.UseVisualStyleBackColor = true;
            this.btnNetCfgGet.Click += new System.EventHandler(this.btnNetCfgGet_Click);
            // 
            // btnNetCfgSet
            // 
            this.btnNetCfgSet.Location = new System.Drawing.Point(369, 10);
            this.btnNetCfgSet.Name = "btnNetCfgSet";
            this.btnNetCfgSet.Size = new System.Drawing.Size(75, 23);
            this.btnNetCfgSet.TabIndex = 56;
            this.btnNetCfgSet.Text = "设置";
            this.btnNetCfgSet.UseVisualStyleBackColor = true;
            this.btnNetCfgSet.Click += new System.EventHandler(this.btnNetCfgSet_Click);
            // 
            // comboBox_cells
            // 
            this.comboBox_cells.FormattingEnabled = true;
            this.comboBox_cells.Location = new System.Drawing.Point(77, 12);
            this.comboBox_cells.Name = "comboBox_cells";
            this.comboBox_cells.Size = new System.Drawing.Size(180, 20);
            this.comboBox_cells.TabIndex = 50;
            this.comboBox_cells.SelectedIndexChanged += new System.EventHandler(this.comboBox_cells_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 51;
            this.label1.Text = "设备名称:";
            // 
            // NetworkToolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 166);
            this.Controls.Add(this.btnNetCfgGet);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnNetCfgSet);
            this.Controls.Add(this.comboBox_cells);
            this.Controls.Add(this.groupBox2);
            this.Name = "NetworkToolForm";
            this.Text = "网络设置工具";
            this.Load += new System.EventHandler(this.NetworkToolForm_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnNetCfgGet;
        private System.Windows.Forms.Button btnNetCfgSet;
        private System.Windows.Forms.TextBox textBoxSdkCfg;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox textBoxDns;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox textBoxSubMask;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox textBoxGateWay;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox textBoxIPAddr;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.ComboBox comboBox_cells;
        private System.Windows.Forms.Label label1;
    }
}

