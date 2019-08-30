using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.Config
{
   public  class ServerOptions
    {
        public ServerOptions()
        {
            //从配置文件读取服务端配置

            this.BossEventLoopCount = 2;
            this.WorkerEventLoopCount = Environment.ProcessorCount * 2;
            this.Port = 8007;
        }

        public int BossEventLoopCount { get; set; }

        public int WorkerEventLoopCount { get; set; }

        public int Port { get; set; }

        public string[] DllPrefix { get; set; }


    }
}
