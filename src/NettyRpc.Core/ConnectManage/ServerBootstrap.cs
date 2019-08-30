using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using NettyRpc.Core.ChannelHandlers;
using NettyRpc.Core.Config;
using NettyRpc.Core.ProtoCodes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using DotNetty.Handlers.Timeout;

namespace NettyRpc.Core.ConnectManage
{
    public class ServerBootstrap
    {
        DotNetty.Transport.Bootstrapping.ServerBootstrap Bootstrap = null;
        MultithreadEventLoopGroup BossGroup = null;
        MultithreadEventLoopGroup WorkerGroup = null;
       
        IChannel ServerChannel = null;


        ServerOptions _serverOptions = null;
        ClientOptions _clientOptions = null;
        IServiceProvider _serviceProvider = null;

        public ServerBootstrap(ServerOptions serverOptions, ClientOptions clientOptions, IServiceProvider serviceProvider)
        {
            _serverOptions = serverOptions;
            _clientOptions = clientOptions;
            _serviceProvider = serviceProvider;

            BossGroup = new MultithreadEventLoopGroup(_serverOptions.BossEventLoopCount);
            WorkerGroup = new MultithreadEventLoopGroup(_serverOptions.WorkerEventLoopCount);

            Bootstrap = new DotNetty.Transport.Bootstrapping.ServerBootstrap();
            Bootstrap
                    .Group(BossGroup, WorkerGroup)
                    .Channel<TcpServerSocketChannel>()
                    .Option(ChannelOption.SoBacklog, 10240)
                    .Option(ChannelOption.SoReuseaddr, true)
                    // .Handler(new LoggingHandler("SRV-LSTN"))
                    .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;

                        //pipeline.AddLast(new LoggingHandler("SRV-CONN"));

                        pipeline.AddLast("framing-dec1", new LengthFieldBasedFrameDecoder(int.MaxValue, 4, 4, -8, 0));
                        pipeline.AddLast("framing-dec2", new MessageDecoder());

                        pipeline.AddLast("framing-enc1", new MessageEncoder());

                        pipeline.AddLast("IdleState", new IdleStateHandler(0, 0, _clientOptions.HeartseatInterval+10));
                        pipeline.AddLast("Heartbeat1", new HeartbeatServerHandler());//new HeartbeatServerHandler()
                        pipeline.AddLast("echo2", _serviceProvider.GetService<DispatchServerHandler>());
                    }));

            GlobalContext.Current.Server(this);

        }

        public async Task<int> Start()
        {
            int port = _serverOptions.Port;
            ServerChannel = await Bootstrap.BindAsync(port);

            return port;
        }

        public async Task CloseAsync()
        {
            await ServerChannel.CloseAsync();
            await Task.WhenAll(
                    BossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                    WorkerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
        }
    }
}
