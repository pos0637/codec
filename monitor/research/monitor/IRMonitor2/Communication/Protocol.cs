using Miscs;
using System;
using System.Text;

namespace Communication
{
    /// <summary>
    /// 通讯协议
    /// </summary>
    public class Protocol
    {
        /// <summary>
        /// 用户索引长度
        /// </summary>
        public const int CLIENT_ID_LENGTH = 4;

        /// <summary>
        /// 会话索引长度
        /// </summary>
        public const int SESSION_ID_LENGTH = 4;

        /// <summary>
        /// 空会话索引
        /// </summary>
        public const string INVALID_SESSION_ID = "----";

        /// <summary>
        /// 源索引
        /// </summary>
        public string SrcId { get; set; }

        /// <summary>
        /// 目标索引
        /// </summary>
        public string DstId { get; set; }

        /// <summary>
        /// 会话索引
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// 构造数据
        /// </summary>
        /// <param name="session">会话</param>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">长度</param>
        /// <returns>协议数据</returns>
        public static byte[] Build(Session session, byte[] data, int offset, int length)
        {
            if (length < 0) {
                length = data.Length;
            }

            var sessionId = session.sessionId;
            if (string.IsNullOrEmpty(sessionId)) {
                sessionId = INVALID_SESSION_ID;
            }

            var headerLength = CLIENT_ID_LENGTH + CLIENT_ID_LENGTH + SESSION_ID_LENGTH;
            var content = new byte[headerLength + length];
            Array.Copy(Encoding.UTF8.GetBytes(session.srcId), 0, content, 0, CLIENT_ID_LENGTH);
            Array.Copy(Encoding.UTF8.GetBytes(session.dstId), 0, content, CLIENT_ID_LENGTH, CLIENT_ID_LENGTH);
            Array.Copy(Encoding.UTF8.GetBytes(sessionId), 0, content, CLIENT_ID_LENGTH + CLIENT_ID_LENGTH, SESSION_ID_LENGTH);
            if (data != null) {
                Array.Copy(data, offset, content, headerLength, length);
            }

            return content;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="buffer">接收缓冲区</param>
        /// <param name="length">接收字节数</param>
        /// <returns>协议数据</returns>
        public static Protocol Parse(byte[] buffer, int length)
        {
            var headerLength = CLIENT_ID_LENGTH + CLIENT_ID_LENGTH + SESSION_ID_LENGTH;
            if ((buffer.Length < headerLength) || (length < headerLength)) {
                return null;
            }

            var protocol = new Protocol();
            protocol.SrcId = Encoding.UTF8.GetString(buffer.SubArray(0, CLIENT_ID_LENGTH));
            protocol.DstId = Encoding.UTF8.GetString(buffer.SubArray(CLIENT_ID_LENGTH, CLIENT_ID_LENGTH));
            protocol.SessionId = Encoding.UTF8.GetString(buffer.SubArray(CLIENT_ID_LENGTH + CLIENT_ID_LENGTH, SESSION_ID_LENGTH));
            if (protocol.SessionId.Equals(INVALID_SESSION_ID)) {
                protocol.SessionId = null;
            }

            var data = new byte[length - headerLength];
            protocol.Data = buffer.SubArray(headerLength, data.Length);

            return protocol;
        }

        private Protocol() { }
    }
}
