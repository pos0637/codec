using Common;
using IRMonitor.Common;
using Newtonsoft.Json;
using Repository.DAO;
using System;

namespace IRMonitor.Services.Cell
{
    /// <summary>
    /// 设备单元服务_选区操作
    /// </summary>
    public partial class CellService
    {
        /// <summary>
        /// 添加选区
        /// </summary>
        /// <param name="data">选区信息</param>
        /// <param name="id">选区索引</param>
        /// <returns>是否成功</returns>
        public ARESULT AddSelection(string data, ref long id)
        {
            var type = Selection.GetType(data);
            if (type == SelectionType.Unknown) {
                return ARESULT.E_INVALIDARG;
            }

            var selection = CreateSelection(type);
            if (ARESULT.AFAILED(selection.Deserialize(data))) {
                return ARESULT.E_INVALIDARG;
            }

            if (ARESULT.AFAILED(SelectionDAO.AddSelection(mCell.mCellId, data, selection.mIsGlobalSelection, ref id))) {
                return ARESULT.E_FAIL;
            }

            lock (mSelections) {
                mSelections.Add(selection.SetId(id));
            }

            return ARESULT.S_OK;
        }

        /// <summary>
        /// 更新选区
        /// </summary>
        /// <param name="data">选区信息</param>
        /// <returns>是否成功</returns>
        public ARESULT UpdateSelection(string data)
        {
            SelectionUpdateParam update = JsonUtils.ObjectFromJson<SelectionUpdateParam>(data);
            if (update == null) {
                return ARESULT.E_FAIL;
            }

            try {
                // 取出普通选区
                Selection selection = GetSelection(update.mId);
                if (selection == null) {
                    return ARESULT.E_FAIL;
                }

                lock (selection) {
                    // 判断选区是否改变
                    if (selection.Serialize().Equals(update.mData)) {
                        return ARESULT.S_OK;
                    }

                    // 清除选区告警
                    ClearSelectionAlarm(selection);

                    // 反序列化
                    if (ARESULT.ASUCCEEDED(selection.Deserialize(update.mData))) {
                        if (selection.mSelectionId != update.mId) {
                            return ARESULT.E_NOIMPL;
                        }
                    }
                    else {
                        return ARESULT.E_INVALIDARG;
                    }
                }

                // 判断普通选区是否关联选区组
                lock (mSelectionGroups) {
                    mSelectionGroups.ForEach(group => {
                        // 清除选区组告警
                        if (group.InSelections(selection.mSelectionId)) {
                            ClearSelectionGroupAlarm(group);
                        }
                    });
                }

                return SelectionDAO.UpdateSelection(update.mId, update.mData);
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return ARESULT.E_FAIL;
            }
        }

        /// <summary>
        /// 删除选区
        /// </summary>
        /// <param name="id">选区索引</param>
        /// <returns>是否成功</returns>
        public ARESULT RemoveSelection(long id)
        {
            try {
                if (ARESULT.AFAILED(SelectionDAO.RemoveSelection(id))) {
                    return ARESULT.E_FAIL;
                }

                bool continueFlag = false;
                lock (mSelectionGroups) {
                    foreach (SelectionGroup groupSelection in mSelectionGroups) {
                        lock (groupSelection) {
                            for (int i = 0; i < groupSelection.mSelectionIds.Count; i++) {
                                if (groupSelection.mSelectionIds[i] == id) {
                                    ClearSelectionGroupAlarm(groupSelection);

                                    if (groupSelection.mSelectionIds.Count == 1) {
                                        GroupSelectionDAO.RemoveGroupSelection(groupSelection.mId);
                                        mSelectionGroups.Remove(groupSelection);
                                    }
                                    else {
                                        for (int k = i; k < groupSelection.mSelectionIds.Count - 1; k++) {
                                            groupSelection.mSelectionIds[k] = groupSelection.mSelectionIds[k + 1];
                                        }

                                        string data = groupSelection.Serialize();
                                        if (data != null) {
                                            GroupSelectionDAO.UpdateGroupSelection(groupSelection.mId, data);
                                        }
                                    }

                                    continueFlag = true;
                                    break;
                                }
                            }
                        }

                        if (continueFlag) {
                            break;
                        }
                    }
                }

                lock (mSelections) {
                    foreach (Selection selection in mSelections) {
                        lock (selection) {
                            if (selection.mSelectionId == id) {
                                ClearSelectionAlarm(selection);
                                mSelections.Remove(selection);
                                return ARESULT.S_OK;
                            }
                        }
                    }
                }

                return ARESULT.E_FAIL;
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return ARESULT.E_FAIL;
            }
        }

