using IRService.Services.Cell;
using System.Linq;
using System.Windows.Forms;

namespace NetworkTool
{
    public partial class NetworkToolForm : Form
    {
        /// <summary>
        /// 设备单元服务列表
        /// </summary>
        private readonly CellService[] services;

        public NetworkToolForm()
        {
            InitializeComponent();
            services = CellServiceManager.Instance.GetServices().OfType<CellService>().ToArray();
        }

        /// <summary>
        /// 刷新设备单元列表
        /// </summary>
        private void RefreshCells()
        {
            comboBox_cells.Items.Clear();
            foreach (var service in services) {
                comboBox_cells.Items.Add(service.cell.name);
            }

            if (comboBox_cells.Items.Count > 0) {
                comboBox_cells.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 刷新网络参数
        /// </summary>
        /// <param name="index">设备单元索引</param>
        private void RefreshParameters(int index)
        {
            var parameters = services[index].GetNetworkParameters(null);
            if (parameters == null) {
                return;
            }

            textBoxIPAddr.Text = parameters.ip;
            textBoxGateWay.Text = parameters.gateway;
            textBoxSubMask.Text = parameters.mask;
            textBoxDns.Text = parameters.dns1;
            textBoxSdkCfg.Text = parameters.port.ToString();
        }

        /// <summary>
        /// 保存网络参数
        /// </summary>
        /// <param name="index">设备单元索引</param>
        private void SaveParameters(int index)
        {
            var parameters = new Repository.Entities.Configuration.NetworkParameters() {
                ip = textBoxIPAddr.Text,
                gateway = textBoxGateWay.Text,
                mask = textBoxSubMask.Text,
                dns1 = textBoxDns.Text
            };

            int.TryParse(textBoxSdkCfg.Text, out parameters.port);

            if (!services[index].SetNetworkParameters(null, parameters)) {
                MessageBox.Show("设置失败!");
            }
            else {
                MessageBox.Show("设置成功, 即将重启设备!");
                services[index].RebootDevice(null);
                Application.Exit();
            }
        }

        private void NetworkToolForm_Load(object sender, System.EventArgs e)
        {
            RefreshCells();
        }

        private void comboBox_cells_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            RefreshParameters(comboBox_cells.SelectedIndex);
        }

        private void btnNetCfgGet_Click(object sender, System.EventArgs e)
        {
            RefreshParameters(comboBox_cells.SelectedIndex);
        }

        private void btnNetCfgSet_Click(object sender, System.EventArgs e)
        {
            SaveParameters(comboBox_cells.SelectedIndex);
        }
    }
}
