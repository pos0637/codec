using System;
using System.Text.RegularExpressions;

namespace Fg.DBHelper.Utilities
{
    public class ConvertUtils
    {
        /// <summary>
        /// 将Object对象转换为指定类型的对象
        /// </summary>
        /// <typeparam name="T">要返回对象的类型</typeparam>
        /// <param name="value">源数据</param>
        /// <param name="objDefault">返回对象的默认值</param>
        /// <returns>返回指定的对象T</returns>
        public static T ChangeType<T>(Object value, T objDefault)
        {
            T obj = default(T);
            try {
                obj = (T)Convert.ChangeType(value, typeof(T));
                if (obj == null) {
                    obj = objDefault;
                }
            }
            catch {
                obj = objDefault;
            }
            return obj;
        }

        /// <summary>
        /// 转换sql中的占位符
        /// </summary>
        /// <param name="sql">Sql脚本</param>
        /// <param name="placeHolder">占位符</param>
        /// <returns>转后的Sql</returns>
        public static String ConvertSqlPlaceHolderOfSqlStatement(String sql, String placeHolder)
        {
            Regex regex = new Regex(@"@(\w+)");
            return regex.Replace(sql, placeHolder);
        }
    }
}
