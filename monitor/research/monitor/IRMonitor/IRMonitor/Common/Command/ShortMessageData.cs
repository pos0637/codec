using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IRMonitor.Common
{
    /// <summary>
    /// 短信消息内容
    /// </summary>
    [DataContract]
    public class ShortMessageData
    {
        /// <summary>
        /// 序列号
        /// </summary>
        [DataMember(Name = "SerialNumber")]
        public String mSerialNumber;

        /// <summary>
        /// 用户电话列表
        /// </summary>
        [DataMember(Name = "UserPhoneList")]
        public List<String> mUserPhoneList;

        /// <summary>
        /// 短信内容
        /// </summary>
        [DataMember(Name = "Content")]
        public String mContent;
    }
}
