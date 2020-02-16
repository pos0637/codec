namespace Repository.Entities
{
    /// <summary>
    /// 配置信息
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// 信息
        /// </summary>
        public class Information
        {
            /// <summary>
            /// 省
            /// </summary>
            public string province;

            /// <summary>
            /// 市
            /// </summary>
            public string city;

            /// <summary>
            /// 公司
            /// </summary>
            public string company;

            /// <summary>
            /// 负责人
            /// </summary>
            public string manager;

            /// <summary>
            /// 测试人员
            /// </summary>
            public string tester;

            /// <summary>
            /// 变电站
            /// </summary>
            public string substation;

            /// <summary>
            /// 设备位置
            /// </summary>
            public string location;

            /// <summary>
            /// 客户端索引
            /// </summary>
            public string clientId;

            /// <summary>
            /// MQTT服务器地址
            /// </summary>
            public string mqttServerIp;

            /// <summary>
            /// MQTT服务器端口
            /// </summary>
            public int mqttServerPort;

            /// <summary>
            /// RTMP服务器地址
            /// </summary>
            public string rtmpServerIp;

            /// <summary>
            /// RTMP服务器端口
            /// </summary>
            public int rtmpServerPort;

            /// <summary>
            /// 视频保存路径
            /// </summary>
            public string saveVideoPath;

            /// <summary>
            /// 图像保存路径
            /// </summary>
            public string saveImagePath;
        }

        /// <summary>
        /// 设备单元
        /// </summary>
        public class Cell
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string name;

            /// <summary>
            /// 设备列表
            /// </summary>
            public Device[] devices;
        }

        /// <summary>
        /// 设备
        /// </summary>
        public class Device
        {
            /// <summary>
            /// 类型
            /// </summary>
            public string category;

            /// <summary>
            /// 型号
            /// </summary>
            public string model;

            /// <summary>
            /// 版本
            /// </summary>
            public string version;

            /// <summary>
            /// 序列号
            /// </summary>
            public string serialNumber;

            /// <summary>
            /// 资源地址
            /// </summary>
            public string uri;

            /// <summary>
            /// 红外摄像机参数
            /// </summary>
            public IrCameraParameters irCameraParameters;

            /// <summary>
            /// 可见光摄像机参数
            /// </summary>
            public CameraParameters cameraParameters;
        }

        /// <summary>
        /// 红外摄像机参数
        /// </summary>
        public class IrCameraParameters
        {
            /// <summary>
            /// 资源地址
            /// </summary>
            public string uri;

            /// <summary>
            /// 图像宽度
            /// </summary>
            public int width;

            /// <summary>
            /// 图像对齐宽度
            /// </summary>
            public int stride;

            /// <summary>
            /// 高度
            /// </summary>
            public int height;

            /// <summary>
            /// 像素位宽
            /// </summary>
            public int bpp;

            /// <summary>
            /// 视频帧率
            /// </summary>
            public int videoFrameRate;

            /// <summary>
            /// 温度矩阵宽度
            /// </summary>
            public int temperatureWidth;

            /// <summary>
            /// 温度矩阵对齐宽度
            /// </summary>
            public int temperatureStride;

            /// <summary>
            /// 温度矩阵高度
            /// </summary>
            public int temperatureHeight;

            /// <summary>
            /// 温度帧率
            /// </summary>
            public int temperatureFrameRate;

            /// <summary>
            /// 辐射率
            /// </summary>
            public float? emissivity;

            /// <summary>
            /// 翻转表面温度
            /// </summary>
            public float? reflectedTemperature;

            /// <summary>
            /// 透过率
            /// </summary>
            public float? transmission;

            /// <summary>
            /// 大气温度
            /// </summary>
            public float? atmosphericTemperature;

            /// <summary>
            /// 相对湿度
            /// </summary>
            public float? relativeHumidity;

            /// <summary>
            /// 距离
            /// </summary>
            public float? distance;
        }

        /// <summary>
        /// 可见光摄像机参数
        /// </summary>
        public class CameraParameters
        {
            /// <summary>
            /// 资源地址
            /// </summary>
            public string uri;

            /// <summary>
            /// 图像宽度
            /// </summary>
            public int width;

            /// <summary>
            /// 图像对齐宽度
            /// </summary>
            public int stride;

            /// <summary>
            /// 高度
            /// </summary>
            public int height;

            /// <summary>
            /// 像素位宽
            /// </summary>
            public int bpp;

            /// <summary>
            /// 视频帧率
            /// </summary>
            public int videoFrameRate;
        }

        /// <summary>
        /// 信息
        /// </summary>
        public Information information;

        /// <summary>
        /// 设备单元列表
        /// </summary>
        public Cell[] cells;
    }
}
