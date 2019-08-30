using Microsoft.Extensions.DependencyInjection;
using NettyRpc.Core.Common;
using NettyRpc.Core.Config;
using NettyRpc.Core.ConnectManage;
using NettyRpc.Core.Serializes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;
using Castle.DynamicProxy;
using NettyRpc.Core.ChannelHandlers;
using NettyRpc.Core.Invokes;
using NettyRpc.Core.DI.Interceptors;
using Microsoft.Extensions.Hosting;

namespace NettyRpc.Core.DI
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddService(this IServiceCollection services, ServerOptions serverOptions, ClientOptions clientOptions)
        {
            services.AddSingleton<ServerOptions>(serverOptions);
            services.AddSingleton<ServerBootstrap>();
            //services.AddSingleton<ClientConnectManage>();
            services.AddSingleton<ServerManage>();
            services.AddTransient<DispatchServerHandler>();
            //services.AddTransient<DispatchClientHandler>();

            // services.AddSingleton<InvokeProcessor>();
            // services.AddSingleton<InvokeRemote>();

            services.AddSingleton<InvokeLocal>();
            services.AddSingleton<ServiceInvokeInterceptor>();
            services.AddSingleton<ServiceLogInterceptor>();

            services.AddClient(clientOptions);
            services.AddSingleton<IHostedService, ServerHostedService>();//启动入口
            //注册服务
            AddLocalService(services, serverOptions.DllPrefix);
           // services.AddSingleton<RpcServiceProxyFactory>();
            //services.AddSingleton<ProxyGenerator>();
            return services;
        }

        public static IServiceCollection AddClient(this IServiceCollection services, ClientOptions clientOptions)
        {
            services.AddSingleton<IHostedService, ClientHostedService>();
            services.AddSingleton<ClientOptions>(clientOptions);
            services.AddSingleton<ClientBootstrap>();
            services.AddSingleton<ClientConnectManage>();
            services.AddSingleton<ServerManage>();
            services.AddTransient<DispatchClientHandler>();
            services.AddSingleton<RpcClientProxyFactory>();
            services.AddSingleton<RpcServiceProxyFactory>();
            services.AddSingleton<ProxyGenerator>();
            services.AddSingleton<InvokeProcessor>();
            services.AddSingleton<InvokeRemote>();
            services.AddSingleton<ClientProxyInterceptor>();

            return services;
        }

        private static void AddLocalService(IServiceCollection services, string[] dllPrefix)
        {
            List<Assembly> Assemblys = new List<Assembly>();

            if (dllPrefix == null) dllPrefix = new string[0];
            foreach (var item in dllPrefix)
            {
                //var temp = item.EndsWith(".dll") ? item : $"{item}.dll";
                //Assemblys.Add(Assembly.LoadFrom(Path.Combine(AppContext.BaseDirectory, temp)));

                var dllFiles = Directory.GetFiles(string.Concat(AppContext.BaseDirectory, ""), $"{item}.dll", SearchOption.AllDirectories);
                foreach (var realFile in dllFiles)
                {
                    Assemblys.Add(Assembly.LoadFrom(realFile));
                }
            }

            foreach (var assembly in Assemblys)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    var typeInfo = type.GetTypeInfo();
                    //var rpcSericeAttr = typeInfo.GetCustomAttribute(typeof(RpcServiceAttribute),true);
                    if (type.IsClass && !type.IsAbstract && typeof(IRpcService).IsAssignableFrom(type))
                    {
                        var interfaceType = type.GetInterfaces().FirstOrDefault(x => x.GetCustomAttribute(typeof(RpcServiceAttribute)) != null);
                        if (interfaceType == null) throw new Exception($"类型{type.FullName}没有定义接口");
                        services.AddSingleton(interfaceType, type);

                        ServiceTypeManage.Instance.RegisterType(type.Name, interfaceType);
                    }

                }
            }



        }
    }
}
