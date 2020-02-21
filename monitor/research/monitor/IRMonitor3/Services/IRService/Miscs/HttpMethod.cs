using Common;
using System;
using System.Net.Http;

namespace IRService.Miscs
{
    /// <summary>
    /// HTTP网络请求库
    /// </summary>
    public static class HttpMethod
    {
        /// <summary>
        /// GET方法
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="timeout">超时(毫秒)</param>
        /// <returns>返回数据</returns>
        public static string Get(string url, int timeout = 3000)
        {
            try {
                var client = new HttpClient() {
                    Timeout = TimeSpan.FromMilliseconds(timeout)
                };

                return client.GetStringAsync(url).Result;
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
        }

        /// <summary>
        /// POST方法
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="data">参数</param>
        /// <param name="timeout">超时(毫秒)</param>
        /// <returns>返回数据</returns>
        public static string Post(string url, string data, int timeout = 3000)
        {
            try {
                var client = new HttpClient() {
                    Timeout = TimeSpan.FromMilliseconds(timeout)
                };
                var content = new StringContent(data);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                var result = client.PostAsync(url, content);
                if (result.Result.StatusCode == System.Net.HttpStatusCode.OK) {
                    return result.Result.Content.ReadAsStringAsync().Result;
                }
                else {
                    return null;
                }
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
        }

        /// <summary>
        /// PUT方法
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="data">参数</param>
        /// <param name="timeout">超时(毫秒)</param>
        /// <returns>返回数据</returns>
        public static string Put(string url, string data, int timeout = 3000)
        {
            try {
                var client = new HttpClient() {
                    Timeout = TimeSpan.FromMilliseconds(timeout)
                };

                var content = new StringContent(data);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                var result = client.PutAsync(url, content);
                if (result.Result.StatusCode == System.Net.HttpStatusCode.OK) {
                    return result.Result.Content.ReadAsStringAsync().Result;
                }
                else {
                    return null;
                }
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
        }
    }
}
