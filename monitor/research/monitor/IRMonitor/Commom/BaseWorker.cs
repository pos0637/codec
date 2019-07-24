using System;
using System.Threading;

namespace Common
{
    /// <summary>
    /// 工作线程
    /// </summary>
    public class BaseWorker
    {
        /// <summary>
        /// 执行函数接口
        /// </summary>
        public interface ExecutorInterface
        {
            /// <summary>
            /// 执行
            /// </summary>
            /// <param name="worker">工作线程</param>
            void Run(BaseWorker worker);
        }

        /// <summary>
        /// 线程状态        
        /// </summary>
        public enum WorkerStatus
        {
            Idle = 0, // 线程处于无动作状态
            Running = 1, // 线程处于运行状态
            WaitFor = 2, // 线程处于被等待结束状态
            Disposed = 3, // 线程处于已被释放状态
        }

        /// <summary>
        /// 终止事件函数委托类型
        /// </summary>
        /// <param name="userData">用户数据</param>
        public delegate void DgOnTerminated(ARESULT userData);

        // 等待线程数量
        private Int32 mWaitCount = 0;

        // 线程运行标志
        private Boolean mRunFlag = false;

        // 线程状态
        private WorkerStatus mStatus = WorkerStatus.Idle;

        // 用户数据
        private ARESULT mUserData = ARESULT.S_OK;

        // 执行函数接口
        private ExecutorInterface mExecutor;

        // 工作线程锁
        public Object mLock = new Object();

        // 工作线程事件
        public Object mEvent = new Object();

        // 终止事件函数委托
        private DgOnTerminated mDgOnTerminated;
        public DgOnTerminated OnTerminated
        {
            private get { return mDgOnTerminated; }
            set { lock (mLock) { mDgOnTerminated = value; } }
        }

        // 线程名称
        private String mName = "unnamed";
        public String Name
        {
            get { return mName; }
            protected set { mName = value; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseWorker()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="executor">执行函数接口</param>
        public BaseWorker(ExecutorInterface executor)
        {
            mExecutor = executor;
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~BaseWorker()
        {
            Discard();
            Join();
        }

        /// <summary>
        /// 启动工作线程
        /// </summary>
        /// <returns>启动结果</returns>
        public virtual ARESULT Start()
        {
            lock (mLock) {
                if (mStatus != WorkerStatus.Idle) {
                    return ARESULT.E_FAIL;
                }

                mStatus = WorkerStatus.Running;
                mRunFlag = true;

                try {
                    if (!ThreadPool.QueueUserWorkItem(Execute)) {
                        mStatus = WorkerStatus.Idle;
                        mRunFlag = false;
                        return ARESULT.E_FAIL;
                    }
                }
                catch {
                    mStatus = WorkerStatus.Idle;
                    mRunFlag = false;
                    return ARESULT.E_FAIL;
                }
            }

            return ARESULT.S_OK;
        }

        /// <summary>
        /// 等待
        /// </summary>
        /// <param name="timeout">超时时间</param>
        public virtual void Wait(Int32 timeout)
        {
            lock (mEvent) {
                Monitor.Wait(mEvent, timeout);
            }
        }

        /// <summary>
        /// 等待工作线程结束
        /// </summary>
        public virtual void Join()
        {
            lock (mLock) {
                if (mStatus == WorkerStatus.Idle) {
                    return;
                }

                mStatus = WorkerStatus.WaitFor;
                mWaitCount++;
            }

            lock (mEvent) {
                Monitor.Wait(mEvent);
            }

            lock (mLock) {
                if (--mWaitCount > 0) {
                    return;
                }

                mStatus = WorkerStatus.Idle;
            }
        }

        /// <summary>
        /// 请求终止线程
        /// </summary>
        /// <param name="userData">用户参数</param>
        public void NotifyToStop(ARESULT userData)
        {
            lock (mLock) {
                if (mRunFlag) {
                    mRunFlag = false;
                    mUserData = userData;
                }
            }
        }

        /// <summary>
        /// 抛弃工作线程
        /// </summary>
        public virtual void Discard()
        {
            lock (mLock) {
                mDgOnTerminated = null;
                NotifyToStop(ARESULT.E_TERMINATED);
            }
        }

        /// <summary>
        /// 是否终止运行
        /// </summary>
        /// <returns>终止运行标志</returns>
        public Boolean IsTerminated()
        {
            Boolean ret;

            lock (mLock) {
                ret = (mRunFlag == false);
            }

            return ret;
        }

        /// <summary>
        /// 线程是否处于无动作状态
        /// </summary>
        /// <returns>无动作状态标志</returns>
        public Boolean IsIdle()
        {
            Boolean ret;

            lock (mLock) {
                ret = (mStatus == WorkerStatus.Idle);
            }

            return ret;
        }

        /// <summary>
        /// 锁定工作线程
        /// </summary>
        public void Down()
        {
            Monitor.Enter(mLock);
        }

        /// <summary>
        /// 解锁工作线程
        /// </summary>
        public void Up()
        {
            Monitor.Exit(mLock);
        }

        /// <summary>
        /// 重新获取配置信息
        /// </summary>
        public virtual ARESULT Reset()
        {
            return ARESULT.S_OK;
        }

        /// <summary>
        /// 执行函数
        /// </summary>
        protected virtual void Run()
        {
            // 运行执行函数接口
            if (mExecutor != null)
                mExecutor.Run(this);
        }

        /// <summary>
        /// 执行函数
        /// </summary>
        /// <param name="state">线程参数</param>
        private void Execute(Object state)
        {
            Run();

            lock (mLock) {
                if (mStatus == WorkerStatus.Running)
                    mStatus = WorkerStatus.Idle;

                NotifyToStop(ARESULT.S_OK);
            }

            // 唤醒等待事件
            lock (mEvent) {
                Monitor.PulseAll(mEvent); 
            }
        }
    }
}