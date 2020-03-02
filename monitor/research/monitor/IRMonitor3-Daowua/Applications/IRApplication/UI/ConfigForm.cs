using Repository.Entities;
using System;
using System.Windows.Forms;

namespace IRApplication.UI
{
    public partial class ConfigForm : Form
    {
        /// <summary>
        /// 配置信息
        /// </summary>
        private Configuration configuartion;

        public ConfigForm()
        {
            InitializeComponent();
            LoadConfiguration();
        }

        /// <summary>
        /// 加载配置信息
        /// </summary>
        private void LoadConfiguration()
        {
            configuartion = Repository.Repository.LoadConfiguation();
            text_province.Text = configuartion.information.province;
            text_city.Text = configuartion.information.city;
            text_clientId.Text = configuartion.information.clientId;
            text_company.Text = configuartion.information.company;
            text_location.Text = configuartion.information.location;
            text_manager.Text = configuartion.information.manager;
            text_mqttServerIp.Text = configuartion.information.mqttServerIp;
            text_rtmpServerIp.Text = configuartion.information.rtmpServerIp;
            text_mqttServerPort.Text = configuartion.information.mqttServerPort.ToString();
            text_rtmpServerport.Text = configuartion.information.rtmpServerPort.ToString();
            com_onlineMode.Text = configuartion.information.onlineMode == true ? "是" : "否";
            com_recordingMode.Text = configuartion.information.recordingMode == true ? "是" : "否";
            text_recordingDuration.Text = configuartion.information.recordingDuration.ToString();
            text_saveImagePath.Text = configuartion.information.saveImagePath;
            text_saveVideoPath.Text = configuartion.information.saveVideoPath;
            text_serverUrl.Text = configuartion.information.serverUrl;
            text_substation.Text = configuartion.information.substation;
            text_tester.Text = configuartion.information.tester;
        }

        /// <summary>
        /// 保存配置信息
        /// </summary>
        private void SetConfiguration()
        {
            configuartion.information.province = text_province.Text;
            configuartion.information.city = text_city.Text;
            configuartion.information.clientId = text_clientId.Text;
            configuartion.information.company = text_company.Text;
            configuartion.information.location = text_location.Text;
            configuartion.information.manager = text_manager.Text;
            configuartion.information.mqttServerIp = text_mqttServerIp.Text;
            configuartion.information.rtmpServerIp = text_rtmpServerIp.Text;
            configuartion.information.mqttServerPort = Convert.ToInt32(text_mqttServerPort.Text);
            configuartion.information.rtmpServerPort = Convert.ToInt32(text_rtmpServerport.Text);
            configuartion.information.onlineMode = com_onlineMode.Text == "是" ? true : false;
            configuartion.information.recordingMode = com_recordingMode.Text == "是" ? true : false;
            configuartion.information.recordingDuration = Convert.ToInt32(text_recordingDuration.Text);
            configuartion.information.saveImagePath = text_saveImagePath.Text;
            configuartion.information.saveVideoPath = text_saveVideoPath.Text;
            configuartion.information.serverUrl = text_serverUrl.Text;
            configuartion.information.substation = text_substation.Text;
            configuartion.information.tester = text_tester.Text;

            Repository.Repository.SaveConfiguation(configuartion);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SetConfiguration();
            LoadConfiguration();
            MessageBox.Show("保存成功,请重启系统!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog {
                Description = "请选择文件路径"
            };

            if (dialog.ShowDialog() == DialogResult.OK) {
                text_saveVideoPath.Text = dialog.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog {
                Description = "请选择文件路径"
            };

            if (dialog.ShowDialog() == DialogResult.OK) {
                text_saveImagePath.Text = dialog.SelectedPath;
            }
        }
    }
}

