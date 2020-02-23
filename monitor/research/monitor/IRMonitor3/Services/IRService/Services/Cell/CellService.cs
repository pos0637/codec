using IRService.Common;
using Miscs;
using Repository.Entities;
using System.Collections.Generic;
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
    }
}
