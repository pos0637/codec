using Common;

namespace Miscs
{
    /// <summary>
    /// 全局唯一索引
    /// </summary>
    public static class UUID
    {
        private static object uuidLocker = new object();
        private static long uuid = 0;

        /// <summary>
        /// 生成全局唯一索引
        /// </summary>
        /// <returns>全局唯一索引</returns>
        public static long GenerateUUID()
        {
            lock (uuidLocker) {
                if (uuid == long.MaxValue) {
                    Tracker.LogE("UUID is full!");
                    uuid = 0;
                }

                return uuid++;
            }
        }
    }
}
