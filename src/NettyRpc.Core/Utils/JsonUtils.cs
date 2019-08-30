using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.Utils
{
    public static class JsonUtils
    {
        private static JsonSerializerSettings _jsonSerializer = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        public static string ToJson(object obj)
        {
            // 
            if (obj == null) return string.Empty;
            return JsonConvert.SerializeObject(obj);
        }

        public static T FromJson<T>(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(str, _jsonSerializer);
        }

        public static object FromJson(Type type,string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            return JsonConvert.DeserializeObject(str, type, _jsonSerializer);
        }


        public static Dictionary<string, T> FromJsonToDict<T>(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new Dictionary<string, T>();
            }
            var jsonDict = JsonConvert.DeserializeObject<Dictionary<string, T>>(str);
            if (jsonDict == null) jsonDict = new Dictionary<string, T>();
            return jsonDict;
        }
    }
}
