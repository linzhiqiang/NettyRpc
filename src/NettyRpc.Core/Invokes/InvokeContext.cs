using NettyRpc.Core.Common;
using NettyRpc.Core.ProtoCodes;
using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;

namespace NettyRpc.Core.Invokes
{
    public class InvokeContext
    {
        /// <summary>
        /// 本次上下文调用是否第一次（首次请求进入的服务）->调用本地方法
        /// </summary>
        public bool IsFirstCall { get; set; }


        /// <summary>
        /// 首次请求message
        /// </summary>
        public Message RequestMessage { get; set; }

        #region 静态

        //public static InvokeContext Current
        //{
        //    get
        //    {
        //        return (InvokeContext)CallContext.GetData(nameof(InvokeContext.Current));
        //    }
        //    set
        //    {
        //        CallContext.SetData(nameof(InvokeContext.Current), value);
        //    }
        //}

        #endregion


    }

}
