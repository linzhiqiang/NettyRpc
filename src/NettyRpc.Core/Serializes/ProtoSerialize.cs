using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static ProtoBuf.Serializer;

namespace NettyRpc.Core.Serializes
{
    public class ProtoSerialize : ISerialize
    {
        public T Deserialize<T>(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return default(T);
            }
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(data, 0, data.Length);
                ms.Position = 0;
                
                return ProtoBuf.Serializer.Deserialize<T>(ms);
            }
        }

        public object Deserialize(Type type, byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(data, 0, data.Length);
                ms.Position = 0;
                return ProtoBuf.Serializer.Deserialize(type, ms);
            }
        }

        public byte[] Serialize(object obj)
        {
            if (obj == null) return null;
            using (MemoryStream ms = new MemoryStream())
            {
                //ProtoBuf.Serializer.Serialize<T>(ms, obj);
                NonGeneric.Serialize(ms, obj);
                byte[] result = new byte[ms.Length];

                ms.Position = 0;
                //将流中的内容读取到二进制数组中
                ms.Read(result, 0, result.Length);
                return result;
            }
        }
    }
}
