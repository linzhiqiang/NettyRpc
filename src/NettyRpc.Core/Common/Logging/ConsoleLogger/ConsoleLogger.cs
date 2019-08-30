using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.Common.Logging.ConsoleLogger
{
    public enum ConsoleLogLevel
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warning = 4,
        Error = 8,
        Fatal = 16
    }

    public class ConsoleLogger : ILogger
    {
        internal static ConsoleLogLevel _logLevel = ConsoleLogLevel.Trace;

        public static ConsoleLogger Instance = new ConsoleLogger();

        public void Trace(string msg, Exception exception = null)
        {
            writer(ConsoleLogLevel.Trace, msg, exception);
        }

        public void Debug(string msg, Exception exception = null)
        {
            writer(ConsoleLogLevel.Debug, msg, exception);
        }

        public void Info(string msg, Exception exception = null)
        {
            writer(ConsoleLogLevel.Info, msg, exception);
        }

        public void Warn(string msg, Exception exception = null)
        {
            writer(ConsoleLogLevel.Warning, msg, exception);
        }

        public void Error(string msg, Exception exception = null)
        {
            writer(ConsoleLogLevel.Error, msg, exception);
        }

        public void Fatal(string msg, Exception exception = null)
        {
            writer(ConsoleLogLevel.Fatal, msg, exception);
        }

        private void writer(ConsoleLogLevel level, string message, Exception exception = null)
        {
            if (IsEnabled(level))
            {
                string exceptionMsg = string.Empty;
                if (exception != null)
                {
                    exceptionMsg = $", {exception.Message},{exception.StackTrace}";
                }

                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}[{level.ToString()}], {message}{exceptionMsg}");
            }
        }

        private bool IsEnabled(ConsoleLogLevel logLevel)
        {
            return logLevel >= _logLevel;
        }

    }
}
