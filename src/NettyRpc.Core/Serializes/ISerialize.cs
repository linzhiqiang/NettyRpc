using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.Serializes
{
    public interface ISerialize
    {
        byte[] Serialize(object obj);

        T Deserialize<T>(byte[] data);
        object Deserialize(Type type, byte[] data);
    }
}
