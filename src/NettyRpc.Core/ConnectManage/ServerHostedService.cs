using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NettyRpc.Core.ConnectManage
{
    public class ServerHostedService : IHostedService
    {
        ServerBootstrap _hostBootstrap;
        ClientBootstrap _clientBootstrap;

        ServerManage _serverManage;
        public ServerHostedService(ServerBootstrap hostBootstrap, ClientBootstrap clientBootstrap, ServerManage serverManage)
        {
            _hostBootstrap = hostBootstrap;
            _clientBootstrap = clientBootstrap;

            _serverManage = serverManage;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _hostBootstrap.Start();
            await _clientBootstrap.Start();

            //读取配置
            //注册服务
            
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
