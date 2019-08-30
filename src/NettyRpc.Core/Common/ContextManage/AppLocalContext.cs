using NettyRpc.Core.Invokes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;

namespace NettyRpc.Core.Common
{
    public class AppLocalContext
    {
        public static AppLocalContext Current = new AppLocalContext();


        static AsyncLocal<ConcurrentDictionary<string, object>> _asyncLocalItems = new AsyncLocal<ConcurrentDictionary<string, object>>();
        public IDictionary<string, object> Items
        {
            get
            {
                if (_asyncLocalItems.Value == null) _asyncLocalItems.Value = new ConcurrentDictionary<string, object>();
                return _asyncLocalItems.Value;
            }
        }

        static AsyncLocal<InvokeContext> InvokeContextAsyncLocal = new AsyncLocal<InvokeContext>();

        public InvokeContext InvokeContext
        {
            get { return InvokeContextAsyncLocal.Value; }
            set { InvokeContextAsyncLocal.Value = value; }
        }
    }


}
