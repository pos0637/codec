using System;
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
        public String mEquipmentModel;

        /// <summary>
        /// 版本
        /// </summary>
        [DataMember(Name = "Version")]
        public String mVersion;

        /// <summary>
        /// 序列号
        /// </summary>
        [DataMember(Name = "SerialNumber")]
        public String mSerialNumber;

        /// <summary>
        /// 分辨率
        /// </summary>
        [DataMember(Name = "Resolution")]
        public String mResolution;

        /// <summary>
        /// 探测器类型
        /// </summary>
        [DataMember(Name = "DetectorType")]
        public String mDetectorType;
    }
}
