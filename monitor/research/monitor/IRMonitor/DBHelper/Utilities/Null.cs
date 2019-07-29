using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Fg.DBHelper.Utilities
{
    public class Null
    {
        #region "公有属性:定义各种类型的Null值"

        public static Boolean NullBoolean {
            get {
                return false;
            }
        }

        public static Byte NullByte {
            get {
                return 0xff;
            }
        }

        public static DateTime NullDate {
            get {
                return DateTime.MinValue;
            }
        }

        public static Decimal NullDecimal {
            get {
                return -79228162514264337593543950335M;
            }
        }

        public static Double NullDouble {
            get {
                return Double.MinValue;
            }
        }

        public static Guid NullGuid {
            get {
                return Guid.Empty;
            }
        }

        public static Int32 NullInteger {
            get {
                return -1;
            }
        }

        public static Int16 NullShort {
            get {
                return -1;
            }
        }

        public static Single NullSingle {
            get {
                return Single.MinValue;
            }
        }

        public static String NullString {
            get { return String.Empty; }
        }

        public static Char NullChar {
            get { return Char.MinValue; }
        }

        #endregion

        #region "公有函数"

        public static Boolean IsNull(Object objField)
        {
            if (objField != null) {
                if (objField is Int32) {
                    return objField.Equals(NullInteger);
                }
                if (objField is Int16) {
                    return objField.Equals(NullShort);
                }
                if (objField is Byte) {
                    return objField.Equals(NullByte);
                }
                if (objField is Single) {
                    return objField.Equals(NullSingle);
                }
                if (objField is Double) {
                    return objField.Equals(NullDouble);
                }
                if (objField is Decimal) {
                    return objField.Equals(NullDecimal);
                }
                if (objField is DateTime) {
                    return ((DateTime)objField).Equals(NullDate.Date);
                }
                if (objField is String) {
                    return objField.Equals(NullString);
                }
                if (objField is Boolean) {
                    return objField.Equals(NullBoolean);
                }
                return ((objField is Guid) && objField.Equals(NullGuid));
            }
            return true;
        }

        public static Object GetNull(PropertyInfo objPropertyInfo)
        {
            Type pType = objPropertyInfo.PropertyType;
            switch (pType.ToString()) {
                case "System.Int16":
                    return NullShort;
                case "System.Int32":
                case "System.Int64":
                    return NullInteger;
                case "system.Byte":
                    return NullByte;
                case "System.Single":
                    return NullSingle;
                case "System.Double":
                    return NullDouble;
                case "System.Decimal":
                    return NullDecimal;
                case "System.DateTime":
                    return NullDate;
                case "System.String":
                    return NullString;
                case "System.Char":
                    return NullChar;
                case "System.Boolean":
                    return NullBoolean;
                case "System.Guid":
                    return NullGuid;
            }

            if (pType.BaseType.Equals(typeof(Enum))) {
                Array objEnumValues = Enum.GetValues(pType);
                Array.Sort(objEnumValues);
                return RuntimeHelpers.GetObjectValue(
                    Enum.ToObject(pType, RuntimeHelpers.GetObjectValue(objEnumValues.GetValue(0))));
            }

            return null;
        }

        #endregion
    }
}
