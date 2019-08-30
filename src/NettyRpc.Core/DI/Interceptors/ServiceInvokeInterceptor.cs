using Castle.DynamicProxy;
using NettyRpc.Core.Common;
using NettyRpc.Core.ConnectManage;
using NettyRpc.Core.Invokes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.DI.Interceptors
{
    /// <summary>
    /// 服务端拦截
    /// </summary>
    public class ServiceInvokeInterceptor : IInterceptor
    {
        Common.Logging.ILogger Logger = Common.Logging.LoggerFactory.Instance.GetLogger<ServiceInvokeInterceptor>();

        private InvokeProcessor _invokeProcessor;
        private ServerManage _serverManage;
        public ServiceInvokeInterceptor(InvokeProcessor invokeProcessor, ServerManage serverManage)
        {
            _invokeProcessor = invokeProcessor;
            _serverManage = serverManage;
        }

        public void Intercept(IInvocation invocation)
        {
            //请求入口 肯定是本地调用了
            if (AppLocalContext.Current.InvokeContext.IsFirstCall)
            {
                AppLocalContext.Current.InvokeContext.IsFirstCall = false;
                invocation.Proceed();
                return;
            }

            if (IsLocalService(invocation))//本地调用
            {
                invocation.Proceed();
                return;
            }

            //远程调其他服务
            try
            {
                var result = _invokeProcessor.InvokeRemoteMethod(invocation, AppLocalContext.Current.InvokeContext.RequestMessage.SerializeType);
                invocation.ReturnValue = result;
                return;
            }
            catch (AggregateException ex)
            {
                throw ex.Flatten().InnerException;
            }
        }

        private bool IsLocalService(IInvocation invocation)
        {
            string serviceName = invocation.MethodInvocationTarget.DeclaringType.Name;
            string messageName = invocation.MethodInvocationTarget.Name;
            bool isLocalService = _serverManage.IsLocalService(serviceName, messageName);

            return isLocalService;
        }



    }
}
