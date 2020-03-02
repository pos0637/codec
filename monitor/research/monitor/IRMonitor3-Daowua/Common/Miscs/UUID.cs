namespace Miscs
{
    /// <summary>
    /// 全局唯一索引
    /// </summary>
    public static class UUID
    {
        /// <summary>
        /// 生成全局唯一索引
        /// </summary>
        /// <returns>全局唯一索引</returns>
        public static string GenerateUUID()
        {
            return System.Guid.NewGuid().ToString("N");
        }
    }
}
