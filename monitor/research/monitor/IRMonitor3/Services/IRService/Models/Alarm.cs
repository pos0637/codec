using Common;
using System;
using System.Drawing;
using System.Runtime.Serialization;
using static Repository.Entities.Alarm;

namespace IRService.Models
{
    /// <summary>
    /// 告警
    /// </summary>
    [DataContract]
    public class Alarm
    {
        /// <summary>
        /// 告警类型
        /// </summary>
        [DataMember]
        public Repository.Entities.Alarm.Type type;

        /// <summary>
        /// 温度类型
        /// </summary>
        [DataMember]
        public Repository.Entities.Selections.TemperatureType temperatureType;

        /// <summary>
        /// 告警等级
        /// </summary>
        [DataMember]
        public Level level;

        /// <summary>
        /// 设备单元名称
        /// </summary>
        [DataMember]
        public string cellName;

        /// <summary>
        /// 设备名称
        /// </summary>
        [DataMember]
        public string deviceName;

        /// <summary>
        /// 选区名称
        /// </summary>
        [DataMember]
        public string selectionName;

        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]
        public DateTime startTime;

        /// <summary>
        /// 告警区域
        /// </summary>
        [DataMember]
        public Rectangle area;

        /// <summary>
        /// 告警点
        /// </summary>
        [DataMember]
        public Point point;

        /// <summary>
        /// 历史温度队列
        /// </summary>
        [NonSerialized]
        public FixedLengthQueue<float> temperatures;

        /// <summary>
        /// 温度矩阵
        /// </summary>
        [NonSerialized]
        public Buffer<float> temperature;

        /// <summary>
        /// 红外图像
        /// </summary>
        [NonSerialized]
        public Buffer<byte> irImage;

        /// <summary>
        /// 可见光图像
        /// </summary>
        [NonSerialized]
        public Buffer<byte> image;
    }
}
