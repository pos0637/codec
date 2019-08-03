using Common;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Communication.Base
{
    /// <summary>
    /// MQTT通讯管道
    /// </summary>
    public class MQTTPipe : Pipe
    {
        /// <summary>
        /// 日志标志
        /// </summary>
        private const string TAG = "MQTT";

        /// <summary>
        /// 重连接间隔
        /// </summary>
        private const int RETRY_DURATION = 3000;

        /// <summary>
        /// 超时时间
        /// </summary>
        private const int TIMEOUT = 3000;

        /// <summary>
        /// 客户端
        /// </summary>
        private IMqttClient mqttClient;

        /// <summary>
        /// 取消令牌
        /// </summary>
        private CancellationTokenSource cancellationToken;

        /// <summary>
        /// 主题
        /// </summary>
        private string topic;

        /// <summary>
        /// 参数
        /// </summary>
        private IMqttClientOptions options;

        public MQTTPipe()
        {
            mqttClient = new MqttFactory().CreateMqttClient();
            cancellationToken = new CancellationTokenSource();

            // 设置断线重连
            mqttClient.UseDisconnectedHandler(async e => {
                Tracker.LogNW(TAG, "disconnected");
                OnDisconnectedCallback?.Invoke();
                await Task.Delay(TimeSpan.FromMilliseconds(RETRY_DURATION));
                await ConnectAsync();
            });
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Dispose()
        {
            cancellationToken.Cancel();
            base.Dispose();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Connect(Dictionary<string, object> arguments)
        {
            ConnectAsync(arguments);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Disconnect()
        {
            mqttClient.DisconnectAsync();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override bool SendData(byte[] buffer, object state)
        {
            bool? result = false;
            SendData(buffer, state, result);

            return result.Value;
        }

        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="arguments">参数列表</param>
        /// <returns>任务</returns>
        private async void ConnectAsync(Dictionary<string, object> arguments)
        {
            ClientId = arguments["ClientId"] as string;
            Debug.Assert(ClientId.Length == CLIENT_ID_LENGTH);
            topic = arguments["Topic"] as string;

            options = new MqttClientOptionsBuilder()
                .WithClientId(ClientId)
                .WithTcpServer(arguments["Server"] as string, arguments["Port"] as int?)
                .WithCleanSession()
                .WithCommunicationTimeout(TimeSpan.FromMilliseconds(TIMEOUT))
                .Build();

            // 订阅主题
            mqttClient.UseConnectedHandler(async e => {
                Tracker.LogNW(TAG, "connected");
                await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic(topic).Build());
                Tracker.LogNW(TAG, "subscribed");
            });

            // 接收数据
            mqttClient.UseApplicationMessageReceivedHandler(e => {
                Tracker.LogNW(TAG, $"received[{e.ApplicationMessage.Topic}]: {string.Concat(e.ApplicationMessage.Payload?.Select(b => b.ToString("X2")).ToArray())}");
                HandleReceiveData(null, e.ApplicationMessage.Payload, e.ApplicationMessage.Payload.Length);
            });

            await ConnectAsync();
        }

        /// <summary>
        /// 异步连接
        /// </summary>
        private async Task ConnectAsync()
        {
            while (true) {
                try {
                    // 连接
                    await mqttClient.ConnectAsync(options, cancellationToken.Token);
                    OnConnectedCallback?.Invoke();

                    return;
                }
                catch (Exception e) {
                    Tracker.LogNW(TAG, "connect fail");
                    OnExceptionCallback?.Invoke(e);
                    await Task.Delay(TimeSpan.FromMilliseconds(RETRY_DURATION));
                }
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="state">状态</param>
        /// <param name="result">是否成功</param>
        protected async void SendData(byte[] buffer, object state, bool? result)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(buffer)
                .WithExactlyOnceQoS()
                .Build();

            try {
                await mqttClient.PublishAsync(message);
                OnSendCompletedCallback?.Invoke(state);
                result = true;
            }
            catch (Exception e) {
                Tracker.LogE(TAG, e);
                OnExceptionCallback?.Invoke(e);
                result = false;
            }
        }
    }
}
