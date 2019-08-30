using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.Common
{
    [ProtoContract]
    public class RpcResult
    {
        [ProtoMember(1)]
        public int Code { get; set; }

        [ProtoMember(2)]
        public string Message { get; set; }
    }

    [ProtoContract]
    public class RpcResult<T> : RpcResult
    {
        [ProtoMember(3)]
        public T Data { get; set; }
    }

}
