using System.Collections.Specialized;
using System.Web;

namespace Miscs
{
    /// <summary>
    /// 字符串工具类
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// 将查询字符串转换为键值对
        /// </summary>
        /// <param name="data">查询字符串</param>
        /// <returns>键值对</returns>
        public static NameValueCollection ParseQueryString(this string data)
        {
            return HttpUtility.ParseQueryString(data);
        }
    }
}
