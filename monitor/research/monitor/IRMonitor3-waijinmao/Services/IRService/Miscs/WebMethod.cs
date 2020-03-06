using Common;

namespace IRService.Miscs
{
    /// <summary>
    /// 平台请求库
    /// </summary>
    public static class WebMethod
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public static string serverUrl = Repository.Repository.LoadConfiguation().information.serverUrl;

        #region

        /// <summary>
        /// 设备信息返回数据
        /// </summary>
        public class DeviceResponse
        {
            /// <summary>
            /// 内容
            /// </summary>
            public int content { get; set; }

            /// <summary>
            /// 代码
            /// </summary>
            public int code { get; set; }

            /// <summary>
            /// 错误码
            /// </summary>
            public int errno { get; set; }

            /// <summary>
            /// 消息
            /// </summary>
            public string message { get; set; }

            /// <summary>
            /// 令牌
            /// </summary>
            public string newToken { get; set; }

            /// <summary>
            /// 设备信息
            /// </summary>
            public Device _embedded { get; set; }
        }

        /// <summary>
        /// 设备信息
        /// </summary>
        public class Device
        {
            /// <summary>
            /// 索引
            /// </summary>
            public int id { get; set; }

            /// <summary>
            /// 序列号
            /// </summary>
            public string serialNumber { get; set; }

            /// <summary>
            /// 可见光视频流推流地址
            /// </summary>
            public string pushUrl { get; set; }

            /// <summary>
            /// 红外视频流推流地址
            /// </summary>
            public string irPushUrl { get; set; }

            /// <summary>
            /// 状态
            /// </summary>
            public string status { get; set; }

            public override bool Equals(object obj)
            {
                if (obj == null) {
                    return base.Equals(obj);
                }

                var device = obj as Device;
                return (id == device.id)
                    && string.Equals(serialNumber, device.serialNumber)
                    && string.Equals(pushUrl, device.pushUrl)
                    && string.Equals(irPushUrl, device.irPushUrl)
                    && string.Equals(status, device.status);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        /// <summary>
        /// 设备参数
        /// </summary>
        public class DeviceParameter
        {
            /// <summary>
            /// 序列号
            /// </summary>
            public string serialNumber;

            /// <summary>
            /// 设备状态 1-工作 2-空闲 3-告警
            /// </summary>
            public string status;
        }

        /// <summary>
        /// 更新设备参数返回数据
        /// </summary>
        public class UpdateDeviceResponse
        {
            /// <summary>
            /// 内容
            /// </summary>
            public int content { get; set; }

            /// <summary>
            /// 代码
            /// </summary>
            public int code { get; set; }

            /// <summary>
            /// 错误码
            /// </summary>
            public int errno { get; set; }

            /// <summary>
            /// 消息
            /// </summary>
            public string message { get; set; }

            /// <summary>
            /// 令牌
            /// </summary>
            public string newToken { get; set; }
        }

        /// <summary>
        /// 告警
        /// </summary>
        public class Alarm
        {
            /// <summary>
            /// 序列号
            /// </summary>
            public string serialNumber;

            /// <summary>
            /// 可见光图像快照
            /// </summary>
            public string image;

            /// <summary>
            /// 红外图像快照
            /// </summary>
            public string irImage;

            /// <summary>
            /// 温度矩阵快照
            /// </summary>
            public string temperature;

            /// <summary>
            /// 扩展数据
            /// </summary>
            public string data;

            /// <summary>
            /// 开始时间
            /// </summary>
            public string datetime;
        }

        /// <summary>
        /// 添加告警返回数据
        /// </summary>
        public class AddAlarmResponse
        {
            /// <summary>
            /// 内容
            /// </summary>
            public int content { get; set; }

            /// <summary>
            /// 代码
            /// </summary>
            public int code { get; set; }

            /// <summary>
            /// 错误码
            /// </summary>
            public int errno { get; set; }

            /// <summary>
            /// 消息
            /// </summary>
            public string message { get; set; }

            /// <summary>
            /// 令牌
            /// </summary>
            public string newToken { get; set; }
        }

        #endregion

        /// <summary>
        /// 根据设备序列号获取设备信息
        /// </summary>
        /// <param name="serialNumber">序列号</param>
        /// <param name="timeout">超时(毫秒)</param>
        /// <returns>设备信息</returns>
        public static Device GetDevice(string serialNumber, int timeout = 3000)
        {
            var result = HttpMethod.Get($"{serverUrl}/api/v1/ir/devices/serialNumber/{serialNumber}", timeout);
            return JsonUtils.ObjectFromJson<DeviceResponse>(result)?._embedded;
        }

        /// <summary>
        /// 更新设备状态
        /// </summary>
        /// <param name="updateDeviceParameter">修改设备信息</param>
        /// <param name="timeout">超时(毫秒)</param>
        /// <returns>结果</returns>
        public static UpdateDeviceResponse UpdateDeviceStatus(DeviceParameter deviceParameter, int timeout = 3000)
        {
            var result = HttpMethod.Put($"{serverUrl}/api/v1/ir/devices/status/{deviceParameter.serialNumber}", JsonUtils.ObjectToJson(deviceParameter), timeout);
            return JsonUtils.ObjectFromJson<UpdateDeviceResponse>(result);
        }

        /// <summary>
        /// 增加告警
        /// </summary>
        /// <param name="alarm">告警</param>
        /// <param name="timeout">超时(毫秒)</param>
        /// <returns>结果</returns>
        public static AddAlarmResponse AddAlarm(Alarm alarm, int timeout = 3000)
        {
            var result = HttpMethod.Post($"{serverUrl}/api/v1/ir/alarms", JsonUtils.ObjectToJson(alarm), timeout);
            return JsonUtils.ObjectFromJson<AddAlarmResponse>(result);
        }
    }
}
