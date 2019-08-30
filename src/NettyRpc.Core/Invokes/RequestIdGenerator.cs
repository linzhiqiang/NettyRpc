using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NettyRpc.Core.Invokes
{
    public class RequestIdGenerator
    {
        private RequestIdGenerator() { }

        public static RequestIdGenerator Instance = new RequestIdGenerator();

        private int RequestId = 0;

        public int GetNewRequestId()
        {
            int id = Interlocked.Increment(ref this.RequestId);
            return id;
        }

    }
}
