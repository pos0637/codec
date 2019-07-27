using System;

namespace Common
{
    /// <summary>
    /// 单例模式
    /// </summary>
    public class Singleton<T> where T : class
    {
        private static T sInstance;
        private static readonly object sLock = new object();

        public static T Instance {
            get {
                if (sInstance == null) {
                    lock (sLock) {
                        if (sInstance == null)
                            sInstance = (T)Activator.CreateInstance(typeof(T), true);
                    }
                }
                return sInstance;
            }
        }
    }
}