        /// <summary>
        /// 更新选区告警配置
        /// </summary>
        public ARESULT UpdateSelectionConfig(string data)
        {
            SelectionConfigUpdate config = JsonUtils.ObjectFromJson<SelectionConfigUpdate>(data);
            if (config == null) {
                return ARESULT.E_FAIL;
            }

            SelectionAlarmConfig alramconfig = JsonUtils.ObjectFromJson<SelectionAlarmConfig>(config.mData);
            if (config == null) {
                return ARESULT.E_FAIL;
            }

            Selection selection = null;
            lock (mSelections) {
                foreach (Selection item in mSelections) {
                    if (item.mSelectionId == config.mId) {
                        selection = item;
                        break;
                    }
                }
            }

            if (selection == null) {
                return ARESULT.E_FAIL;
            }

            #region 更新告警设置

            lock (selection) {
                selection.mSelectionName = config.mName;

                // 暂停温度处理线程
                mProcessingWorker.Pause();

                if (!selection.mAlarmConfig.mMaxTempConfig.Equals(alramconfig.mMaxTempConfig)) {
                    if (selection.mAlarmInfo.mMaxTempAlarmInfo.mAlarmStatus == AlarmStatus.Alarming) {
                        UpdateAlarmInfo(
                            (int)AlarmMode.Selection,
                            (int)SelectionAlarmType.MaxTemp,
                            selection.mSelectionId,
                            selection.Serialize(),
                            selection.mAlarmInfo.mMaxTempAlarmInfo);
                    }

                    selection.mAlarmConfig.mMaxTempConfig = alramconfig.mMaxTempConfig;
                    selection.mAlarmInfo.mMaxTempAlarmInfo.Reset();
                }

                if (!selection.mAlarmConfig.mMinTempConfig.Equals(alramconfig.mMinTempConfig)) {
                    if (selection.mAlarmInfo.mMinTempAlarmInfo.mAlarmStatus == AlarmStatus.Alarming) {
                        UpdateAlarmInfo(
                            (int)AlarmMode.Selection,
                            (int)SelectionAlarmType.MinTemp,
                            selection.mSelectionId,
                            selection.Serialize(),
                            selection.mAlarmInfo.mMinTempAlarmInfo);
                    }

                    selection.mAlarmConfig.mMinTempConfig = alramconfig.mMinTempConfig;
                    selection.mAlarmInfo.mMinTempAlarmInfo.Reset();
                }

                if (!selection.mAlarmConfig.mAvgTempConfig.Equals(alramconfig.mAvgTempConfig)) {
                    if (selection.mAlarmInfo.mAvgTempAlarmInfo.mAlarmStatus == AlarmStatus.Alarming) {
                        UpdateAlarmInfo(
                            (int)AlarmMode.Selection,
                            (int)SelectionAlarmType.AvgTemp,
                            selection.mSelectionId,
                            selection.Serialize(),
                            selection.mAlarmInfo.mAvgTempAlarmInfo);
                    }

                    selection.mAlarmConfig.mAvgTempConfig = alramconfig.mAvgTempConfig;
                    selection.mAlarmInfo.mAvgTempAlarmInfo.Reset();
                }

                // 取消暂停
                mProcessingWorker.Resume();
            }

            #endregion

            return SelectionDAO.UpdateSelection(selection.mSelectionId, selection.Serialize());
        }

        /// <summary>
        /// 添加组选区
        /// </summary>
        public ARESULT AddNewGroupSelection(string data, ref long id)
        {
            SelectionGroup groupSelection = new SelectionGroup();
            if (groupSelection == null)
                return ARESULT.E_OUTOFMEMORY;

            if (ARESULT.AFAILED(groupSelection.Deserialize(data)))
                return ARESULT.E_INVALIDARG;

            long groupSelectionId = -1;
            if (ARESULT.AFAILED(GroupSelectionDAO.AddNewGroupSelection(mCell.mCellId, data, ref groupSelectionId)))
                return ARESULT.E_FAIL;

            groupSelection.mId = groupSelectionId;
            groupSelection.mTemperatureData.mGroupId = groupSelectionId;
            id = groupSelectionId;

            lock (mSelectionGroups) {
                mSelectionGroups.Add(groupSelection);
            }

            return ARESULT.S_OK;
        }

