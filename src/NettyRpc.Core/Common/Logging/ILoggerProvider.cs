using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.Common.Logging
{
    public interface ILoggerProvider
    {
        ILogger GetLogger(string name);
    }
}
