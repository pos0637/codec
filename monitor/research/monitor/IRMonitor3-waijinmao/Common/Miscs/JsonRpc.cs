using Newtonsoft.Json;
using System.Collections.Generic;

namespace Miscs
{
    public class JsonRpcRequest
    {
        [JsonProperty(PropertyName = "jsonrpc")]
        public string version;

        [JsonProperty(PropertyName = "method")]
        public string method;

        [JsonProperty(PropertyName = "params")]
        public Dictionary<string, object> parameters;

        [JsonProperty(PropertyName = "id")]
        public string id;
    }

    public class JsonRpcResult
    {
        [JsonProperty(PropertyName = "jsonrpc")]
        public string version;

        [JsonProperty(PropertyName = "result")]
        public Dictionary<string, object> result;

        [JsonProperty(PropertyName = "id")]
        public string id;
    }

    public class JsonRpcError
    {
        [JsonProperty(PropertyName = "jsonrpc")]
        public string version;

        [JsonProperty(PropertyName = "error")]
        public Error error;

        [JsonProperty(PropertyName = "id")]
        public string id;
    }

    public class Error
    {
        [JsonProperty(PropertyName = "code")]
        public int code;

        [JsonProperty(PropertyName = "message")]
        public string message;

        [JsonProperty(PropertyName = "data")]
        public Dictionary<string, object> data;
    }
}
