using NettyRpc.Core.ConnectManage;
using System;

namespace NettyRpc.Core
{
    public class GlobalContext
    {
        public static GlobalContext Current = new GlobalContext();

        public ServerBootstrap HostBootstrap { get; private set; }
        public ClientBootstrap ClientBootstrap { get; private set; }


        public GlobalContext Client(ClientBootstrap clientBootstrap)
        {
            this.ClientBootstrap = clientBootstrap;
            return this;
        }

        public GlobalContext Server(ServerBootstrap hostBootstrap)
        {
            this.HostBootstrap = hostBootstrap;
            return this;
        }

    }
}
