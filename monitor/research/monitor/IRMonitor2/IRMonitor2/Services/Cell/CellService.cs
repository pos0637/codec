using Common;
using Devices;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace IRMonitor2.Services.Cell
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
        private Configuration.Cell cell;

        /// <summary>
        /// 选区列表
        /// </summary>
        private readonly List<Models.Selections.Selection> selections = new List<Models.Selections.Selection>();

        /// <summary>
        /// 设备列表
        /// </summary>
        private readonly List<IDevice> devices = new List<IDevice>();

        /// <summary>
        /// 工作线程列表
        /// </summary>
        private readonly List<BaseWorker> workers = new List<BaseWorker>();

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
            CloseDevices();
            base.Stop();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnStart()
        {
            if (!LoadDevices()) {
                return;
            }

            base.OnStart();
        }

        #endregion
    }
}
