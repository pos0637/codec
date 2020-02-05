using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Miscs
{
    /// <summary>
    /// 动态调用器
    /// </summary>
    public class DynamicInvoker
    {
        /// <summary>
        /// 调用类静态方法
        /// </summary>
        /// <param name="clazz">类型</param>
        /// <param name="arguments">参数列表</param>
        /// <param name="data">JSONRPC数据</param>
        /// <returns>返回值</returns>
        public static dynamic JsonRpcInvoke(Type clazz, Dictionary<string, object> arguments, byte[] data)
        {
            try {
                var rpc = JsonConvert.DeserializeObject<JsonRpcRequest>(Encoding.UTF8.GetString(data));
                var method = clazz.GetMethod(rpc.method);
                if (method == null) {
                    throw new ArgumentException();
                }

                var args = new List<object>();
                ParameterInfo[] parameters = method.GetParameters();
                for (int i = 0; i < parameters.Length; ++i) {
                    var type = parameters[i].ParameterType;
                    var name = parameters[i].Name;

                    try {
                        if ((arguments != null) && (arguments.ContainsKey(name))) {
                            args.Add(arguments[name]);
                            continue;
                        }

                        // 简单类型转换
                        var obj = Convert.ChangeType(rpc.parameters[name], type);
                        args.Add(obj);
                    }
                    catch {
                        try {
                            // 复杂类型转换
                            var json = JsonConvert.SerializeObject(rpc.parameters[name]);
                            var obj = Activator.CreateInstance(type);
                            obj = JsonConvert.DeserializeAnonymousType(json, obj);
                            args.Add(obj);
                        }
                        catch {
                            args.Add(null);
                        }
                    }
                }

                return method.Invoke(null, args.ToArray());
            }
            catch (Exception e) {
                Tracker.LogE(e);
                throw e;
            }
        }
    }
}
