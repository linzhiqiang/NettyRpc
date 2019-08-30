using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.Common.Logging
{
    public interface ILogger
    {
        void Trace(string msg, Exception exception = null);

        void Debug(string msg, Exception exception = null);

        void Info(string msg, Exception exception = null);

        void Warn(string msg, Exception exception = null);

        void Error(string msg, Exception exception = null);

        void Fatal(string msg, Exception exception = null);
    }
}
