using NettyRpc.Core.Common.Logging.NullLogger;
using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.Common.Logging
{
    public class LoggerFactory
    {
        public static LoggerFactory Instance = new LoggerFactory();
        private LoggerFactory() { }


        private ILoggerProvider LoggerProvider = new NullLoggerProvider();

        public ILogger GetLogger<T>()
        {
            return GetLogger(typeof(T).FullName);
        }

        public ILogger GetLogger(string name)
        {
            return LoggerProvider.GetLogger(name);
        }

        public void AddProvider(ILoggerProvider loggerProvide)
        {
            this.LoggerProvider = loggerProvide;
        }

    }
}
