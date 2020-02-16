using Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace IRMonitor2.Services.Cell.Worker
{
    /// <summary>
    /// 通知设置
    /// </summary>
    public class RealtimeNoticeWorker : BaseWorker
    {
        /// <summary>
        /// 整点回调
        /// </summary>
        public event Delegates.DgOnHourSend OnHourSend;

        /// <summary>
        /// 定时回调
        /// </summary>
        public event Delegates.DgOnRegularSend OnRegularSend;

        /// <summary>
        /// 定时时间集
        /// </summary>
        private List<int> mRegularTimeList = new List<int>();

        /// <summary>
        /// 发送定时时间
        /// </summary>
        private int mRegularTimeSend = 0;

        /// <summary>
        /// 当前小时数
        /// </summary>
        private int mCurrentHour = -1;

        /// <summary>
        /// 是否允许整点通知
        /// </summary>
        private bool mIsHourSend = false;

        /// <summary>
        /// 是否允许定时通知
        /// </summary>
        private bool mIsRegularTimeSend = false;

        /// <summary>
        /// 修改配置
        /// </summary>
        /// <param name="isHourSend">是否允许整点通知</param>
        /// <param name="isRegularTime">是否允许定时通知</param>
        /// <param name="regularTime">定时时间集</param>
        public void SetConfig(bool isHourSend, bool isRegularTime, List<TimeSpan> regularTime)
        {
            lock (mRegularTimeList) {
                mRegularTimeList.Clear();

                mIsHourSend = isHourSend;
                mIsRegularTimeSend = isRegularTime;

                foreach (TimeSpan time in regularTime) {
                    int seconds = (int)time.TotalSeconds;
                    mRegularTimeList.Add(seconds);
                }

                // 降序排序
                mRegularTimeList.Sort();
                mRegularTimeList.Reverse();
                mRegularTimeSend = (int)DateTime.Now.TimeOfDay.TotalSeconds;
            }
        }

        /// <summary>
        /// 整点匹配
        /// </summary>
        /// <param name="time">当前时间</param>
        /// <returns></returns>
        private bool MatchOnTime(TimeSpan time)
        {
            int hour = time.Hours;
            int minute = time.Minutes;

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
        private bool MatchRegularTime(TimeSpan time)
        {
            int seconds = (int)time.TotalSeconds;

            lock (mRegularTimeList) {
                int regularTime = mRegularTimeList.Find((item) => item <= seconds);
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
