using Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace IRMonitor.Services.Cell.Worker
{
    /// <summary>
    /// 温度处理线程
    /// </summary>
    public class ProcessingWorker : BaseWorker
    {
        #region 委托

        public event Delegates.DgOnAddAlarmInfo OnAddAlarmInfo; // 添加告警信息
        public event Delegates.DgOnUpdateAlarmInfo OnUpdateAlarmInfo; // 更新告警信息
        public event Delegates.DgOnAlarmTimeout OnAlarmTimeout; // 告警超时
        public event Delegates.DgOnAddSelectionTemperature OnAddSelectionTemperature; // 添加选区历史数据
        public event Delegates.DgOnAddGroupSelectionTemperature OnAddGroupSelectionTemperature; // 添加选区组历史数据
        public event Delegates.DgOnRealtimeSelectionTemperature OnRealtimeSelectionTemperature; // 温度处理完回调
        public event Delegates.DgOnSendReportData OnSendReportData; // 定时报表回调
        public event Delegates.DgOnSendRealTemperature OnSendRealTemperature; // 定时发送实时温度

        #endregion

        #region 参数

        private int mMaxReadyAlarmNum = 3; // 预告警次数
        private FixedLenQueue<float[]> mTempertureQueue = new FixedLenQueue<float[]>(1); // 缓存队列
        private double mSelctionSampleRate; // 选区采样频率, 单位(min)
        private bool mNeedSampleSelction; // 是否需要添加选区历史数据
        private DateTime mNextSelectionSample; // 下一次添加选区数据的时间
        private double mGroupSampleRate; // 选区组采样频率, 单位(min)
        private bool mNeedSampleGroup; // 是否需要添加选区组历史数据
        private DateTime mNextGroupSample; // 下一次添加选区组数据的时间
        private List<Selection> mSelectionList; // 选区链表
        private List<GroupSelection> mSelectionGroupList; // 选区组链表
        private int mWidth; // 宽度
        private int mHeight; // 高度
        private float mMaxTemperature; // 全局最高温
        private float mMinTemperature; // 全局最低温
        private bool mSyncState = false; // 同步状态
        private object mSyncEvent = new object(); // 同步Event
        private int mTimeout = 5; // 5分钟超时

        private double mReportSendRate = 1; // 定时报表频率, 单位(h)
        private bool mNeedReportSend; // 是否需要发送报表
        private DateTime mNextReportSend; // 下一次发送报表的时间

        private double mRealTemperatureSendRate = 1; // 定时实时温度频率, 单位(min)
        private bool mNeedRealTemperatureSend; // 是否需要发送实时温度
        private DateTime mNextRealTemperatureSend; // 下一次发送实时温度的时间

        /// <summary>
        /// 结束告警误差范围
        /// </summary>
        private const float HIGH_ALARM_ERROR_RANGE = 5f;

        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="selectionList">选区列表</param>
        /// <param name="groupList">组选区列表</param>
        /// <returns></returns>
        public ARESULT Initialize(
            int width,
            int height,
            List<Selection> selectionList,
            List<GroupSelection> groupList)
        {
            mWidth = width;
            mHeight = height;
            mSelectionList = selectionList;
            mSelectionGroupList = groupList;
            mNextSelectionSample = DateTime.Now;
            mNextGroupSample = DateTime.Now;
            mNextReportSend = DateTime.Now;
            mNextRealTemperatureSend = DateTime.Now;

            return ARESULT.S_OK;
        }

        /// <summary>
        /// 接收温度数据
        /// </summary>
        public void ReceiveTempertureData(float[] buf)
        {
            mTempertureQueue.Enqueue(buf);
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void WaitFor()
        {
            bool ret = false;
            lock (mSyncEvent) {
                mSyncState = true;
                mTempertureQueue.Notify();
                ret = Monitor.TryEnter(mSyncEvent, 2000);
            }

            if (!ret) {
                mSyncState = false;
            }
        }

        /// <summary>
        /// 取消暂停
        /// </summary>
        public void CancelWaitFor()
        {
            lock (mSyncEvent) {
                mSyncState = false;
                Monitor.Exit(mSyncEvent);
            }
        }

        protected override void Run()
        {
            while (!IsTerminated()) {
                if (!mSyncState) {
                    if (mTempertureQueue.Count <= 0)
                        mTempertureQueue.Wait();
                }

                lock (mSyncEvent) {
                    if (mSyncState) {
                        Monitor.Exit(mSyncEvent);
                        if (Monitor.TryEnter(mSyncEvent, 2000))
                            mSyncState = false;
                        continue;
                    }
                }

                // 判断是否要采集数据
                var time = DateTime.Now;
                mNeedSampleSelction = false;
                mNeedSampleGroup = false;
                mNeedReportSend = false;
                mNeedRealTemperatureSend = false;
                if (time > mNextSelectionSample) {
                    mNeedSampleSelction = true;
                    mNextSelectionSample = time.AddMinutes(mSelctionSampleRate);
                }

                if (time > mNextGroupSample) {
                    mNeedSampleGroup = true;
                    mNextGroupSample = time.AddMinutes(mGroupSampleRate);
                }

                if (time > mNextReportSend) {
                    mNeedReportSend = true;
                    mNextReportSend = time.AddHours(mReportSendRate);
                }

                if (time > mNextRealTemperatureSend) {
                    mNeedRealTemperatureSend = true;
                    mNextRealTemperatureSend = time.AddMinutes(mRealTemperatureSendRate);
                }

                var buf = mTempertureQueue.Dequeue();
                if (buf == null) {
                    continue;
                }

                if (mSelectionList.Count <= 0) {
                    continue;
                }

                ProcessTemperature(buf);

                int count;
                lock (mSelectionList) {
                    count = mSelectionList.Count;
                }

                for (var i = 0; i < count; i++) {
                    try {
                        Selection selection;
                        lock (mSelectionList) {
                            selection = mSelectionList[i];
                        }

                        if (selection.mIsGlobalSelection) {
                            continue;
                        }

                        ProcessSeletionAlarm(selection);
                    }
                    catch {
                        continue;
                    }
                }

                lock (mSelectionGroupList) {
                    count = mSelectionGroupList.Count;
                }

                for (var i = 0; i < count; i++) {
                    try {
                        GroupSelection group;
                        lock (mSelectionGroupList) {
                            group = mSelectionGroupList[i];
                        }

                        ProcessGroupAlarm(group);
                    }
                    catch {
                        continue;
                    }
                }
            }
        }

        public override void Discard()
        {
            mSyncState = false; // 同步状态
            lock (mSyncEvent) {
                Monitor.PulseAll(mSyncEvent);
            }

            OnAddAlarmInfo = null;
            OnUpdateAlarmInfo = null;
            OnAddSelectionTemperature = null;
            OnAddGroupSelectionTemperature = null;
            OnRealtimeSelectionTemperature = null;
            mTempertureQueue.Notify();

            base.Discard();
        }

        /// <summary>
        /// 温度处理
        /// </summary>
        private void ProcessTemperature(float[] buf)
        {
            var time = DateTime.Now;
            var selectionData = "";
            var groupData = "";

            int count = 0;
            lock (mSelectionList) {
                count = mSelectionList.Count;
            }

            for (var i = 0; i < count; i++) {
                try {
                    Selection selection;
                    lock (mSelectionList) {
                        selection = mSelectionList[i];
                    }

                    if (ARESULT.AFAILED(selection.CalcSelectionAreaValue(buf, mWidth, mWidth, mHeight)))
                        continue;

                    if (selection.mIsGlobalSelection) {
                        mMaxTemperature = selection.mTemperatureData.mMaxTemperature;
                        mMinTemperature = selection.mTemperatureData.mMinTemperature;

                        if (mNeedRealTemperatureSend) {
                            OnSendRealTemperature?.Invoke(
                                time,
                                selection.mTemperatureData.mMaxTemperature,
                                selection.mTemperatureData.mMinTemperature,
                                selection.mTemperatureData.mAvgTemperature);
                        }
                    }

                    if (mNeedSampleSelction) { // 添加普通选区历史数据
                        OnAddSelectionTemperature?.Invoke(
                            selection.mSelectionId,
                            time,
                            selection.mTemperatureData.mMaxTemperature,
                            selection.mTemperatureData.mMinTemperature,
                            selection.mTemperatureData.mAvgTemperature);
                    }

                    if (mNeedReportSend && (!selection.mIsGlobalSelection)) {
                        OnSendReportData?.Invoke(selection.mSelectionId);
                    }

                    selectionData += JsonUtils.ObjectToJson(selection.mTemperatureData) + ",";
                }
                catch (Exception e) {
                    Tracker.LogE(e);
                    continue;
                }
            }

            count = 0;
            lock (mSelectionGroupList) {
                count = mSelectionGroupList.Count;
            }

            for (var i = 0; i < count; i++) {
                try {
                    GroupSelection group;
                    lock (mSelectionGroupList) {
                        group = mSelectionGroupList[i];
                    }

                    lock (mSelectionList) {
                        if (ARESULT.AFAILED(group.CalcSelectionAreaValue(mSelectionList)))
                            continue;
                    }

                    if (mNeedSampleGroup) { // 添加选区组历史数据
                        OnAddGroupSelectionTemperature?.Invoke(
                            group.mId,
                            time,
                            group.mTemperatureData.mMaxTemperature,
                            group.mTemperatureData.mTemperatureDif,
                            group.mTemperatureData.mTemperatureRise);
                    }
                    groupData += JsonUtils.ObjectToJson(group.mTemperatureData) + ",";
                }
                catch (Exception e) {
                    Tracker.LogE(e);
                    continue;
                }
            }

            // 选区温度信息实时回调
            if (!string.IsNullOrEmpty(selectionData))
                selectionData = string.Format("[{0}]", selectionData.Remove(selectionData.Length - 1, 1));

            if (!string.IsNullOrEmpty(groupData))
                groupData = string.Format("[{0}]", groupData.Remove(groupData.Length - 1, 1));

            OnRealtimeSelectionTemperature?.Invoke(selectionData, groupData);
        }

        /// <summary>
        /// 处理选区组告警
        /// </summary>
        private void ProcessGroupAlarm(GroupSelection group)
        {
            SubProcessGroupAlarm(GroupAlarmType.MaxTemperature, group); // 高温
            SubProcessGroupAlarm(GroupAlarmType.MaxTempRise, group); // 温升
            SubProcessGroupAlarm(GroupAlarmType.MaxTempDif, group); // 温差
            SubProcessGroupAlarm(GroupAlarmType.RelativeTempDif, group); // 相对温差
        }

        /// <summary>
        /// 单个告警处理
        /// </summary>
        private void SubProcessGroupAlarm(
            GroupAlarmType alarmType,
            GroupSelection group)
        {
            AlarmConfigData alarmSet;
            AlarmInfo info;
            float temperature;

            switch (alarmType) {
                case GroupAlarmType.MaxTemperature:
                    alarmSet = group.mAlarmConfig.mMaxTempConfig;
                    temperature = group.mTemperatureData.mMaxTemperature;
                    info = group.mAlarmInfo.mMaxTempAlarm;
                    break;

                case GroupAlarmType.MaxTempRise:
                    alarmSet = group.mAlarmConfig.mMaxTempeRiseConfig;
                    temperature = group.mTemperatureData.mTemperatureRise;
                    info = group.mAlarmInfo.mMaxTempRiseAlarmInfo;
                    break;

                case GroupAlarmType.MaxTempDif:
                    alarmSet = group.mAlarmConfig.mMaxTempDifConfig;
                    temperature = group.mTemperatureData.mTemperatureDif;
                    info = group.mAlarmInfo.mMaxTempDifAlarmInfo;
                    break;

                case GroupAlarmType.RelativeTempDif:
                    alarmSet = group.mAlarmConfig.mRelativeTempDifConfig;
                    temperature = group.mTemperatureData.mRelTemperatureDif;
                    info = group.mAlarmInfo.mRelativeTempDifAlarmInfo;
                    break;

                default:
                    return;
            }

            AlarmStatus lastStatus = info.mAlarmStatus;
            info.mLastAlarmLevel = info.mCurrentAlarmLevel;

            if ((alarmType == GroupAlarmType.MaxTemperature) || (alarmType == GroupAlarmType.RelativeTempDif))
                CalcAlarmInfo(alarmSet, temperature, info);
            else
                SpecialCalAlarmInfo(alarmSet, temperature, info); // 特殊处理组选区温升/温差

            if (alarmType == GroupAlarmType.MaxTemperature)
                ProcessAlarmReadyEnd(alarmSet, temperature, info);

            if (info.mAlarmStatus == AlarmStatus.AlarmStart) {
                info.mAlarmStatus = AlarmStatus.Alarming;

                // 添加告警
                string groupData = group.Serialize();
                string temperatureInfo = JsonUtils.ObjectToJson(group.mTemperatureData);

                // 阈值设置序列化
                string condition = string.Format("{0},{1},{2}", alarmSet.mGeneralThreshold, alarmSet.mSeriousThreshold, alarmSet.mCriticalThreshold);

                // 记录告警开始时间
                info.mBeginTime = DateTime.Now;
                info.mIsTimeout = false;

                // 告警入库
                OnAddAlarmInfo?.Invoke(
                    (int)AlarmMode.GroupSelection,
                    (int)alarmType,
                    (int)alarmSet.mAlarmReason,
                    (int)info.mCurrentAlarmLevel,
                    condition,
                    temperature,
                    group.mId,
                    group.mGroupName,
                    groupData,
                    temperatureInfo,
                    mMaxTemperature,
                    mMinTemperature,
                    group.mSelectionList,
                    info);
            }
            else if (info.mAlarmStatus == AlarmStatus.AlarmEnd) {
                info.mAlarmStatus = AlarmStatus.Unknown;

                // 结束告警
                string selectionData = group.Serialize();
                OnUpdateAlarmInfo?.Invoke(
                    (int)AlarmMode.GroupSelection,
                    (int)alarmType,
                    group.mId,
                    selectionData,
                    info);

                // 取消超时
                info.mIsTimeout = true;
            }
            else if (info.mAlarmStatus == AlarmStatus.AlarmingChanged) {

                // 结束上一次告警
                string selectionData = group.Serialize();
                OnUpdateAlarmInfo?.Invoke(
                    (int)AlarmMode.GroupSelection,
                    (int)alarmType,
                    group.mId,
                    selectionData,
                    info);

                string temperatureInfo = JsonUtils.ObjectToJson(group.mTemperatureData);

                // 阈值设置序列化
                string condition = string.Format("{0},{1},{2}", alarmSet.mGeneralThreshold, alarmSet.mSeriousThreshold, alarmSet.mCriticalThreshold);

                // 记录告警开始时间
                info.mBeginTime = DateTime.Now;
                info.mIsTimeout = false;
                info.mAlarmStatus = AlarmStatus.Alarming;

                OnAddAlarmInfo?.Invoke(
                    (int)AlarmMode.GroupSelection,
                    (int)alarmType,
                    (int)alarmSet.mAlarmReason,
                    (int)info.mCurrentAlarmLevel,
                    condition,
                    temperature,
                    group.mId,
                    group.mGroupName,
                    selectionData,
                    temperatureInfo,
                    mMaxTemperature,
                    mMinTemperature,
                    group.mSelectionList,
                    info);
            }
            else if ((info.mAlarmStatus == AlarmStatus.Alarming
                || info.mAlarmStatus == AlarmStatus.AlarmingReadyChanged)
                 && (info.mIsTimeout == false)) {
                DateTime time = DateTime.Now;
                if ((time - info.mBeginTime).TotalMinutes >= mTimeout) {
                    string selectionData = group.Serialize();
                    OnAlarmTimeout?.Invoke(
                        (int)AlarmMode.GroupSelection,
                        (int)alarmType,
                        group.mId,
                        selectionData,
                        info);

                    info.mIsTimeout = true;
                }
            }
        }

        /// <summary>
        /// 处理选区告警
        /// </summary>
        private void ProcessSeletionAlarm(Selection item)
        {
            SubProcessSeletionAlarm(SelectionAlarmType.MaxTemp, item); // 高温告警
            SubProcessSeletionAlarm(SelectionAlarmType.MinTemp, item); // 低温告警
            SubProcessSeletionAlarm(SelectionAlarmType.AvgTemp, item); // 平均温告警
        }

        /// <summary>
        /// 单个告警处理
        /// </summary>
        private void SubProcessSeletionAlarm(SelectionAlarmType alarmType, Selection select)
        {
            AlarmConfigData alarmSet;
            AlarmInfo info;
            float temperature;

            switch (alarmType) {
                case SelectionAlarmType.MaxTemp:
                    alarmSet = select.mAlarmConfig.mMaxTempConfig;
                    temperature = select.mTemperatureData.mMaxTemperature;
                    info = select.mAlarmInfo.mMaxTempAlarmInfo;
                    break;

                case SelectionAlarmType.MinTemp:
                    alarmSet = select.mAlarmConfig.mMinTempConfig;
                    temperature = select.mTemperatureData.mMinTemperature;
                    info = select.mAlarmInfo.mMinTempAlarmInfo;
                    break;

                case SelectionAlarmType.AvgTemp:
                    alarmSet = select.mAlarmConfig.mAvgTempConfig;
                    temperature = select.mTemperatureData.mAvgTemperature;
                    info = select.mAlarmInfo.mAvgTempAlarmInfo;
                    break;

                default:
                    return;
            }

            AlarmStatus lastStatus = info.mAlarmStatus;
            info.mLastAlarmLevel = info.mCurrentAlarmLevel;

            CalcAlarmInfo(alarmSet, temperature, info);
            ProcessAlarmReadyEnd(alarmSet, temperature, info);

            if (info.mAlarmStatus == AlarmStatus.AlarmStart) {
                info.mAlarmStatus = AlarmStatus.Alarming;

                // 添加告警
                string selectionData = select.Serialize();
                string temperatureInfo = JsonUtils.ObjectToJson(select.mTemperatureData);

                // 阈值设置序列化
                string condition = string.Format("{0},{1},{2}", alarmSet.mGeneralThreshold,
                        alarmSet.mSeriousThreshold, alarmSet.mCriticalThreshold);

                List<Selection> list = new List<Selection>() { select };

                // 记录告警开始时间
                info.mBeginTime = DateTime.Now;
                info.mIsTimeout = false;

                // 告警入库
                OnAddAlarmInfo?.Invoke(
                    (int)AlarmMode.Selection,
                    (int)alarmType,
                    (int)alarmSet.mAlarmReason,
                    (int)info.mCurrentAlarmLevel,
                    condition,
                    temperature,
                    select.mSelectionId,
                    select.mSelectionName,
                    selectionData,
                    temperatureInfo,
                    mMaxTemperature,
                    mMinTemperature,
                    list,
                    info);

            }
            else if (info.mAlarmStatus == AlarmStatus.AlarmEnd) {
                info.mAlarmStatus = AlarmStatus.Unknown;

                // 结束告警
                string selectionData = select.Serialize();
                OnUpdateAlarmInfo?.Invoke(
                    (int)AlarmMode.Selection,
                    (int)alarmType,
                    select.mSelectionId,
                    selectionData,
                    info);

                // 取消超时
                info.mIsTimeout = true;
            }
            else if (info.mAlarmStatus == AlarmStatus.AlarmingChanged) {

                // 结束上一次告警
                string selectionData = select.Serialize();
                OnUpdateAlarmInfo?.Invoke(
                    (int)AlarmMode.Selection,
                    (int)alarmType,
                    select.mSelectionId,
                    selectionData,
                    info);

                string temperatureInfo = JsonUtils.ObjectToJson(select.mTemperatureData);
                List<Selection> list = new List<Selection>() { select };

                // 阈值设置序列化
                string condition = string.Format("{0},{1},{2}", alarmSet.mGeneralThreshold,
                    alarmSet.mSeriousThreshold, alarmSet.mCriticalThreshold);

                // 记录告警开始时间
                info.mBeginTime = DateTime.Now;
                info.mIsTimeout = false;
                info.mAlarmStatus = AlarmStatus.Alarming;

                OnAddAlarmInfo?.Invoke(
                    (int)AlarmMode.Selection,
                    (int)alarmType,
                    (int)alarmSet.mAlarmReason,
                    (int)info.mCurrentAlarmLevel,
                    condition,
                    temperature,
                    select.mSelectionId,
                    select.mSelectionName,
                    selectionData,
                    temperatureInfo,
                    mMaxTemperature,
                    mMinTemperature,
                    list,
                    info);
            }
            else if (((info.mAlarmStatus == AlarmStatus.Alarming)
                || (info.mAlarmStatus == AlarmStatus.AlarmingReadyChanged))
                && (info.mIsTimeout == false)) {
                DateTime time = DateTime.Now;
                if ((time - info.mBeginTime).TotalMinutes >= mTimeout) {
                    string selectionData = select.Serialize();
                    OnAlarmTimeout?.Invoke(
                        (int)AlarmMode.Selection,
                        (int)alarmType,
                        select.mSelectionId,
                        selectionData,
                        info);

                    info.mIsTimeout = true;
                }
            }
        }

        /// <summary>
        /// 计算告警信息
        /// </summary>
        private void CalcAlarmInfo(
            AlarmConfigData set,
            float temperture,
            AlarmInfo info)
        {
            AlarmLevel level = AlarmLevel.Unknown;

            if (set.mAlarmReason == AlarmReason.High) {
                if (temperture >= set.mCriticalThreshold)
                    level = AlarmLevel.Critical;
                else if (temperture >= set.mSeriousThreshold)
                    level = AlarmLevel.Serious;
                else if (temperture >= set.mGeneralThreshold)
                    level = AlarmLevel.General;
                else
                    level = AlarmLevel.Unknown;
            }
            else if (set.mAlarmReason == AlarmReason.Low) {
                if (temperture <= set.mCriticalThreshold)
                    level = AlarmLevel.Critical;
                else if (temperture <= set.mSeriousThreshold)
                    level = AlarmLevel.Serious;
                else if (temperture <= set.mGeneralThreshold)
                    level = AlarmLevel.General;
                else
                    level = AlarmLevel.Unknown;
            }

            if (level > AlarmLevel.Unknown) {
                if (info.mAlarmStatus == AlarmStatus.Unknown) {
                    // 进入告警开始延迟状态
                    info.mAlarmStatus = AlarmStatus.AlarmReadyStart;
                    info.mReadyAlarmNum = 0;
                }
                else if (info.mAlarmStatus == AlarmStatus.AlarmReadyEnd) {
                    // 告警结束延迟未结束时, 告警等级不为正常, 则切换至告警进行时状态
                    info.mAlarmStatus = AlarmStatus.Alarming;
                    info.mReadyAlarmNum = 0;
                }
                else if (info.mAlarmStatus == AlarmStatus.AlarmReadyStart) {
                    // 告警开始延迟, 计数
                    info.mReadyAlarmNum++;
                    if (info.mReadyAlarmNum >= mMaxReadyAlarmNum)
                        info.mAlarmStatus = AlarmStatus.AlarmStart;
                }
                else if ((info.mAlarmStatus == AlarmStatus.Alarming)
                    && (info.mCurrentAlarmLevel != level)) {
                    // 告警等级准备改变
                    info.mAlarmingLevel = info.mCurrentAlarmLevel;
                    info.mAlarmStatus = AlarmStatus.AlarmingReadyChanged;
                    info.mReadyAlarmNum = 0;
                }
                else if (info.mAlarmStatus == AlarmStatus.AlarmingReadyChanged) {
                    if (info.mCurrentAlarmLevel == level) {
                        // 告警等级改变
                        info.mReadyAlarmNum++;
                        if (info.mReadyAlarmNum >= mMaxReadyAlarmNum)
                            info.mAlarmStatus = AlarmStatus.AlarmingChanged;
                    }
                    else if (info.mAlarmingLevel == level) {
                        info.mAlarmStatus = AlarmStatus.Alarming;
                        info.mReadyAlarmNum = 0;
                    }
                    else {
                        info.mReadyAlarmNum = 0;
                    }
                }
            }
            else {
                if ((info.mAlarmStatus == AlarmStatus.Alarming)
                    || (info.mAlarmStatus == AlarmStatus.AlarmingReadyChanged)) {
                    // 进入告警结束延迟状态
                    info.mAlarmStatus = AlarmStatus.AlarmReadyEnd;
                    info.mReadyAlarmNum = 0;
                }
                else if (info.mAlarmStatus == AlarmStatus.AlarmReadyStart) {
                    // 告警开始延迟未结束时, 告警等级为正常, 则切换至正常状态
                    info.mAlarmStatus = AlarmStatus.Unknown;
                    info.mReadyAlarmNum = 0;
                }
                else if (info.mAlarmStatus == AlarmStatus.AlarmReadyEnd) {
                    // 告警结束延迟, 计数
                    info.mReadyAlarmNum++;
                    if (info.mReadyAlarmNum >= mMaxReadyAlarmNum)
                        info.mAlarmStatus = AlarmStatus.AlarmEnd;
                }
            }

            info.mCurrentAlarmLevel = level;
        }

        /// <summary>
        /// 计算组选区温升/温差告警信息
        /// </summary>
        private void SpecialCalAlarmInfo(
            AlarmConfigData set,
            float temperture,
            AlarmInfo info)
        {
            AlarmLevel level = AlarmLevel.Unknown;

            if (set.mAlarmReason == AlarmReason.High) {
                if (temperture >= set.mCriticalThreshold)
                    level = AlarmLevel.Critical;
                else if (temperture >= set.mSeriousThreshold)
                    level = AlarmLevel.Serious;
                else if (temperture >= set.mGeneralThreshold)
                    level = AlarmLevel.General;
                else if (temperture < 0)
                    level = AlarmLevel.General;
                else
                    level = AlarmLevel.Unknown;
            }
            else if (set.mAlarmReason == AlarmReason.Low) {
                if (temperture <= set.mCriticalThreshold)
                    level = AlarmLevel.Critical;
                else if (temperture <= set.mSeriousThreshold)
                    level = AlarmLevel.Serious;
                else if (temperture <= set.mGeneralThreshold)
                    level = AlarmLevel.General;
                else if (temperture < 0)
                    level = AlarmLevel.General;
                else
                    level = AlarmLevel.Unknown;
            }

            if (level > AlarmLevel.Unknown) {

                if (info.mAlarmStatus == AlarmStatus.Unknown) {
                    // 进入告警开始延迟状态
                    info.mAlarmStatus = AlarmStatus.AlarmReadyStart;
                    info.mReadyAlarmNum = 0;
                }
                else if (info.mAlarmStatus == AlarmStatus.AlarmReadyEnd) {
                    // 告警结束延迟未结束时, 告警等级不为正常, 则切换至告警进行时状态
                    info.mAlarmStatus = AlarmStatus.Alarming;
                    info.mReadyAlarmNum = 0;
                }
                else if (info.mAlarmStatus == AlarmStatus.AlarmReadyStart) {
                    // 告警开始延迟, 计数
                    info.mReadyAlarmNum++;
                    if (info.mReadyAlarmNum >= mMaxReadyAlarmNum)
                        info.mAlarmStatus = AlarmStatus.AlarmStart;
                }
                else if ((info.mAlarmStatus == AlarmStatus.Alarming)
                    && (info.mCurrentAlarmLevel != level)) {
                    // 告警等级准备改变
                    info.mAlarmingLevel = info.mCurrentAlarmLevel;
                    info.mAlarmStatus = AlarmStatus.AlarmingReadyChanged;
                    info.mReadyAlarmNum = 0;
                }
                else if (info.mAlarmStatus == AlarmStatus.AlarmingReadyChanged) {
                    if (info.mCurrentAlarmLevel == level) {
                        // 告警等级改变
                        info.mReadyAlarmNum++;
                        if (info.mReadyAlarmNum >= mMaxReadyAlarmNum)
                            info.mAlarmStatus = AlarmStatus.AlarmingChanged;
                    }
                    else if (info.mAlarmingLevel == level) {
                        info.mAlarmStatus = AlarmStatus.Alarming;
                        info.mReadyAlarmNum = 0;
                    }
                    else {
                        info.mReadyAlarmNum = 0;
                    }
                }
            }
            else {
                if ((info.mAlarmStatus == AlarmStatus.Alarming)
                    || (info.mAlarmStatus == AlarmStatus.AlarmingReadyChanged)) {
                    // 进入告警结束延迟状态
                    info.mAlarmStatus = AlarmStatus.AlarmReadyEnd;
                    info.mReadyAlarmNum = 0;
                }
                else if (info.mAlarmStatus == AlarmStatus.AlarmReadyStart) {
                    // 告警开始延迟未结束时, 告警等级为正常, 则切换至正常状态
                    info.mAlarmStatus = AlarmStatus.Unknown;
                    info.mReadyAlarmNum = 0;
                }
                else if (info.mAlarmStatus == AlarmStatus.AlarmReadyEnd) {
                    // 告警结束延迟, 计数
                    info.mReadyAlarmNum++;
                    if (info.mReadyAlarmNum >= mMaxReadyAlarmNum)
                        info.mAlarmStatus = AlarmStatus.AlarmEnd;
                }
            }

            info.mCurrentAlarmLevel = level;
        }

        /// <summary>
        /// 处理延迟结束告警
        /// </summary>
        private void ProcessAlarmReadyEnd(
            AlarmConfigData set,
            float temperture,
            AlarmInfo info)
        {
            if (info.mAlarmStatus == AlarmStatus.AlarmReadyEnd) {
                if (set.mAlarmReason == AlarmReason.High) {
                    AlarmLevel level;
                    if (temperture >= (set.mCriticalThreshold - HIGH_ALARM_ERROR_RANGE))
                        level = AlarmLevel.Critical;
                    else if (temperture >= (set.mSeriousThreshold - HIGH_ALARM_ERROR_RANGE))
                        level = AlarmLevel.Serious;
                    else if (temperture >= (set.mGeneralThreshold - HIGH_ALARM_ERROR_RANGE))
                        level = AlarmLevel.General;
                    else
                        level = AlarmLevel.Unknown;

                    if (level > AlarmLevel.Unknown) {
                        info.mAlarmStatus = AlarmStatus.Alarming;
                        info.mCurrentAlarmLevel = info.mLastAlarmLevel;
                    }
                }
            }
        }

        /// <summary>
        /// 设置选区采样频率
        /// </summary>
        public void SetSelctionSampleRate(int rate)
        {
            mSelctionSampleRate = rate;
        }

        /// <summary>
        /// 设置选区组采样频率
        /// </summary>
        public void SetGroupSelctionSampleRate(int rate)
        {
            mGroupSampleRate = rate;
        }
    }
}
