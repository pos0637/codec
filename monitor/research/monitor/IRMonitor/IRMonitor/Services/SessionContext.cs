using Communication.Base;
using Communication.Session;
using IRMonitor.ServiceManager;
using IRMonitor.Services.LiveStreaming;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace IRMonitor.Services
{
    /// <summary>
    /// 会话上下文
    /// </summary>
    public class SessionContext : ISessionContext
    {
        /// <summary>
        /// 会话通讯管道
        /// </summary>
        private SessionPipe pipe;

        /// <summary>
        /// 对象列表
        /// </summary>
        private Hashtable hashtable = Hashtable.Synchronized(new Hashtable());

        public void Dispose()
        {
            StopLiveStreaming();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void OnReceived(Pipe.Response response, byte[] buffer, int length)
        {
            throw new NotImplementedException();
        }

        public void OnSessionClosed(SessionPipe pipe)
        {
        }

        public void OnSessionConnected(SessionPipe pipe)
        {
            this.pipe = pipe;
        }

        #region 实时推流服务

        /// <summary>
        /// 启动实时推流服务
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private string StartLiveStreaming()
        {
            if (hashtable.ContainsKey("LiveStreamingService")) {
                return null;
            }

            // 移动式红外监控系统仅有一个设备单元
            var guid = Guid.NewGuid().ToString();
            var serviceId = LiveStreamingServiceManager.Instance.AddService(new Dictionary<string, object>() {
                { "Cell", CellServiceManager.gIRServiceList[0] },
                { "StreamId", guid }
            });
            LiveStreamingServiceManager.Instance.GetService(serviceId).Start();
            hashtable["LiveStreamingService"] = serviceId;

            return guid;
        }

        /// <summary>
        /// 停止实时推流服务
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void StopLiveStreaming()
        {
            if (!hashtable.ContainsKey("LiveStreamingService")) {
                return;
            }

            LiveStreamingServiceManager.Instance.RemoveService(hashtable["LiveStreamingService"] as string);
            hashtable.Remove("LiveStreamingService");
        }

        #endregion
    }
}
