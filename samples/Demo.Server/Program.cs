using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NettyRpc.Core.Common.Logging;
using NettyRpc.Core.Common.Logging.ConsoleLogger;
using NettyRpc.Core.Config;
using NettyRpc.Core.DI;
using System;

namespace Demo.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("请输入端口：");
            int port = int.Parse(Console.ReadLine());


            var host = new HostBuilder()
                .ConfigureHostConfiguration(builder =>
                {
                    builder.AddEnvironmentVariables(prefix: "DesignDemo_");
                })
                 .ConfigureAppConfiguration((hostContext, config) =>
                 {
                     //var environmentName = hostContext.HostingEnvironment.EnvironmentName;
                 })
                .ConfigureLogging((context, factory) =>
                {
                   LoggerFactory.Instance.AddProvider(new ConsoleLoggerProvider(ConsoleLogLevel.Trace));

                })
                .ConfigureServices((context, services) =>
                {
                    ServerOptions serverOptions = new ServerOptions
                    {
                        Port = port,
                         DllPrefix = new string[] { "Demo.*ServiceImpl" }
                    };
                    ClientOptions clientOptions = new ClientOptions { SerializeType = 0, EventLoopCount=2 };
                    services.AddService(serverOptions, clientOptions);

                    services.AddSingleton<IHostedService, StartService>();
                    
                });


            host.RunConsoleAsync().Wait();
            Console.WriteLine("服务已退出");
        }
    }
}
