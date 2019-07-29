using Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IRMonitor.Common
{
    /// <summary>
    /// 报表
    /// </summary>
    [DataContract]
    public class Report
    {
        /// <summary>
        /// id列表
        /// </summary>
        [DataMember(Name = "IdList")]
        public List<Int64> mIdList;
    }

    [DataContract]
    public class ReportData
    {
        /// <summary>
        /// 类型
        /// 0：选区
        /// 1：组选区
        /// </summary>
        [DataMember(Name = "Type")]
        public Int32 mType;

        /// <summary>
        /// 索引
        /// </summary>
        [DataMember(Name = "Id")]
        public Int64 mId;

        /// <summary>
        /// 省份(缺省 例：福建省 = 福建)
        /// </summary>
        [DataMember(Name = "Province")]
        public String mProvince;

        /// <summary>
        /// 地市(缺市 例：福州市 = 福州)
        /// </summary>
        [DataMember(Name = "City")]
        public String mCity;

        /// <summary>
        /// 设备名称
        /// </summary>
        [DataMember(Name = "DeviceName")]
        public String mDeviceName;

        /// <summary>
        /// 报表类型 (0:告警报表 1:定时发送 2:定点发送)
        /// </summary>
        [DataMember(Name = "ReportType")]
        public Int32 mReportType;

        /// <summary>
        /// 测量单位
        /// </summary>
        [DataMember(Name = "Unit")]
        public String mUnit;

        /// <summary>
        /// 测试人员名称
        /// </summary>
        [DataMember(Name = "TestPersonName")]
        public String mTestPersonName;

        /// <summary>
        /// 项目负责人
        /// </summary>
        [DataMember(Name = "ChargePersonName")]
        public String mChargePersonName;

        /// <summary>
        /// 变电站
        /// </summary>
        [DataMember(Name = "Substation")]
        public String mSubstation;

        /// <summary>
        /// 图像数据
        /// Base64
        /// </summary>
        [DataMember(Name = "ImageData")]
        public String mImageData;

        /// <summary>
        /// 日期时间
        /// </summary>
        [DataMember(Name = "Datetime")]
        public String mDatetime;

        /// <summary>
        /// 红外参数
        /// </summary>
        [DataMember(Name = "IRParam")]
        public IRParam mIRParam;

        /// <summary>
        /// 设备信息
        /// </summary>
        [DataMember(Name = "DeviceInfo")]
        public DeviceInfo mDeviceInfo;
    }
}