        /// <summary>
        /// 更新组选区
        /// </summary>
        public ARESULT UpdateGroupSelectionConfig(string data)
        {
            GroupSelectionUpdateParam gupdate = JsonUtils.ObjectFromJson<GroupSelectionUpdateParam>(data);
            if (gupdate == null)
                return ARESULT.E_FAIL;

            GroupAlarmConfig alramconfig = JsonUtils.ObjectFromJson<GroupAlarmConfig>(gupdate.mData);
            if (alramconfig == null)
                return ARESULT.E_FAIL;

            SelectionGroup selectionGroup = null;
            lock (mSelectionGroups) {
                foreach (SelectionGroup item in mSelectionGroups) {
                    if (item.mId == gupdate.mId) {
                        selectionGroup = item;
                        break;
                    }
                }
            }

            if (selectionGroup == null) {
                return ARESULT.E_FAIL;
            }

            #region 更新告警设置

            lock (selectionGroup) {
                // 暂停温度处理线程
                mProcessingWorker.Pause();

                if (!selectionGroup.mAlarmConfig.mMaxTempConfig.Equals(alramconfig.mMaxTempConfig)) {
                    // 正在告警时, 结束当前告警
                    if (selectionGroup.mAlarmInfo.mMaxTempAlarm.mAlarmStatus == AlarmStatus.Alarming)
                        UpdateAlarmInfo(
                            (int)AlarmMode.GroupSelection,
                            (int)GroupAlarmType.MaxTemperature,
                            selectionGroup.mId,
                            selectionGroup.Serialize(),
                            selectionGroup.mAlarmInfo.mMaxTempAlarm);

                    selectionGroup.mAlarmConfig.mMaxTempConfig = alramconfig.mMaxTempConfig;
                    selectionGroup.mAlarmInfo.mMaxTempAlarm.Reset();
                }

                if (!selectionGroup.mAlarmConfig.mMaxTempeRiseConfig.Equals(alramconfig.mMaxTempeRiseConfig)) {
                    if (selectionGroup.mAlarmInfo.mMaxTempRiseAlarmInfo.mAlarmStatus == AlarmStatus.Alarming)
                        UpdateAlarmInfo(
                            (int)AlarmMode.GroupSelection,
                            (int)GroupAlarmType.MaxTempRise,
                            selectionGroup.mId,
                            selectionGroup.Serialize(),
                            selectionGroup.mAlarmInfo.mMaxTempRiseAlarmInfo);

                    selectionGroup.mAlarmConfig.mMaxTempeRiseConfig = alramconfig.mMaxTempeRiseConfig;
                    selectionGroup.mAlarmInfo.mMaxTempRiseAlarmInfo.Reset();
                }

                if (!selectionGroup.mAlarmConfig.mMaxTempDifConfig.Equals(alramconfig.mMaxTempDifConfig)) {
                    if (selectionGroup.mAlarmInfo.mMaxTempDifAlarmInfo.mAlarmStatus == AlarmStatus.Alarming) {
                        UpdateAlarmInfo(
                            (int)AlarmMode.GroupSelection,
                            (int)GroupAlarmType.MaxTempDif,
                            selectionGroup.mId,
                            selectionGroup.Serialize(),
                            selectionGroup.mAlarmInfo.mMaxTempDifAlarmInfo);
                    }

                    selectionGroup.mAlarmConfig.mMaxTempDifConfig = alramconfig.mMaxTempDifConfig;
                    selectionGroup.mAlarmInfo.mMaxTempDifAlarmInfo.Reset();
                }

                if (!selectionGroup.mAlarmConfig.mRelativeTempDifConfig.Equals(alramconfig.mRelativeTempDifConfig)) {
                    if (selectionGroup.mAlarmInfo.mRelativeTempDifAlarmInfo.mAlarmStatus == AlarmStatus.Alarming) {
                        UpdateAlarmInfo(
                            (int)AlarmMode.GroupSelection,
                            (int)GroupAlarmType.RelativeTempDif,
                            selectionGroup.mId,
                            selectionGroup.Serialize(),
                            selectionGroup.mAlarmInfo.mRelativeTempDifAlarmInfo);
                    }

                    selectionGroup.mAlarmConfig.mRelativeTempDifConfig = alramconfig.mRelativeTempDifConfig;
                    selectionGroup.mAlarmInfo.mRelativeTempDifAlarmInfo.Reset();
                }

                // 取消暂停
                mProcessingWorker.Resume();
            }

            #endregion

            return GroupSelectionDAO.UpdateGroupSelection(selectionGroup.mId, selectionGroup.Serialize());
        }

        /// <summary>
        /// 删除选区组
        /// </summary>
        public ARESULT RemoveSelectionGroup(long id)
        {
            if (ARESULT.AFAILED(GroupSelectionDAO.RemoveGroupSelection(id)))
                return ARESULT.E_FAIL;

            lock (mSelectionGroups) {
                foreach (SelectionGroup selectionGroup in mSelectionGroups) {
                    if (selectionGroup.mId == id) {
                        ClearSelectionGroupAlarm(selectionGroup);
                        mSelectionGroups.Remove(selectionGroup);
                        return ARESULT.S_OK;
                    }
                }
            }

            return ARESULT.E_FAIL;
        }

        /// <summary>
        /// 获取所有选区信息
        /// </summary>
        /// <returns>JSON字符串</returns>
        public string GetAllSelectionInfo()
        {
            return JsonConvert.SerializeObject(mSelections);
        }

