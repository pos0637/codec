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
            cell = arguments["cell"] as Configuration.Cell;

            if (!LoadSelections()) {
                return false;
            }

            return base.Initialize(arguments);
        }

        public override void Stop()
        {
            EventEmitter.Instance.Unsubscribe(Constants.EVENT_SERVICE_START_STREAMING, onStartLiveStreaming);
            CloseDevices();

            base.Stop();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnStart()
        {
            if (!LoadDevices()) {
                return;
            }

            onStartLiveStreaming = (args) => { StartLiveStreaming(args[0] as Dictionary<int, string>); };
            EventEmitter.Instance.Subscribe(Constants.EVENT_SERVICE_START_STREAMING, onStartLiveStreaming);

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

        #endregion
    }
}
