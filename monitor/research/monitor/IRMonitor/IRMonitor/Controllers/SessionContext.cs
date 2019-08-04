﻿using Common;
using Communication.Base;
using Communication.Session;
using IRMonitor.Miscs;
using IRMonitor.Services.LiveStreaming;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace IRMonitor.Controllers
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

        public SessionContext(SessionPipe pipe)
        {
            this.pipe = pipe;
        }

        private SessionContext() { }

        public void Dispose()
        {
            StopLiveStreaming();
        }

        public void Initialize()
        {
        }

        public void OnReceived(Pipe.Request request, byte[] buffer, int length)
        {
            try {
                var data = Encoding.UTF8.GetString(buffer);
                var arguments = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                if ((arguments == null) || (!arguments.ContainsKey("method"))) {
                    buffer = GetResponse(false, "invalid arguments!");
                    pipe?.Send(buffer, 0, buffer.Length, null);
                    return;
                }

                var result = MethodUtils.Invoke(this, arguments["method"] as string, arguments);
                if (result != null) {
                    buffer = GetResponse(true, result);
                    pipe?.Send(buffer, 0, buffer.Length, null);
                }
            }
            catch (Exception e) {
                buffer = GetResponse(false, e);
                pipe?.Send(buffer, 0, buffer.Length, null);
                Tracker.LogE(e);
            }
        }

        public void OnSessionClosed(SessionPipe pipe)
        {
        }

        public void OnSessionConnected(SessionPipe pipe)
        {
            this.pipe = pipe;
        }

        /// <summary>
        /// 获取响应信息
        /// </summary>
        /// <param name="success">是否成功</param>
        /// <param name="result">返回值</param>
        /// <returns>响应信息</returns>
        private byte[] GetResponse(bool success, object result)
        {
            if (success) {
                return Encoding.UTF8.GetBytes($"{{ code: 200, data: \"{JsonConvert.SerializeObject(result)}\",  message: \"\" }}");
            }
            else {
                return Encoding.UTF8.GetBytes($"{{ code: 500, data: \"\",  message: \"{JsonConvert.SerializeObject(result)}\" }}");
            }
        }

        #region 实时推流服务

        /// <summary>
        /// 启动实时推流服务
        /// </summary>
        /// <param name="cellId">设备单元索引</param>
        /// <returns>流索引</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string StartLiveStreaming(int cellId)
        {
            if (hashtable.ContainsKey("LiveStreamingService")) {
                return hashtable["LiveStreamingService"] as string;
            }

            var guid = Guid.NewGuid().ToString();
            var serviceId = LiveStreamingServiceManager.Instance.AddService(new Dictionary<string, object>() {
                { "CellId", cellId },
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
        public void StopLiveStreaming()
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
