using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.Common.Logging.NullLogger
{
    public class NullLogger : ILogger
    {
        public static NullLogger Instance = new NullLogger();
        public void Trace(string msg, Exception exception = null)
        {
        }

        public void Debug(string msg, Exception exception = null)
        {

        }

        public void Info(string msg, Exception exception = null)
        {

        }

        public void Warn(string msg, Exception exception = null)
        {

        }

        public void Error(string msg, Exception exception = null)
        {

        }

        public void Fatal(string msg, Exception exception = null)
        {

        }
    }
}
