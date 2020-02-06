using Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace IRMonitor2
{
    /// <summary>
    /// 组选区
    /// </summary>
    [DataContract]
    public class SelectionGroup
    {
        /// <summary>
        ///  ID
        /// </summary>
        [DataMember(Name = "Id")]
        public long mId;

        /// <summary>
        /// 名称
        /// </summary>
        [DataMember(Name = "GroupName")]
        public string mGroupName;

        /// <summary>
        /// 主选区ID
        /// </summary>
        [DataMember(Name = "PrimarySelection")]
        public long mPrimarySelectionId;

        /// <summary>
        /// 选区列表
        /// </summary>
        [DataMember(Name = "GroupSelections")]
        public List<long> mSelectionIds = new List<long>();

        /// <summary>
        /// 告警设置
        /// </summary>
        [DataMember(Name = "AlarmConfig")]
        public GroupAlarmConfig mAlarmConfig = new GroupAlarmConfig();

        /// <summary>
        /// 告警状态信息
        /// </summary>
        [NonSerialized]
        public SelectionGroupAlarmInfo mAlarmInfo = new SelectionGroupAlarmInfo();

        /// <summary>
        /// 温度信息
        /// </summary>
        [NonSerialized]
        public GroupTempData mTemperatureData = new GroupTempData();

        /// <summary>
        /// 选区链表
        /// </summary>
        [NonSerialized]
        public List<Selection> mSelections = new List<Selection>();

        /// <summary>
        /// 计算温度
        /// </summary>
        /// <param name="selections">选区列表</param>
        /// <returns>计算结果</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public ARESULT CalcTemperature(List<Selection> selections)
        {
            // 重建选区列表
            mSelections.Clear();
            Selection primarySeletion = null;
            foreach (var id in mSelectionIds) {
                foreach (Selection selection in selections) {
                    if (selection.mSelectionId == id) {
                        mSelections.Add(selection);
                        break;
                    }
                    else if (selection.mSelectionId == mPrimarySelectionId) {
                        primarySeletion = selection;
                        break;
                    }
                }
            }

            // 主选区不能为空
            if (primarySeletion == null) {
                return ARESULT.E_FAIL;
            }

            #region 计算温差,温升,最高温,相对温差

            mTemperatureData.mMaxTemperature = primarySeletion.mTemperatureData.mMaxTemperature;
            float? minTemp = null;
            foreach (Selection selection in mSelections) {
                mTemperatureData.mMaxTemperature =
                    mTemperatureData.mMaxTemperature > selection.mTemperatureData.mMaxTemperature ?
                    mTemperatureData.mMaxTemperature : selection.mTemperatureData.mMaxTemperature;

                if (minTemp.HasValue) {
                    if (minTemp.Value > selection.mTemperatureData.mMaxTemperature) {
                        minTemp = selection.mTemperatureData.mMaxTemperature;
                    }
                }
                else {
                    minTemp = selection.mTemperatureData.mMaxTemperature;
                }
            }

            if (minTemp.HasValue) {
                mTemperatureData.mTemperatureRise = primarySeletion.mTemperatureData.mMaxTemperature - minTemp.Value;
                mTemperatureData.mTemperatureDif = primarySeletion.mTemperatureData.mMaxTemperature - minTemp.Value;
            }

            if (mSelections.Count >= 2) {
                float t1 = Math.Abs(primarySeletion.mTemperatureData.mMaxTemperature - mSelections[0].mTemperatureData.mMaxTemperature);
                float t2 = Math.Abs(primarySeletion.mTemperatureData.mMaxTemperature - mSelections[1].mTemperatureData.mMaxTemperature);
                if (t1 > t2) {
                    mTemperatureData.mRelTemperatureDif = t2 / t1;
                }
                else {
                    if (t2 == 0) {
                        mTemperatureData.mRelTemperatureDif = 0;
                    }
                    else {
                        mTemperatureData.mRelTemperatureDif = t1 / t2;
                    }
                }
            }

            #endregion

            // 主选区加入选区列表
            mSelections.Insert(0, primarySeletion);

            return ARESULT.S_OK;
        }

        /// <summary>
        /// 选区索引是否在选区组中
        /// </summary>
        /// <param name="id">选区索引</param>
        /// <returns>是否在选区组中</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool InSelections(long id)
        {
            return (id == mPrimarySelectionId) || mSelectionIds.Exists(value => value == id);
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns>序列化信息串</returns>
        public string Serialize()
        {
            return JsonUtils.ObjectToJson(this);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="data">序列化信息串</param>
        /// <returns>结果</returns>
        public ARESULT Deserialize(string data)
        {
            var selectionGroup = JsonUtils.ObjectFromJson<SelectionGroup>(data);
            if (selectionGroup == null) {
                return ARESULT.E_FAIL;
            }

            mId = selectionGroup.mId;
            mGroupName = selectionGroup.mGroupName;
            mPrimarySelectionId = selectionGroup.mPrimarySelectionId;
            mSelectionIds = selectionGroup.mSelectionIds;
            mAlarmConfig = selectionGroup.mAlarmConfig;

            return ARESULT.S_OK;
        }
    }

    /// <summary>
    /// 告警状态信息
    /// </summary>
    public class SelectionGroupAlarmInfo
    {
        /// <summary>
        ///  最大温度
        /// </summary>
        public AlarmInfo mMaxTempAlarm = new AlarmInfo();

        /// <summary>
        /// 最大温升
        /// </summary>
        public AlarmInfo mMaxTempRiseAlarmInfo = new AlarmInfo();

        /// <summary>
        /// 最大温差
        /// </summary>
        public AlarmInfo mMaxTempDifAlarmInfo = new AlarmInfo();

        /// <summary>
        /// 相对温差
        /// </summary>
        public AlarmInfo mRelativeTempDifAlarmInfo = new AlarmInfo();
    }
}
