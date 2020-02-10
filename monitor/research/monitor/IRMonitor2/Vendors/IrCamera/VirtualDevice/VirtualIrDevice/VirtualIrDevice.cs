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
        /// 宽度
        /// </summary>
        private const int mWidth = 384;

        /// <summary>
        /// 高度
        /// </summary>
        private const int mHeight = 288;

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
                    for (int y = 0, i = 0; y < mHeight; ++y) {
                        for (int x = 0; x < mWidth; ++x) {
                            dst[i++] = 0.0F;
                        }
                    }

                    return true;
                }

                case ReadMode.ImageArray: {
                    var dst = (byte[])inData;
                    for (int y = 0, i = 0; y < mHeight; ++y) {
                        for (int x = 0; x < mWidth; ++x) {
                            dst[i++] = 0;
                        }
                    }

                    return true;
                }

                default:
                    break;
            }

            return false;
        }

        public override bool Write(WriteMode mode, object data)
        {
            return true;
        }

        public override bool Control(ControlMode mode)
        {
            return true;
        }

        public override DeviceStatus GetDeviceStatus()
        {
            return status;
        }

        public override DeviceType GetDeviceType()
        {
            return DeviceType.IrCamera;
        }

        public override void Dispose()
        {
        }
    }
}
