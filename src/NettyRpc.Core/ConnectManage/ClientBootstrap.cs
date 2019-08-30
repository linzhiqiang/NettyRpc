using DotNetty.Codecs;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using NettyRpc.Core.ChannelHandlers;
using NettyRpc.Core.Config;
using NettyRpc.Core.ProtoCodes;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace NettyRpc.Core.ConnectManage
{
    public class ClientBootstrap
    {

        Bootstrap Bootstrap = null;

        public ClientOptions _clientOptions = null;
        IServiceProvider _serviceProvider = null;
        public ClientBootstrap(ClientOptions clientOptions, IServiceProvider serviceProvider)
        {
            _clientOptions = clientOptions;
            _serviceProvider = serviceProvider;
            var group = new MultithreadEventLoopGroup(this._clientOptions.EventLoopCount);
            Bootstrap = new Bootstrap();
            Bootstrap
                .Group(group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast("framing-dec1", new LengthFieldBasedFrameDecoder(int.MaxValue, 4, 4, -8, 0));
                    pipeline.AddLast("framing-dec2", new MessageDecoder());

                    pipeline.AddLast("framing-enc1", new MessageEncoder());
                    pipeline.AddLast("Heartbeat", new IdleStateHandler(0, 0, _clientOptions.HeartseatInterval));
                    pipeline.AddLast("Heartbeat1", new HeartbeatClientHandler());
                    pipeline.AddLast("echo", _serviceProvider.GetService< DispatchClientHandler >());
                }));

            GlobalContext.Current.Client(this);
        }

        public async Task Start()
        {
            await Task.Run(() =>
            {

            });
        }
        public Task<IChannel> ConnectAsync(EndPoint remoteAddress)
        {
            return Bootstrap.ConnectAsync(remoteAddress);
        }

        public async Task<IChannel> ConnectAsync(string serverAddress)
        {
            string[] arr = serverAddress.Split(new char[] { ':' });
            //return Bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(arr[0]), int.Parse(arr[1])));

            var channel = await Bootstrap.ConnectAsync(IPAddress.Parse(arr[0]), int.Parse(arr[1]));
            return channel;
        }




    }
}
