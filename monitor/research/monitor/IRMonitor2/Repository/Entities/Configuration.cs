﻿namespace Repository.Entities
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
            /// 红外摄像机设备参数
            /// </summary>
            public IrCameraDeviceParameters irCameraDeviceParameters;

            /// <summary>
            /// 可见光摄像机设备参数
            /// </summary>
            public CameraDeviceParameters cameraDeviceParameters;
        }

        /// <summary>
        /// 红外摄像机设备参数
        /// </summary>
        public class IrCameraDeviceParameters
        {
            /// <summary>
            /// IP地址
            /// </summary>
            public string ip;

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
            /// 温度帧率
            /// </summary>
            public int temperatureFrameRate;

            /// <summary>
            /// 辐射率
            /// </summary>
            public float? emissivity;

            /// <summary>
            /// 翻转表象温度
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
        /// 可见光摄像机设备参数
        /// </summary>
        public class CameraDeviceParameters
        {
            /// <summary>
            /// IP地址
            /// </summary>
            public string ip;

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
