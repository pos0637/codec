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
        private const Int32 mWidth = 384;

        /// <summary>
        /// 高度
        /// </summary>
        private const Int32 mHeight = 288;

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

        public override bool Read(ReadMode mode, IntPtr dataAddr, int bufferLen)
        {
            return false;
        }

        public override bool Read(ReadMode mode, object data, out int useLen)
        {
            useLen = 0;
            if (data == null)
                return false;

            switch (mode) {
                case ReadMode.ObjectDistance:
                case ReadMode.Emissivity:
                case ReadMode.AtmosphericTemperature:
                case ReadMode.RelativeHumidity:
                case ReadMode.Transmission:
                    data = 0.0F;
                    break;

                case ReadMode.TemperatureArray: {
                        float[] dst = (float[])data;
                        for (int y = 0, i = 0; y < mHeight; ++y) {
                            int startOffset = (mHeight - y - 1) * mWidth;
                            for (int x = 0; x < mWidth; ++x) {
                                dst[i++] = 0.0F;
                            }
                        }

                        return true;
                    }

                case ReadMode.ImageArray: {
                        byte[] dst = (byte[])data;
                        for (int y = 0, i = 0; y < mHeight; ++y) {
                            int startOffset = (mHeight - y - 1) * mWidth;
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
