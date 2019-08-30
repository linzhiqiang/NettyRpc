using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.Common.Logging.ConsoleLogger
{
    public class ConsoleLoggerProvider : ILoggerProvider
    {
        public ConsoleLoggerProvider(ConsoleLogLevel level)
        {
            ConsoleLogger._logLevel = level;
        }

        public ILogger GetLogger(string name)
        {
            return ConsoleLogger.Instance;
        }
    }
}
