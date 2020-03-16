namespace IRService.Common
{
    /// <summary>
    /// 常量
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// 接收温度事件名称
        /// </summary>
        public static readonly string EVENT_SERVICE_RECEIVE_TEMPERATURE = "EVENT_SERVICE_RECEIVE_TEMPERATURE";

        /// <summary>
        /// 接收红外图像事件名称
        /// </summary>
        public static readonly string EVENT_SERVICE_RECEIVE_IRIMAGE = "EVENT_SERVICE_RECEIVE_IRIMAGE";

        /// <summary>
        /// 接收可见光图像事件名称
        /// </summary>
        public static readonly string EVENT_SERVICE_RECEIVE_IMAGE = "EVENT_SERVICE_RECEIVE_IMAGE";

        /// <summary>
        /// 触发告警事件名称
        /// </summary>
        public static readonly string EVENT_SERVICE_ON_ALARM = "EVENT_SERVICE_ON_ALARM";

        /// <summary>
        /// 开启推流事件名称
        /// </summary>
        public static readonly string EVENT_SERVICE_START_STREAMING = "EVENT_SERVICE_START_STREAMING";

        /// <summary>
        /// ROI改变事件名称
        /// </summary>
        public static readonly string EVENT_SERVICE_ROI_CHANGED = "EVENT_SERVICE_ROI_CHANGED";

        /// <summary>
        /// 搜索人脸事件名称
        /// </summary>
        public static readonly string EVENT_SERVICE_FIND_FACE = "EVENT_SERVICE_FIND_FACE";
    }
}
