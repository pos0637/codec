using System;
using System.Runtime.Serialization;

namespace IRMonitor.Common
{
    [DataContract]
    public class DiscoveryInfo
    {
        /// <summary>
        /// 客户IP
        /// </summary>
        [DataMember(Name = "IP")]
        public String mIP;

        /// <summary>
        /// 端口
        /// </summary>
        [DataMember(Name = "Port")]
        public Int32 mPort;
    }
}
