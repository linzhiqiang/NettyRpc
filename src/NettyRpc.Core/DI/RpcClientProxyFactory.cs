using Castle.DynamicProxy;
using NettyRpc.Core.Common;
using NettyRpc.Core.Config;
using NettyRpc.Core.Invokes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NettyRpc.Core.DI
{
    /// <summary>
    /// 独立客户端 客户端代理容器
    /// </summary>
    public class RpcClientProxyFactory
    {
        private ProxyGenerator _proxyGenerator;
        private ClientProxyInterceptor _clientProxyInterceptor;
        public RpcClientProxyFactory(ProxyGenerator proxyGenerator, ClientProxyInterceptor clientProxyInterceptor)
        {
            _proxyGenerator = proxyGenerator;
            _clientProxyInterceptor = clientProxyInterceptor;
        }

        public T GetService<T>() where T : class
        {
            return _proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(_clientProxyInterceptor);
        }
    }

    public class ClientProxyInterceptor : IInterceptor
    {
        private InvokeProcessor _invokeProcessor;
        ClientOptions _clientOptions;
        public ClientProxyInterceptor(InvokeProcessor invokeProcessor, ClientOptions clientOptions)
        {
            _invokeProcessor = invokeProcessor;
            _clientOptions = clientOptions;
        }
        public void Intercept(IInvocation invocation)
        {
            try
            {
                var result = _invokeProcessor.InvokeRemoteMethod(invocation, _clientOptions.SerializeType);
                invocation.ReturnValue = result;
            }
            catch (AggregateException ex)
            {
                throw ex.Flatten().InnerException;
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
