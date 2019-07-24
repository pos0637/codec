using System;
using System.Runtime.Serialization;

namespace Models
{
    [DataContract]
    public class UserRoutineConfig
    {
        /// <summary>
        /// ID
        /// </summary>
        [DataMember(Name = "ID")]
        public Int32 mId;

        /// <summary>
        /// 省
        /// </summary>
        [DataMember(Name = "Province")]
        public String mProvince;

        /// <summary>
        /// 市
        /// </summary>
        [DataMember(Name = "City")]
        public String mCity;

        /// <summary>
        /// 公司
        /// </summary>
        [DataMember(Name = "Company")]
        public String mCompany;

        /// <summary>
        /// 项目负责人
        /// </summary>
        [DataMember(Name = "ProjectLeader")]
        public String mProjectLeader;

        /// <summary>
        /// 测试人员
        /// </summary>
        [DataMember(Name = "TestPersonnel")]
        public String mTestPersonnel;

        /// <summary>
        /// 变电站
        /// </summary>
        [DataMember(Name = "Substation")]
        public String mSubstation;

        /// <summary>
        /// 设备位置
        /// </summary>
        [DataMember(Name = "DevicePosition")]
        public String mDevicePosition;
    }
}
