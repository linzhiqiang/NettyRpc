using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.Exceptions
{
   public class RpcException:Exception
    {
        public int Code { get; set; }

        public RpcException(int code, string message) : base(message)
        {
            this.Code = code;
        }
    }
}
