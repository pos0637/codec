using Common;
using Miscs;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Communication
{
    /// <summary>
    /// 通讯会话管理器
    /// </summary>
    public class MQTTSessionManager : SessionManager
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
        /// 参数
        /// </summary>
        private IMqttClientOptions options;

        /// <summary>
        /// 主题
        /// </summary>
        private const string topic = "/SessionTopic";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="server">服务器</param>
        /// <param name="port">端口</param>
        /// <param name="clientId">客户端索引</param>
        public MQTTSessionManager(string server, int? port, string clientId)
            : base(clientId)
        {
            mqttClient = new MqttFactory().CreateMqttClient();
            cancellationToken = new CancellationTokenSource();
            options = new MqttClientOptionsBuilder()
                .WithTcpServer(server, port)
                .WithClientId(clientId)
                .WithCleanSession()
                .WithCommunicationTimeout(TimeSpan.FromMilliseconds(TIMEOUT))
                .Build();

            // 设置断线重连
            mqttClient.UseDisconnectedHandler(async e => {
                Tracker.LogNW(TAG, "disconnected");
                await Task.Delay(TimeSpan.FromMilliseconds(RETRY_DURATION));
                await ConnectAsync();
            });

            // 订阅主题
            mqttClient.UseConnectedHandler(async e => {
                Tracker.LogNW(TAG, "connected");
                await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic(topic).Build());
                Tracker.LogNW(TAG, "subscribed");
            });

            // 接收数据
            mqttClient.UseApplicationMessageReceivedHandler(e => {
                Tracker.LogNW(TAG, $"received[{e.ApplicationMessage.Topic}]: {string.Concat(e.ApplicationMessage.Payload?.Select(b => b.ToString("X2")).ToArray())}");
                ThreadPool.QueueUserWorkItem(state => OnReceive(e.ApplicationMessage.Payload, e.ApplicationMessage.Payload.Length));
            });

            // 异步连接
            ThreadPool.QueueUserWorkItem(async state => {
                await ConnectAsync();
            });
        }

        public override void Dispose()
        {
            cancellationToken?.Cancel();
            base.Dispose();
        }

        protected override void OnSend(byte[] data, int offset, int length)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(data.SubArray(offset, length))
                .WithExactlyOnceQoS()
                .Build();

            if (!mqttClient.PublishAsync(message).Wait(TIMEOUT, cancellationToken.Token)) {
                throw new TimeoutException();
            }
        }

        /// <summary>
        /// 异步连接
        /// </summary>
        private async Task ConnectAsync()
        {
            while (true) {
                try {
                    await mqttClient.ConnectAsync(options, cancellationToken.Token);
                    return;
                }
                catch (Exception) {
                    Tracker.LogNW(TAG, "connect fail");
                    await Task.Delay(TimeSpan.FromMilliseconds(RETRY_DURATION));
                }
            }
        }
    }
}
