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
    public static class DynamicInvoker
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
                var requestJson = Encoding.UTF8.GetString(data);
                Tracker.LogD($"JsonRpcInvoke request: {requestJson}");

                var request = JsonConvert.DeserializeObject<JsonRpcRequest>(requestJson);
                var method = clazz.GetMethod(request.method);
                if (method == null) {
                    throw new ArgumentException();
                }

                var args = new List<object>();
                ParameterInfo[] parameters = method.GetParameters();
                for (int i = 0; i < parameters.Length; ++i) {
                    var type = parameters[i].ParameterType;
                    var name = parameters[i].Name;

                    try {
                        if ((arguments != null) && arguments.ContainsKey(name)) {
                            args.Add(arguments[name]);
                            continue;
                        }

                        // 简单类型转换
                        var obj = Convert.ChangeType(request.parameters[name], type);
                        args.Add(obj);
                    }
                    catch {
                        try {
                            // 复杂类型转换
                            var json = JsonConvert.SerializeObject(request.parameters[name]);
                            var obj = Activator.CreateInstance(type);
                            obj = JsonConvert.DeserializeAnonymousType(json, obj);
                            args.Add(obj);
                        }
                        catch {
                            args.Add(null);
                        }
                    }
                }

                try {
                    dynamic result = method.Invoke(null, args.ToArray());
                    if (result == null) {
                        Tracker.LogD("JsonRpcInvoke response: null");
                        return null;
                    }

                    if (Type.GetType(result) != typeof(Dictionary<string, object>)) {
                        result = new Dictionary<string, object>() { { "data", result } };
                    }

                    var JsonRpcResult = new JsonRpcResult() {
                        version = request.version,
                        result = result,
                        id = request.id
                    };

                    var response = JsonConvert.SerializeObject(JsonRpcResult);
                    Tracker.LogD($"JsonRpcInvoke response: {response}");
                    return Encoding.UTF8.GetBytes(response);
                }
                catch (Exception e) {
                    Tracker.LogE(e);
                    var JsonRpcError = new JsonRpcError() {
                        version = request.version,
                        error = new Error() {
                            code = -32000,
                            message = e.Message
                        },
                        id = request.id
                    };

                    var response = JsonConvert.SerializeObject(JsonRpcError);
                    Tracker.LogD($"JsonRpcInvoke response: {response}");
                    return Encoding.UTF8.GetBytes(response);
                }
            }
            catch (Exception e) {
                Tracker.LogE(e);
                throw e;
            }
        }
    }
}
