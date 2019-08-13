using System.Runtime.Serialization;

namespace IRMonitor.Common
{
    [DataContract]
    public class SelectionUpdateParam
    {
        [DataMember(Name = "Id")]
        public long mId;

        [DataMember(Name = "Data")]
        public string mData;
    }

    [DataContract]
    public class SelectionConfigUpdate
    {
        [DataMember(Name = "Id")]
        public long mId;

        [DataMember(Name = "Name")]
        public string mName;

        [DataMember(Name = "Data")]
        public string mData;
    }

    [DataContract]
    public class GroupSelectionUpdateParam
    {
        [DataMember(Name = "Id")]
        public long mId;

        [DataMember(Name = "Data")]
        public string mData;
    }
}
