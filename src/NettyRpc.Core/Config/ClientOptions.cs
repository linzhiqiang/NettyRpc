using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.Config
{
   public class ClientOptions
    {
        public ClientOptions()
        {
            this.EventLoopCount = Environment.ProcessorCount * 2;
            this.PerClientConnectCount = 2;
            this.HeartseatInterval = 5 * 60;
            this.Timeout = 5 * 1000;
            this.SerializeType = 0;
            this.AutoReConnectInterval = 15;
            this.AutoReConnectMaxNumber = 360;
        }


        /// <summary>
        /// 每个客户端连接数
        /// </summary>
        public int EventLoopCount { get; set; }

        /// <summary>
        /// 每个客户端连接数
        /// </summary>
        public int PerClientConnectCount { get; set; }

        /// <summary>
        /// 客户端心跳频率 秒
        /// </summary>
        public int HeartseatInterval { get; set; }

        /// <summary>
        /// 调用超时时间(毫秒)
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// 序列化类型 0:proto,1:json,服务端的客户端不用配置，依赖请求的类型
        /// </summary>
        public byte SerializeType { get; set; }

        /// <summary>
        /// 自动重连间隔 秒，小于等于0即是不自动重连，默认15秒
        /// </summary>
        public int AutoReConnectInterval { get; set; }

        /// <summary>
        /// 自动重连最大次数 默认360次
        /// </summary>
        public int AutoReConnectMaxNumber { get; set; }
    }
}
