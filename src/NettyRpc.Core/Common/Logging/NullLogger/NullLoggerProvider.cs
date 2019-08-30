using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.Common.Logging.NullLogger
{
    public class NullLoggerProvider : ILoggerProvider
    {
        public ILogger GetLogger(string name)
        {
            return NullLogger.Instance;
        }
    }
}