        /// <summary>
        /// 获取所有选区组信息
        /// </summary>
        /// <returns>Json字符串</returns>
        public string GetAllSelectionGroupInfo()
        {
            return JsonConvert.SerializeObject(mSelectionGroups);
        }

        /// <summary>
        /// 清除选区报警信息
        /// </summary>
        /// <param name="selection">选区</param>
        private void ClearSelectionAlarm(Selection selection)
        {
            if ((selection.mAlarmInfo == null))
                return;

            // 温度处理线程暂停
            mProcessingWorker.Pause();

            // 正在告警时, 结束当前告警
            if (selection.mAlarmInfo.mMaxTempAlarmInfo.mAlarmStatus == AlarmStatus.Alarming) {
                UpdateAlarmInfo(
                    (int)AlarmMode.Selection,
                    (int)SelectionAlarmType.MaxTemp,
                    selection.mSelectionId,
                    selection.Serialize(),
                    selection.mAlarmInfo.mMaxTempAlarmInfo);
            }

            if (selection.mAlarmInfo.mMinTempAlarmInfo.mAlarmStatus == AlarmStatus.Alarming) {
                UpdateAlarmInfo(
                    (int)AlarmMode.Selection,
                    (int)SelectionAlarmType.MinTemp,
                    selection.mSelectionId,
                    selection.Serialize(),
                    selection.mAlarmInfo.mMinTempAlarmInfo);
            }

            if (selection.mAlarmInfo.mAvgTempAlarmInfo.mAlarmStatus == AlarmStatus.Alarming) {
                UpdateAlarmInfo(
                    (int)AlarmMode.Selection,
                    (int)SelectionAlarmType.AvgTemp,
                    selection.mSelectionId,
                    selection.Serialize(),
                    selection.mAlarmInfo.mAvgTempAlarmInfo);
            }

            selection.mAlarmInfo.mMaxTempAlarmInfo.Reset();
            selection.mAlarmInfo.mMinTempAlarmInfo.Reset();
            selection.mAlarmInfo.mAvgTempAlarmInfo.Reset();

            // 温度处理线程继续
            mProcessingWorker.Resume();
        }

        /// <summary>
        /// 清除选区组报警信息
        /// </summary>
        /// <param name="selectionGroup">选区组</param>
        private void ClearSelectionGroupAlarm(SelectionGroup selectionGroup)
        {
            if (selectionGroup.mAlarmInfo == null) {
                return;
            }

            // 温度处理线程暂停
            mProcessingWorker.Pause();

            // 正在告警时, 结束当前告警
            if (selectionGroup.mAlarmInfo.mMaxTempAlarm.mAlarmStatus == AlarmStatus.Alarming) {
                UpdateAlarmInfo(
                    (int)AlarmMode.GroupSelection,
                    (int)GroupAlarmType.MaxTemperature,
                    selectionGroup.mId,
                    selectionGroup.Serialize(),
                    selectionGroup.mAlarmInfo.mMaxTempAlarm);
            }

            if (selectionGroup.mAlarmInfo.mMaxTempRiseAlarmInfo.mAlarmStatus == AlarmStatus.Alarming) {
                UpdateAlarmInfo(
                    (int)AlarmMode.GroupSelection,
                    (int)GroupAlarmType.MaxTempRise,
                    selectionGroup.mId,
                    selectionGroup.Serialize(),
                    selectionGroup.mAlarmInfo.mMaxTempRiseAlarmInfo);
            }

            if (selectionGroup.mAlarmInfo.mMaxTempDifAlarmInfo.mAlarmStatus == AlarmStatus.Alarming) {
                UpdateAlarmInfo(
                    (int)AlarmMode.GroupSelection,
                    (int)GroupAlarmType.MaxTempDif,
                    selectionGroup.mId,
                    selectionGroup.Serialize(),
                    selectionGroup.mAlarmInfo.mMaxTempDifAlarmInfo);
            }

            if (selectionGroup.mAlarmInfo.mRelativeTempDifAlarmInfo.mAlarmStatus == AlarmStatus.Alarming) {
                UpdateAlarmInfo(
                    (int)AlarmMode.GroupSelection,
                    (int)GroupAlarmType.RelativeTempDif,
                    selectionGroup.mId,
                    selectionGroup.Serialize(),
                    selectionGroup.mAlarmInfo.mRelativeTempDifAlarmInfo);
            }

            selectionGroup.mAlarmInfo.mMaxTempAlarm.Reset();
            selectionGroup.mAlarmInfo.mMaxTempRiseAlarmInfo.Reset();
            selectionGroup.mAlarmInfo.mMaxTempDifAlarmInfo.Reset();
            selectionGroup.mAlarmInfo.mRelativeTempDifAlarmInfo.Reset();

            // 温度处理线程继续
            mProcessingWorker.Resume();
        }
    }
}
