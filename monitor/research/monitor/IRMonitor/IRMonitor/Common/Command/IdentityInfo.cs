using System;
using System.Runtime.Serialization;

namespace IRMonitor.Common
{
    [DataContract]
    public class IdentityInfo
    {
        /// <summary>
        /// 身份识别类型(终端/PAD/WEB)
        /// </summary>
        [DataMember(Name = "IdentityType")]
        public String mIdentityType;
    }
}
