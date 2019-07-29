using Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IRMonitor
{
    /// <summary>
    /// 组选区
    /// </summary>
    [DataContract]
    public class GroupSelection
    {
        /// <summary>
        ///  ID
        /// </summary>
        [DataMember(Name = "Id")]
        public Int64 mId;

        /// <summary>
        /// 名称
        /// </summary>
        [DataMember(Name = "GroupName")]
        public String mGroupName;

        /// <summary>
        /// 主选区ID
        /// </summary>
        [DataMember(Name = "PrimarySelection")]
        public Int64 mPrimarySelectionId;

        /// <summary>
        /// 选区链表
        /// </summary>
        [DataMember(Name = "GroupSelections")]
        public List<Int64> mSelectionIds = new List<Int64>();

        /// <summary>
        /// 告警设置
        /// </summary>
        [DataMember(Name = "AlarmConfig")]
        public GroupAlarmConfig mAlarmConfig = new GroupAlarmConfig();

        /// <summary>
        /// 告警状态信息
        /// </summary>
        [NonSerialized]
        public GroupSelectionAlarmInfo mAlarmInfo = new GroupSelectionAlarmInfo();

        /// <summary>
        /// 温度信息
        /// </summary>
        [NonSerialized]
        public GroupTempData mTemperatureData = new GroupTempData();

        /// <summary>
        /// 选区链表
        /// </summary>
        [NonSerialized]
        public List<Selection> mSelectionList = new List<Selection>();

        public ARESULT CalcSelectionAreaValue(List<Selection> sList)
        {
            mSelectionList.Clear();
            Selection primaryseletion = null;
            foreach (Int64 id in mSelectionIds) {
                foreach (Selection item in sList) {
                    if (item.mSelectionId == id) {
                        mSelectionList.Add(item);
                        continue;
                    }
                    else if (item.mSelectionId == mPrimarySelectionId) {
                        primaryseletion = item;
                        continue;
                    }
                }
            }

            if (primaryseletion == null)
                return ARESULT.E_FAIL;

            #region 计算温差,温升,最高温,相对温差

            mTemperatureData.mMaxTemperature = primaryseletion.mTemperatureData.mMaxTemperature;
            Single? minTemp = null;
            foreach (Selection selection in mSelectionList) {
                mTemperatureData.mMaxTemperature =
                    mTemperatureData.mMaxTemperature > selection.mTemperatureData.mMaxTemperature ?
                    mTemperatureData.mMaxTemperature : selection.mTemperatureData.mMaxTemperature;

                if (minTemp.HasValue) {
                    if (minTemp.Value > selection.mTemperatureData.mMaxTemperature)
                        minTemp = selection.mTemperatureData.mMaxTemperature;
                }
                else
                    minTemp = selection.mTemperatureData.mMaxTemperature;
            }

            if (minTemp.HasValue) {
                mTemperatureData.mTemperatureRise = primaryseletion.mTemperatureData.mMaxTemperature - minTemp.Value;
                mTemperatureData.mTemperatureDif = primaryseletion.mTemperatureData.mMaxTemperature - minTemp.Value;
            }

            if (mSelectionList.Count >= 2) {
                Single t1 = Math.Abs(
                    primaryseletion.mTemperatureData.mMaxTemperature - mSelectionList[0].mTemperatureData.mMaxTemperature);
                Single t2 = Math.Abs(
                    primaryseletion.mTemperatureData.mMaxTemperature - mSelectionList[1].mTemperatureData.mMaxTemperature);

                if (t1 > t2)
                    mTemperatureData.mRelTemperatureDif = t2 / t1;
                else {
                    if (t2 == 0)
                        mTemperatureData.mRelTemperatureDif = 0;
                    else
                        mTemperatureData.mRelTemperatureDif = t1 / t2;
                }
            }

            #endregion

            mSelectionList.Insert(0, primaryseletion);
            return ARESULT.S_OK;
        }

        public String Serialize()
        {
            return JsonUtils.ObjectToJson(this);
        }

        public ARESULT Deserialize(
            String data)
        {
            GroupSelection groupSelection = JsonUtils.ObjectFromJson<GroupSelection>(data);
            if (groupSelection == null)
                return ARESULT.E_FAIL;

            mId = groupSelection.mId;
            mGroupName = groupSelection.mGroupName;
            mPrimarySelectionId = groupSelection.mPrimarySelectionId;
            mSelectionIds = groupSelection.mSelectionIds;
            mAlarmConfig = groupSelection.mAlarmConfig;

            return ARESULT.S_OK;
        }
    }

    /// <summary>
    /// 告警状态信息
    /// </summary>
    public class GroupSelectionAlarmInfo
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
