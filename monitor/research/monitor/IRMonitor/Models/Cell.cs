using System;
using System.Runtime.Serialization;

namespace Models
{
    /// <summary>
    /// 设备单元信息
    /// </summary>
    [DataContract]
    public class Cell
    {
        [DataMember(Name = "CellId")]
        public Int64 mCellId;

        [DataMember(Name = "CellName")]
        public String mCellName;

        [DataMember(Name = "IRCameraIP")]
        public String mIRCameraIp;

        [DataMember(Name = "IRCameraWidth")]
        public Int32 mIRCameraWidth;

        [DataMember(Name = "IRCameraStride")]
        public Int32 mIRCameraStride;

        [DataMember(Name = "IRCameraHeight")]
        public Int32 mIRCameraHeight;

        [DataMember(Name = "IRCameraBitsPerPixel")]
        public Int32 mIRCameraBitsPerPixel;

        [NonSerialized]
        public Int32 mIRCameraVideoFrameRate;

        [NonSerialized]
        public Int32 mIRCameraTemperatureFrameRate;

        [NonSerialized]
        public Int32 mIRCameraVideoDuration;

        [NonSerialized]
        public String mIRCameraVideoFolder;

        [NonSerialized]
        public String mIRCameraImageFolder;

        [DataMember(Name = "IRCameraType")]
        public String mIRCameraType;
    }
}
