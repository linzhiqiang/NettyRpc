using NettyRpc.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.Serializes
{
    public class JsonSerialize : ISerialize
    {
        private static readonly Encoding encoding = Encoding.UTF8;


        public T Deserialize<T>(byte[] data)
        {
            if (data != null && data.Length > 0)
            {
                string json = encoding.GetString(data);
                return JsonUtils.FromJson<T>(json);
            }
            return default(T);
        }

        public object Deserialize(Type type, byte[] data)
        {
            if (data != null && data.Length > 0)
            {
                string json = encoding.GetString(data);
                return JsonUtils.FromJson(type,json);
            }
            return null;
        }

        public byte[] Serialize(object obj)
        {
            string json = string.Empty;
            if (obj != null)
            {
                json = JsonUtils.ToJson(obj);
            }
            return encoding.GetBytes(json);


        }
    }
}
