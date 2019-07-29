using Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace IRTerminal.Worker
{
    /// <summary>
    /// 通知设置
    /// </summary>
    public class RealtimeNoticeWorker : BaseWorker
    {
        /// <summary>
        /// 整点回调
        /// </summary>
        public event DelegateStorage.DgOnHourSend OnHourSend;

        /// <summary>
        /// 定时回调
        /// </summary>
        public event DelegateStorage.DgOnRegularSend OnRegularSend;

        /// <summary>
        /// 定时时间集
        /// </summary>
        private List<Int32> mRegularTimeList = new List<Int32>();

        /// <summary>
        /// 发送定时时间
        /// </summary>
        private Int32 mRegularTimeSend = 0;

        /// <summary>
        /// 当前小时数
        /// </summary>
        private Int32 mCurrentHour = -1;

        /// <summary>
        /// 是否允许整点通知
        /// </summary>
        private Boolean mIsHourSend = false;

        /// <summary>
        /// 是否允许定时通知
        /// </summary>
        private Boolean mIsRegularTimeSend = false;

        /// <summary>
        /// 修改配置
        /// </summary>
        /// <param name="isHourSend">是否允许整点通知</param>
        /// <param name="isRegularTime">是否允许定时通知</param>
        /// <param name="regularTime">定时时间集</param>
        public void SetConfig(Boolean isHourSend, Boolean isRegularTime, List<TimeSpan> regularTime)
        {
            lock (mRegularTimeList) {
                mRegularTimeList.Clear();

                mIsHourSend = isHourSend;
                mIsRegularTimeSend = isRegularTime;

                foreach (TimeSpan time in regularTime) {
                    Int32 seconds = (Int32)time.TotalSeconds;
                    mRegularTimeList.Add(seconds);
                }

                // 降序排序
                mRegularTimeList.Sort();
                mRegularTimeList.Reverse();
                mRegularTimeSend = (Int32)DateTime.Now.TimeOfDay.TotalSeconds;
            }
        }

        /// <summary>
        /// 整点匹配
        /// </summary>
        /// <param name="time">当前时间</param>
        /// <returns></returns>
        private Boolean MatchOnTime(TimeSpan time)
        {
            Int32 hour = time.Hours;
            Int32 minute = time.Minutes;

            if ((hour != mCurrentHour) && (minute >= 59)) {
                mCurrentHour = hour;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 定时匹配
        /// </summary>
        /// <param name="time">当前时间</param>
        /// <returns></returns>
        private Boolean MatchRegularTime(TimeSpan time)
        {
            Int32 seconds = (Int32)time.TotalSeconds;

            lock (mRegularTimeList) {
                Int32 regularTime = mRegularTimeList.Find((item) => item <= seconds);
                if ((regularTime != 0) && (regularTime > mRegularTimeSend)) {
                    mRegularTimeSend = regularTime;
                    return true;
                }
                else {
                    return false;
                }
            }
        }

        protected override void Run()
        {
            while (!IsTerminated()) {
                TimeSpan time = DateTime.Now.TimeOfDay;

                if (mIsHourSend && MatchOnTime(time))
                    OnHourSend?.Invoke();

                if (mIsRegularTimeSend && MatchRegularTime(time))
                    OnRegularSend?.Invoke();

                Thread.Sleep(1000);
            }
        }

        public override void Discard()
        {
            OnHourSend = null;
            OnRegularSend = null;
            base.Discard();
        }
    }
}
