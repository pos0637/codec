using System.Runtime.Serialization;

namespace Models
{
    [DataContract]
    public class DeviceInfo
    {
        /// <summary>
        /// 设备型号
        /// </summary>
        [DataMember(Name = "EquipmentModel")]
        public string mEquipmentModel;

        /// <summary>
        /// 版本
        /// </summary>
        [DataMember(Name = "Version")]
        public string mVersion;

        /// <summary>
        /// 序列号
        /// </summary>
        [DataMember(Name = "SerialNumber")]
        public string mSerialNumber;

        /// <summary>
        /// 分辨率
        /// </summary>
        [DataMember(Name = "Resolution")]
        public string mResolution;

        /// <summary>
        /// 探测器类型
        /// </summary>
        [DataMember(Name = "DetectorType")]
        public string mDetectorType;
    }
}
