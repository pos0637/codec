using System;
using System.Runtime.Serialization;

namespace IRMonitor.Common
{
    [DataContract]
    public class SelectionUpdateParam
    {
        [DataMember(Name = "Id")]
        public Int64 mId;

        [DataMember(Name = "Data")]
        public String mData;
    }

    [DataContract]
    public class SelectionConfigUpdate
    {
        [DataMember(Name = "Id")]
        public Int64 mId;

        [DataMember(Name = "Name")]
        public String mName;

        [DataMember(Name = "Data")]
        public String mData;
    }

    [DataContract]
    public class GroupSelectionUpdateParam
    {
        [DataMember(Name = "Id")]
        public Int64 mId;

        [DataMember(Name = "Data")]
        public String mData;
    }
}
