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
}
