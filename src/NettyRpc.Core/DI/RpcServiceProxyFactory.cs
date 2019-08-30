using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using NettyRpc.Core.DI.Interceptors;

namespace NettyRpc.Core.DI
{
    /// <summary>
    /// 服务端 服务代理类容器
    /// </summary>
    public class RpcServiceProxyFactory
    {
        /// <summary>
        /// 代理类对象缓存
        /// </summary>
        private ConcurrentDictionary<Type, object> _cache = new ConcurrentDictionary<Type, object>();
        private ProxyGenerator _proxyGenerator;

        private IServiceProvider _serviceProvider;

        private ServiceInvokeInterceptor _serviceInvokeInterceptor;
        private ServiceLogInterceptor _serviceLogInterceptor;
        public RpcServiceProxyFactory(IServiceProvider serviceProvider, ProxyGenerator proxyGenerator)
        {
            _serviceProvider = serviceProvider;
            _proxyGenerator = proxyGenerator;

            _serviceInvokeInterceptor = serviceProvider.GetService<ServiceInvokeInterceptor>();// serviceInvokeInterceptor;
            _serviceLogInterceptor = serviceProvider.GetService<ServiceLogInterceptor>();//serviceLogInterceptor;
        }

        public object GetService(Type type)
        {
            if (_cache.TryGetValue(type, out object value))
            {
                return value;
            }
            var service = _serviceProvider.GetService(type);

            var interceptors = new IInterceptor[] { _serviceLogInterceptor, _serviceInvokeInterceptor };
            if (service != null) //本地有实现
            {
                //创建代理类
                //加入缓存
                var proxy = _proxyGenerator.CreateInterfaceProxyWithTarget(type, service, interceptors); //服务端拦截器
                _cache.TryAdd(type, proxy);
                return proxy;
            }
            else //本地没有实现，即是客户端调用
            {
                //创建代理类
                //加入缓存
                var proxy = _proxyGenerator.CreateInterfaceProxyWithoutTarget(type, interceptors); //服务端拦截器
                _cache.TryAdd(type, proxy);
                return proxy;
            }

        }
        public T GetService<T>() where T : class
        {
            return GetService(typeof(T)) as T;
        }
    }
}
