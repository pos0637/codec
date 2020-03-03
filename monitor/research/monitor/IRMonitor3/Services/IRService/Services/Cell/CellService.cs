using Devices;
using IRService.Common;
using Miscs;
using Repository.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace IRService.Services.Cell
{
    /// <summary>
    /// 设备单元服务
    /// </summary>
    public partial class CellService : Service
    {
        #region 成员变量

        /// <summary>
        /// 配置信息
        /// </summary>
        public Configuration configuration;

        /// <summary>
        /// 设备单元配置
        /// </summary>
        public Configuration.Cell cell;

        #endregion

        #region 接口方法

        public override void Dispose()
        {
            CloseDevices();
            base.Dispose();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override bool Initialize(Dictionary<string, object> arguments)
        {
            configuration = Repository.Repository.LoadConfiguation();
            cell = arguments["cell"] as Configuration.Cell;

            if (!LoadSelections()) {
                return false;
            }

            return base.Initialize(arguments);
        }

        public override void Stop()
        {
            if (configuration.information.onlineMode) {
                EventEmitter.Instance.Unsubscribe(Constants.EVENT_SERVICE_START_STREAMING, onStartLiveStreaming);
            }

            CloseDevices();

            base.Stop();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnStart()
        {
            if (!LoadDevices()) {
                return;
            }

            if (configuration.information.recordingMode) {
                if (!StartRecording()) {
                    return;
                }
            }

            if (configuration.information.onlineMode) {
                onStartLiveStreaming = (args) => { StartLiveStreaming(args[0] as Dictionary<int, string>); };
                EventEmitter.Instance.Subscribe(Constants.EVENT_SERVICE_START_STREAMING, onStartLiveStreaming);
            }

            base.OnStart();
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 获取调色板模式
        /// </summary>
        /// <param name="deviceId">设备索引</param>
        /// <returns>调色板模式</returns>
        public string GetPaletteMode(string deviceId)
        {
            var device = deviceId != null ? devices.FirstOrDefault(d => d.Id.Equals(deviceId)) : devices.Count > 0 ? devices[0] : null;
            if (device == null) {
                return null;
            }

            if (!device.Read(ReadMode.PaletteMode, null, out object result, out int used)) {
                return null;
            }

            return result.ToString();
        }

        /// <summary>
        /// 设置调色板模式
        /// </summary>
        /// <param name="deviceId">设备索引</param>
        /// <param name="mode">调色板模式</param>
        /// <returns>是否成功</returns>
        public bool SetPaletteMode(string deviceId, string mode)
        {
            var device = deviceId != null ? devices.FirstOrDefault(d => d.Id.Equals(deviceId)) : devices.Count > 0 ? devices[0] : null;
            if (device == null) {
                return false;
            }

            if (!device.Write(WriteMode.PaletteMode, mode)) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取人脸测温配置规则
        /// </summary>
        /// <param name="deviceId">设备索引</param>
        /// <returns>人脸测温配置规则</returns>
        public Dictionary<string, object> GetFaceThermometryRegion(string deviceId)
        {
            var device = deviceId != null ? devices.FirstOrDefault(d => d.Id.Equals(deviceId)) : devices.Count > 0 ? devices[0] : null;
            if (device == null) {
                return null;
            }

            if (!device.Read(ReadMode.FaceThermometryRegion, null, out object result, out int used)) {
                return null;
            }

            return result as Dictionary<string, object>;
        }

        /// <summary>
        /// 设置人脸测温配置规则
        /// </summary>
        ///  <param name="deviceId">设备索引</param>
        /// <param name="arguments">人脸测温配置规则</param>
        /// <returns>是否成功</returns>
        public bool SetFaceThermometryRegion(string deviceId, Dictionary<string, object> arguments)
        {
            var device = deviceId != null ? devices.FirstOrDefault(d => d.Id.Equals(deviceId)) : devices.Count > 0 ? devices[0] : null;
            if (device == null) {
                return false;
            }

            if (!device.Write(WriteMode.FaceThermometryRegion, arguments)) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取人脸测温基本参数配置
        /// </summary>
        /// <param name="deviceId">设备索引</param> 
        /// <returns>人脸测温基本参数配置</returns>
        public Dictionary<string, object> GetFaceThermometryBasicParameter(string deviceId)
        {
            var device = deviceId != null ? devices.FirstOrDefault(d => d.Id.Equals(deviceId)) : devices.Count > 0 ? devices[0] : null;
            if (device == null) {
                return null;
            }

            if (!device.Read(ReadMode.FaceThermometryBasicParameter, null, out object result, out int used)) {
                return null;
            }

            return result as Dictionary<string, object>;
        }

        /// <summary>
        /// 设置人脸测温基本参数配置
        /// </summary>
        /// <param name="deviceId">设备索引</param> 
        /// <param name="arguments">人脸测温基本参数配置</param>
        /// <returns>是否成功</returns>
        public bool SetFaceThermometryBasicParameter(string deviceId, Dictionary<string, object> arguments)
        {
            var device = deviceId != null ? devices.FirstOrDefault(d => d.Id.Equals(deviceId)) : devices.Count > 0 ? devices[0] : null;
            if (device == null) {
                return false;
            }

            if (!device.Write(WriteMode.FaceThermometryBasicParameter, arguments)) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取体温温度补偿配置
        /// </summary>
        /// <param name="deviceId">设备索引</param> 
        /// <returns>体温温度补偿配置</returns>
        public Dictionary<string, object> GetBodyTemperatureCompensation(string deviceId)
        {
            var device = deviceId != null ? devices.FirstOrDefault(d => d.Id.Equals(deviceId)) : devices.Count > 0 ? devices[0] : null;
            if (device == null) {
                return null;
            }

            if (!device.Read(ReadMode.BodyTemperatureCompensation, null, out object result, out int used)) {
                return null;
            }

            return result as Dictionary<string, object>;
        }

        /// <summary>
        /// 设置体温温度补偿配置
        /// </summary>
        /// <param name="deviceId">设备索引</param> 
        /// <param name="arguments">体温温度补偿配置</param>
        /// <returns>是否成功</returns>
        public bool SetBodyTemperatureCompensation(string deviceId, Dictionary<string, object> arguments)
        {
            var device = deviceId != null ? devices.FirstOrDefault(d => d.Id.Equals(deviceId)) : devices.Count > 0 ? devices[0] : null;
            if (device == null) {
                return false;
            }

            if (!device.Write(WriteMode.BodyTemperatureCompensation, arguments)) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取黑体配置
        /// </summary>
        /// <param name="deviceId">设备索引</param> 
        /// <returns>黑体配置</returns>
        public Dictionary<string, object> GetBlackBody(string deviceId)
        {
            var device = deviceId != null ? devices.FirstOrDefault(d => d.Id.Equals(deviceId)) : devices.Count > 0 ? devices[0] : null;
            if (device == null) {
                return null;
            }

            if (!device.Read(ReadMode.BlackBody, null, out object result, out int used)) {
                return null;
            }

            return result as Dictionary<string, object>;
        }

        /// <summary>
        /// 设置黑体配置
        /// </summary>
        /// <param name="deviceId">设备索引</param> 
        /// <param name="arguments">黑体配置</param>
        /// <returns>是否成功</returns>
        public bool SetBlackBody(string deviceId, Dictionary<string, object> arguments)
        {
            var device = deviceId != null ? devices.FirstOrDefault(d => d.Id.Equals(deviceId)) : devices.Count > 0 ? devices[0] : null;
            if (device == null) {
                return false;
            }

            if (!device.Write(WriteMode.BlackBody, arguments)) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取镜像模式
        /// </summary>
        /// <param name="deviceId">设备索引</param> 
        /// <returns>镜像模式</returns>
        public bool GetMirrorMode(string deviceId)
        {
            var device = deviceId != null ? devices.FirstOrDefault(d => d.Id.Equals(deviceId)) : devices.Count > 0 ? devices[0] : null;
            if (device == null) {
                return false;
            }

            if (!device.Read(ReadMode.MirrorMode, null, out object result, out int used)) {
                return false;
            }

            return (bool)result;
        }

        /// <summary>
        /// 设置镜像模式
        /// </summary>
        /// <param name="deviceId">设备索引</param> 
        /// <param name="arguments">镜像模式</param>
        /// <returns>是否成功</returns>
        public bool SetMirrorMode(string deviceId, bool mode)
        {
            var device = deviceId != null ? devices.FirstOrDefault(d => d.Id.Equals(deviceId)) : devices.Count > 0 ? devices[0] : null;
            if (device == null) {
                return false;
            }

            if (!device.Write(WriteMode.MirrorMode, mode)) {
                return false;
            }

            return true;
        }
        #endregion
    }
}
