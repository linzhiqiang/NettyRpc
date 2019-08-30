using Microsoft.Extensions.Hosting;
using NettyRpc.Core.ConnectManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Server
{
    public class StartService : IHostedService
    {
        ServerManage _serverManage;
        public StartService(ServerManage serverManage)
        {
            _serverManage = serverManage;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _serverManage.RegisterService("UserService", "*", new List<string> { "127.0.0.1:8007" });//new List<string> { "127.0.0.1:8007", "127.0.0.1:8008", "127.0.0.1:8009", "127.0.0.1:8010" }
            _serverManage.RegisterService("ProductService", "*", new List<string> { "127.0.0.1:8008" });//new List<string> { "127.0.0.1:8007", "127.0.0.1:8008", "127.0.0.1:8009", "127.0.0.1:8010" }
            //_serverManage.RegisterService("OrderService", "*", new List<string> { "127.0.0.1:8007", "127.0.0.1:8008" });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
