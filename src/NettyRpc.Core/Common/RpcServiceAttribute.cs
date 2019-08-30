using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.Common
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class RpcServiceAttribute:Attribute
    {
        public RpcServiceAttribute()
        {
        }
        /// <summary>
        /// 超时时间 单位秒 默认获取客户端全局配置
        /// </summary>
        public int Timeout { get; set; }
    }
}
