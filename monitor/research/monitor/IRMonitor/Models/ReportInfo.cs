using System;
using System.Collections.Generic;

namespace Models
{
    /// <summary>
    /// 报表数据
    /// </summary>
    public class ReportInfo
    {
        public String Provincial { get; set; } // 省
        public String City { get; set; } // 城市
        public String Company { get; set; } // 测试单位
        public String Projectleader { get; set; } // 项目负责人
        public String Testpersonnel { get; set; } // 测试人员

        public String SubStation { get; set; } // 变电站名称
        public DateTime StartTime { get; set; } // 告警开始时间
        public Byte[] ViewImage { get; set; } // 视图Image数据
        public Byte[] ChartImage { get; set; } // 折线图Image数据
        public Double StandardTemp { get; set; } // 标准温度
        public Double Emissivity { get; set; } // 辐射率
        public Double Distance { get; set; } // 距离
        public Double AmbientTemp { get; set; } // 环境温度
        public String DeviceName { get; set; } // 设备名称
        public String DeviceNum { get; set; }   // 设备序列号
        public String DevicePosition { get; set; } // 设备位置

        public List<ReportTemperature> ReportTemperatures { get; set; }
        public List<ReportGroupTemperature> ReportGruopTemperatures { get; set; }

        public ReportInfo()
        {
            ReportTemperatures = new List<ReportTemperature>();
            ReportGruopTemperatures = new List<ReportGroupTemperature>();
        }
    }

    /// <summary>
    /// 报表温度
    /// </summary>
    public class ReportTemperature
    {
        public Double MaxTemperature { get; set; } // 高温
        public Double MinTemperature { get; set; } // 低温
        public Double AvgTemperature { get; set; } // 平均温
        public DateTime HistoryTime { get; set; } // 历史时间
    }

    /// <summary>
    /// 选区组报表温度
    /// </summary>
    public class ReportGroupTemperature
    {
        public Double MaxTemperature { get; set; } // 高温
        public Double TemperatureDifference { get; set; } // 低温
        public Double TemperatureRise { get; set; } // 平均温
        public DateTime HistoryTime { get; set; } // 历史时间
    }
}
