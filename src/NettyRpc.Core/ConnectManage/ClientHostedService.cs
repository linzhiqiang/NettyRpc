using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NettyRpc.Core.ConnectManage
{
    public class ClientHostedService : IHostedService
    {
        ClientBootstrap _clientBootstrap;

        public ClientHostedService(ClientBootstrap clientBootstrap)
        {
            _clientBootstrap = clientBootstrap;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _clientBootstrap.Start();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
