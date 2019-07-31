using Common;
using System;
using System.Collections.Generic;

namespace IRMonitor.CellService.Worker
{
    /// <summary>
    /// 短信服务线程
    /// </summary>
    public class SmsServiceWorker : BaseWorker
    {
        /// <summary>
        /// 短信结构体
        /// </summary>
        private class Message
        {
            /// <summary>
            /// 手机号码
            /// </summary>
            public List<string> mPhoneNumList;

            /// <summary>
            /// 短信内容
            /// </summary>
            public string mContent;

            public Message(List<string> phoneList, string content)
            {
                mPhoneNumList = phoneList;
                mContent = content;
            }
        }

        #region 委托

        public event Delegates.DgOnAutoReplyShortMessage gOnReceiveShortMessage;

        public event Delegates.DgOnSendShortMessage gOnSendShortMessage;

        #endregion

        #region 参数

        /// <summary>
        /// 唯一实例
        /// </summary>
        public static SmsServiceWorker Instance = new SmsServiceWorker();

        /// <summary>
        /// 失败次数
        /// </summary>
        private const int mMaxErrorTime = 10;

        /// <summary>
        /// 等待发送的短信
        /// </summary>
        private FixedLenQueue<Message> mWaitSendQueue = new FixedLenQueue<Message>(100);

        #endregion

        private SmsServiceWorker()
        {
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        public void SendMessage(List<String> phoneList, String content)
        {
        }

        protected override void Run()
        {
        }
    }
}
