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
using System.Text;
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
        public override void Send(string targetClientId, byte[] buffer, int offset, int length, object state)
        {
            SendAsync(targetClientId, buffer, offset, length, state);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Receive(string clientId, byte[] buffer, int length)
        {
            int headerLength = CLIENT_ID_LENGTH + CLIENT_ID_LENGTH;
            if (length < headerLength) {
                return;
            }

            byte[] data = buffer.SubArray(headerLength, length - headerLength);
            OnReceiveCallback?.Invoke(Encoding.UTF8.GetString(buffer, 0, CLIENT_ID_LENGTH), data, length);
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
                Receive(null, e.ApplicationMessage.Payload, e.ApplicationMessage.Payload.Length);
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
        /// 异步发送数据
        /// </summary>
        /// <param name="targetClientId">目标客户索引</param>
        /// <param name="buffer">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">长度</param>
        /// <param name="state">状态</param>
        /// <returns>任务</returns>
        private async void SendAsync(string targetClientId, byte[] buffer, int offset, int length, object state)
        {
            int newLength = CLIENT_ID_LENGTH + CLIENT_ID_LENGTH + length;
            byte[] data = new byte[newLength];
            Array.Copy(Encoding.UTF8.GetBytes(ClientId), 0, data, 0, CLIENT_ID_LENGTH);
            Array.Copy(Encoding.UTF8.GetBytes(targetClientId), 0, data, CLIENT_ID_LENGTH, CLIENT_ID_LENGTH);
            Array.Copy(buffer, offset, data, CLIENT_ID_LENGTH + CLIENT_ID_LENGTH, length);

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(data)
                .WithExactlyOnceQoS()
                .Build();

            await mqttClient.PublishAsync(message);
            OnSendCompletedCallback?.Invoke(state);
        }
    }
}
