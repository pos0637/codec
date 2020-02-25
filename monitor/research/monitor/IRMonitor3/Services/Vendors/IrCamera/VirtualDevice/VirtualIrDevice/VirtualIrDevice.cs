using Devices;
using System;

namespace VirtualIrDevice
{
    public class VirtualIrDevice : IDevice
    {
        /// <summary>
        /// 设备状态
        /// </summary>
        private DeviceStatus status;

        /// <summary>
        /// 红外摄像机参数
        /// </summary>
        private Repository.Entities.Configuration.IrCameraParameters irCameraParameters;

        /// <summary>
        /// 可见光摄像机参数
        /// </summary>
        public Repository.Entities.Configuration.CameraParameters cameraParameters;

        public override bool Initialize()
        {
            status = DeviceStatus.Idle;
            return true;
        }

        public override bool Open()
        {
            status = DeviceStatus.Running;
            return true;
        }

        public override bool Close()
        {
            status = DeviceStatus.Idle;
            return true;
        }

        public override bool Read(ReadMode mode, IntPtr dataAddr, int bufferLength)
        {
            return false;
        }

        public override bool Read(ReadMode mode, in object inData, out object outData, out int used)
        {
            used = 0;
            outData = null;

            switch (mode) {
                case ReadMode.ObjectDistance:
                case ReadMode.Emissivity:
                case ReadMode.AtmosphericTemperature:
                case ReadMode.RelativeHumidity:
                case ReadMode.Transmission:
                    outData = 0.0F;
                    break;

                case ReadMode.TemperatureArray: {
                    var dst = (float[])inData;
                    for (int y = 0, i = 0; y < irCameraParameters.temperatureHeight; ++y) {
                        for (int x = 0; x < irCameraParameters.temperatureWidth; ++x) {
                            dst[i++] = 0.0F;
                        }
                    }

                    return true;
                }

                case ReadMode.IrImage: {
                    var dst = (byte[])inData;
                    for (int y = 0, i = 0; y < irCameraParameters.height; ++y) {
                        for (int x = 0; x < irCameraParameters.width; ++x) {
                            dst[i++] = 0;
                        }
                    }

                    return true;
                }

                case ReadMode.Image: {
                    var dst = (byte[])inData;
                    for (int y = 0, i = 0; y < cameraParameters.height; ++y) {
                        for (int x = 0; x < cameraParameters.width; ++x) {
                            dst[i++] = 0;
                        }
                    }

                    return true;
                }

                case ReadMode.IrCameraParameters: {
                    outData = irCameraParameters;
                    return true;
                }

                case ReadMode.CameraParameters: {
                    outData = cameraParameters;
                    return true;
                }

                default:
                    break;
            }

            return false;
        }

        public override bool Write(WriteMode mode, object data)
        {
            switch (mode) {
                case WriteMode.IrCameraParameters: {
                    irCameraParameters = data as Repository.Entities.Configuration.IrCameraParameters;
                    return true;
                }

                case WriteMode.CameraParameters: {
                    cameraParameters = data as Repository.Entities.Configuration.CameraParameters;
                    return true;
                }

                default:
                    break;
            }

            return true;
        }

        public override bool Control(ControlMode mode, object data)
        {
            return true;
        }

        public override DeviceStatus GetDeviceStatus()
        {
            return status;
        }

        public override DeviceCategory GetDeviceType()
        {
            return DeviceCategory.IrCamera;
        }

        public override void Dispose()
        {
        }
    }
}
