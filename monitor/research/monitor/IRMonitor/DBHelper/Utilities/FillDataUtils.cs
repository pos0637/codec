using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Fg.DBHelper.Utilities
{
    public class FillDataUtils
    {
        #region "公有函数"

        /// <summary>
        /// 将IDataReader中数据填充到指定对象集合中
        /// </summary>
        /// <typeparam name="TItem">指定的对象类型</typeparam>
        /// <param name="dataReader">IDataReader</param>
        /// <param name="objList">指定的对象集合</param>
        /// <param name="closeReader">填充完成后是否关闭IDataReader</param>
        /// <returns></returns>
        public static List<TItem> FillCollection<TItem>(IDataReader dataReader,
            List<TItem> objList = null, Boolean closeReader = true)
        {
            return (List<TItem>)FillListFromReader<TItem>(dataReader, objList, closeReader);
        }

        /// <summary>
        /// 将IDataReader中数据填充到指定对象集合中
        /// </summary>
        /// <param name="objType">指定的对象类型</param>
        /// <param name="dataReader">IDataReader</param>
        /// <param name="objList">指定的对象集合</param>
        /// <param name="closeReader">填充完成后是否关闭IDataReader</param>
        /// <returns></returns>
        public static ArrayList FillCollection(Type objType, IDataReader dataReader,
            ArrayList objList = null, Boolean closeReader = true)
        {
            return (ArrayList)FillListFromReader(objType, dataReader, objList, closeReader);
        }

        /// <summary>
        /// 将IDataReader中数据填充到指定对象中
        /// </summary>
        /// <typeparam name="TObject">指定的对象类型</typeparam>
        /// <param name="dataReader">IDataReader</param>
        /// <param name="closeReader">填充完成后是否关闭IDataReader</param>
        /// <returns></returns>
        public static TObject FillObject<TObject>(IDataReader dataReader, Boolean closeReader = true)
        {
            return (TObject)CreateObjectFromReader(typeof(TObject), dataReader);
        }

        /// <summary>
        /// 将IDataReader中数据填充到指定对象中
        /// </summary>
        /// <param name="objType">指定的对象类型</param>
        /// <param name="dataReader">IDataReader</param>
        /// <param name="closeReader">填充完成后是否关闭IDataReader</param>
        /// <returns></returns>
        public static Object FillObject(Type objType, IDataReader dataReader, Boolean closeReader = true)
        {
            return CreateObjectFromReader(objType, dataReader, closeReader);
        }

        /// <summary>
        /// 关闭 IDataReader
        /// </summary>
        /// <param name="dataReader">IDataReader</param>
        /// <param name="closeReader">是否关闭IDataReader</param>
        public static void CloseDataReader(IDataReader dataReader, Boolean closeReader = true)
        {
            if (dataReader != null && closeReader) {
                dataReader.Close();
            }
        }

        /// <summary>
        /// 创建对象实例
        /// </summary>
        /// <typeparam name="TObject">要创建的对象类型</typeparam>
        /// <param name="isInitialization">是否初始化对象实例</param>
        /// <returns>返回创建的对象实例></returns> 
        public static TObject CreateObject<TObject>(Boolean isInitialization = false)
        {
            return (TObject)CreateObject(typeof(TObject), isInitialization);
        }

        /// <summary>
        /// 创建对象实例
        /// </summary>
        /// <param name="objType">要创建的对象类型</param>
        /// <param name="isInitialization">是否初始化对象实例</param>
        /// <returns>返回创建的对象实例</returns>
        public static Object CreateObject(Type objType, Boolean isInitialization = false)
        {
            try {
                Object objObject = Activator.CreateInstance(objType);
                if (isInitialization) {
                    ObjectMappingInfo.InitializationObject(objObject);
                }
                return objObject;
            }
            catch {
                throw;
            }
        }

        #endregion

        #region "私有函数"

        /// <summary>
        /// 将IDataReader中数据填充到指定对象集合中
        /// </summary>
        /// <param name="objType">指定的对象类型</param>
        /// <param name="dataReader">IDataReader</param>
        /// <param name="objList">要填充的对象集合</param>
        /// <param name="closeReader">填充完成后是否关闭IDataReader</param>
        /// <returns></returns>
        private static IList FillListFromReader(Type objType, IDataReader dataReader,
            IList objList = null, Boolean closeReader = true)
        {
            Object objObject;
            Boolean isSuccess = false;

            try {
                if (objList == null)
                    objList = new ArrayList();

                while ((!dataReader.IsClosed) && dataReader.Read()) {
                    objObject = CreateObjectFromReader(objType, dataReader, false);
                    objList.Add(objObject);
                }

                isSuccess = true;
            }
            finally {
                if (!isSuccess)
                    closeReader = true;

                CloseDataReader(dataReader, closeReader);
            }

            return objList;
        }

        /// <summary>
        /// 将IDataReader中数据填充到指定对象集合中
        /// </summary>
        /// <typeparam name="TItem">指定的对象类型</typeparam>
        /// <param name="dataReader">IDataReader</param>
        /// <param name="objList">要填充的对象集合</param>
        /// <param name="closeReader">填充完成后是否关闭IDataReader</param>
        /// <returns></returns>
        private static IList<TItem> FillListFromReader<TItem>(IDataReader dataReader,
            IList<TItem> objList = null, Boolean closeReader = true)
        {
            TItem objObject;
            Boolean isSuccess = false;

            try {
                if (objList == null)
                    objList = new List<TItem>();

                while ((!dataReader.IsClosed) && dataReader.Read()) {
                    objObject = (TItem)CreateObjectFromReader(typeof(TItem), dataReader, false);
                    objList.Add(objObject);
                }

                isSuccess = true;
            }
            finally {
                if (!isSuccess)
                    closeReader = true;

                CloseDataReader(dataReader, closeReader);
            }

            return objList;
        }

        /// <summary>
        /// 创建对象，填充IDataReader中数据
        /// </summary>
        /// <param name="objType">要创建的对象类型</param>
        /// <param name="dataReader">IDataReader</param>
        /// <param name="closeReader">填充完成后是否关闭IDataReader</param>
        /// <returns></returns>
        private static Object CreateObjectFromReader(Type objType, IDataReader dataReader,
            Boolean closeReader = true)
        {
            Object objObject = null;
            Boolean isSuccess = false;
            Boolean canRead = true;

            if (closeReader) {
                canRead = false;
                if (dataReader.Read()) {
                    canRead = true;
                }
            }

            try {
                if (canRead) {
                    objObject = CreateObject(objType, false);
                    FillObjectFromReader(objObject, dataReader);
                }

                isSuccess = true;
            }
            catch {
                throw;
            }
            finally {
                if ((!isSuccess))
                    closeReader = true;

                CloseDataReader(dataReader, closeReader);
            }

            return objObject;
        }

        /// <summary>
        /// 将IDataReader中数据填充到对象中
        /// </summary>
        /// <param name="objObject">要填充的对象</param>
        /// <param name="dataReader">IDataReader</param>
        private static void FillObjectFromReader(Object objObject, IDataReader dataReader)
        {
            PropertyInfo objPropertyInfo = null;
            Type objPropertyType = null;
            Object objDataValue;
            Type objDataType;
            Int32 intIndex;
            ObjectMappingInfo objMappingInfo = ObjectMappingInfo.GetObjectMapping(objObject.GetType());
            if (objMappingInfo == null) {
                objObject = null;
                return;
            }

            try {
                for (intIndex = 0; intIndex <= dataReader.FieldCount - 1; intIndex++) {
                    if (objMappingInfo.Properties.TryGetValue(dataReader.GetName(intIndex).ToUpperInvariant(), out objPropertyInfo)) {
                        objPropertyType = objPropertyInfo.PropertyType;
                        if (objPropertyInfo.CanWrite) {
                            objDataValue = dataReader.GetValue(intIndex);
                            objDataType = objDataValue.GetType();
                            if (objDataValue == null || objDataValue == DBNull.Value) {
                                objPropertyInfo.SetValue(objObject, Null.GetNull(objPropertyInfo), null);
                            }
                            else if (objPropertyType.Equals(objDataType)) {
                                objPropertyInfo.SetValue(objObject, objDataValue, null);
                            }
                            else {
                                if (objPropertyType == typeof(Char)) {
                                    if (objDataValue.ToString().Length > 1)
                                        objPropertyInfo.SetValue(objObject, objDataValue.ToString().ToCharArray()[0], null);
                                    else
                                        objPropertyInfo.SetValue(objObject, Null.NullChar, null);
                                }
                                else if (objPropertyType.BaseType.Equals(typeof(Enum))) {
                                    if (Regex.IsMatch(objDataValue.ToString(), "^\\d+$")) {
                                        objPropertyInfo.SetValue(objObject, Enum.ToObject(objPropertyType, Convert.ToInt32(objDataValue)), null);
                                    }
                                    else {
                                        objPropertyInfo.SetValue(objObject, Enum.ToObject(objPropertyType, objDataValue), null);
                                    }
                                }
                                else if (objPropertyType == typeof(Guid)) {
                                    objPropertyInfo.SetValue(objObject, Convert.ChangeType(new Guid(objDataValue.ToString()), objPropertyType), null);
                                }
                                else if (objPropertyType == typeof(Version)) {
                                    objPropertyInfo.SetValue(objObject, new Version(objDataValue.ToString()), null);
                                }
                                else if (objPropertyType == objDataType) {
                                    objPropertyInfo.SetValue(objObject, objDataValue, null);
                                }
                                else {
                                    //等=1 为true 非1为false
                                    if (objPropertyType == typeof(Boolean)) {
                                        if (objDataValue.ToString() == "1")
                                            objDataValue = true;
                                        else
                                            objDataValue = false;
                                    }

                                    if (((objDataValue is Int32)
                                        || (objDataValue is Single)
                                        || (objDataValue is Decimal))
                                       && objPropertyType == typeof(String))
                                        objDataValue = objDataValue.ToString();
                                    objPropertyInfo.SetValue(objObject, Convert.ChangeType(objDataValue, objPropertyType), null);
                                }
                            }
                        }
                    }
                }
            }
            catch {
                throw;
            }
        }

        #endregion
    }
}
