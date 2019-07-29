using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace IRMonitor.Common
{
    /// <summary>
    /// 下载请求
    /// </summary>
    [DataContract]
    public class DownloadRequest
    {
        /// <summary>
        /// 下载类型(0:告警 1:手动)
        /// </summary>
        [DataMember(Name = "Type")]
        public Int32 mType;

        /// <summary>
        /// 类型Id
        /// </summary>
        [DataMember(Name = "Id")]
        public Int64 mId;

        /// <summary>
        /// 下载序列号
        /// </summary>
        [DataMember(Name = "DownloadId")]
        public Double mDownloadId;

        /// <summary>
        /// 文件列表
        /// </summary>
        [DataMember(Name = "FileList")]
        public List<String> mFileList;
    }

    /// <summary>
    /// 下载Info数据
    /// </summary>
    [DataContract]
    public class DownloadInfo
    {
        /// <summary>
        /// 下载序列号
        /// </summary>
        [DataMember(Name = "DownloadId")]
        public Double mDownloadId;

        /// <summary>
        /// 文件名称
        /// </summary>
        [DataMember(Name = "File")]
        public String mFile;

        /// <summary>
        /// 回调数据
        /// </summary>
        [DataMember(Name = "Data")]
        public String mData;
    }

    /// <summary>
    /// 下载信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DownloadFrame
    {
        public Double mId;
        public Int32 mFrameNum;
        public IFrameInfo mFrameInfo;
        public Int64 mDownloadPosition;
        public Int64 mDownloadLength;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IFrameInfo
    {
        public Boolean mIsIFrame;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public String mVideoFileName;
        public Int64 mVideoPosition;
        public Int64 mVideoLength;
        public Int64 mSelectionDatalength;
    }
}
