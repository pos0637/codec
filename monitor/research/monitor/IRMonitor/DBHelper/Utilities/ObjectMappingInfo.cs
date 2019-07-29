using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Fg.DBHelper.Utilities
{
    /// <summary>
    /// 对象映射信息
    /// </summary>
    [Serializable]
    public class ObjectMappingInfo
    {
        private const String mObjectMapCacheKey = "ObjectMap_";
        private const String mDefaultPrimaryKey = "ItemID";

        private static Hashtable hashtable = Hashtable.Synchronized(new Hashtable());
        private String _CacheByProperty;
        private Int32 _CacheTimeOutMultiplier;
        private Dictionary<String, String> _ColumnNames = new Dictionary<String, String>();
        private String _ObjectType;
        private String _PrimaryKey;
        private Dictionary<String, PropertyInfo> _Properties = new Dictionary<String, PropertyInfo>();
        private String _TableName;

        public String CacheByProperty {
            get {
                return this._CacheByProperty;
            }
            set {
                this._CacheByProperty = value;
            }
        }

        public String CacheKey {
            get {
                String _CacheKey = "ObjectCache_" + TableName + "_";
                if (!String.IsNullOrEmpty(CacheByProperty)) {
                    _CacheKey = _CacheKey + CacheByProperty + "_";
                }
                return _CacheKey;
            }
        }

        public Int32 CacheTimeOutMultiplier {
            get {
                return this._CacheTimeOutMultiplier;
            }
            set {
                this._CacheTimeOutMultiplier = value;
            }
        }

        public Dictionary<String, String> ColumnNames {
            get {
                return this._ColumnNames;
            }
        }

        public String ObjectType {
            get {
                return this._ObjectType;
            }
            set {
                this._ObjectType = value;
            }
        }

        public String PrimaryKey {
            get {
                return this._PrimaryKey;
            }
            set {
                this._PrimaryKey = value;
            }
        }

        public Dictionary<String, PropertyInfo> Properties {
            get {
                return this._Properties;
            }
        }

        public String TableName {
            get {
                return this._TableName;
            }
            set {
                this._TableName = value;
            }
        }

        /// <summary>
        /// 获取对象映射信息
        /// </summary>
        /// <param name="objType">对象类型</param>
        /// <returns></returns>
        public static ObjectMappingInfo GetObjectMapping(Type objType)
        {
            String cacheKey = mObjectMapCacheKey + objType.FullName;
            try {
                ObjectMappingInfo objMap = hashtable[cacheKey] as ObjectMappingInfo;
                if (objMap == null) {
                    objMap = new ObjectMappingInfo();
                    objMap.ObjectType = objType.FullName;
                    objMap.PrimaryKey = GetPrimaryKey(objType);
                    objMap.TableName = GetTableName(objType);

                    foreach (PropertyInfo objProperty in objType.GetProperties()) {
                        objMap.Properties.Add(
                            objProperty.Name.ToUpperInvariant(),
                            objProperty);

                        objMap.ColumnNames.Add(
                            objProperty.Name.ToUpperInvariant(),
                            GetColumnName(objProperty));
                    }

                    hashtable.Add(cacheKey, objMap);
                }
                return objMap;
            }
            catch {
                return null;
            }
        }

        /// <summary>
        /// 初始化对象各属性
        /// </summary>
        /// <param name="objObject">需初始化的对象</param>
        /// <returns>初始化后的对象</returns>
        public static Object InitializationObject(Object objObject)
        {
            return InitializationObject(objObject, objObject.GetType());
        }

        /// <summary>
        /// 初始化对象各属性
        /// </summary>
        /// <param name="objObject">需初始化的对象</param>
        /// <param name="objType">需初始化的对象的类型</param>
        /// <returns>初始化后的对象</returns>
        public static Object InitializationObject(Object objObject, Type objType)
        {
            ObjectMappingInfo objMapping = GetObjectMapping(objType);
            if (objMapping == null)
                return null;

            try {
                foreach (PropertyInfo objPropertyInfo in objMapping.Properties.Values) {
                    if (objPropertyInfo.CanWrite) {
                        objPropertyInfo.SetValue(objObject, Null.GetNull(objPropertyInfo), null);
                    }
                }
                return objObject;
            }
            catch {
                return null;
            }
        }

        private static String GetPrimaryKey(Type objType)
        {
            String primaryKey = mDefaultPrimaryKey;
            return primaryKey;
        }

        private static String GetTableName(Type objType)
        {
            String tableName = objType.Name;
            return tableName;
        }

        private static String GetColumnName(PropertyInfo objProperty)
        {
            String columnName = objProperty.Name;
            return columnName;
        }
    }
}
