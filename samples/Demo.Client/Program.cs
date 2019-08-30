using Microsoft.Extensions.Hosting;
using System;
using  Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NettyRpc.Core.DI;
using NettyRpc.Core.Config;
using NettyRpc.Core.Common.Logging;
using NettyRpc.Core.Common.Logging.ConsoleLogger;

namespace Demo.Client
{
    class Program
    {
        static void Main(string[] args)
        {
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
                    //ConsoleGlobalConfiguration.Configuration.UseConsoleLog();
                    //NLogLogGlobalConfiguration.Configuration.UseNLog();
                    LoggerFactory.Instance.AddProvider(new ConsoleLoggerProvider(ConsoleLogLevel.Trace));

                })
                .ConfigureServices((context, services) =>
                {
                    ClientOptions clientOptions = new ClientOptions {  SerializeType=1, Timeout =3000};
                    services.AddClient(clientOptions);
                    services.AddSingleton<IHostedService, StartService>(); //启动入口
                });


            host.RunConsoleAsync().Wait();

            Console.WriteLine("服务已退出");
        }
    }
}
