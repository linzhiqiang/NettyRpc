using Castle.DynamicProxy;
using NettyRpc.Core.Common;
using NettyRpc.Core.Invokes;
using NettyRpc.Core.ProtoCodes;
using NettyRpc.Core.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NettyRpc.Core.DI.Interceptors
{
    /// <summary>
    /// 记录审计日志拦截器
    /// </summary>
    public class ServiceLogInterceptor : IInterceptor
    {
        Common.Logging.ILogger AuditLogger = Common.Logging.LoggerFactory.Instance.GetLogger("AuditLogger");
        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            finally
            {
                LogAuditInfoWrap(invocation);
            }

        }

        private void LogAuditInfoWrap(IInvocation invocation)
        {
            Task.Run(async () =>
            {
                await LogAuditInfo(invocation);
            });
        }

        private async Task LogAuditInfo(IInvocation invocation)
        {
            var arguments = invocation.Arguments;
            object firstP = arguments.Length > 0 ? arguments[0] : null;

            string serviceName = invocation.MethodInvocationTarget.DeclaringType.Name;
            string messageName = invocation.MethodInvocationTarget.Name;

            var requestMessage = AppLocalContext.Current.InvokeContext?.RequestMessage;
            var traceId = requestMessage?.GetExtData(ExtDataType.TraceId);

            //这里要判断ReturnValue 是否是task类型的
            object result = await GetResult(invocation);

            AuditLogger.Info($"{serviceName}.{messageName}, {traceId}, request={JsonUtils.ToJson(firstP)}, response={StringUtils.Truncation(JsonUtils.ToJson(result),100)}");
        }

        private async Task<object> GetResult(IInvocation invocation)
        {
            Type returnType = invocation.Method.ReturnType;
            object result = null;
            if (returnType == typeof(void))
            {
                result = null;
            }
            else if (returnType == typeof(Task))
            {
                result = null;
            }
            else if (returnType.GetTypeInfo().IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                dynamic dynTask = invocation.ReturnValue;
                result = await dynTask;
            }
            else
            {
                result = invocation.ReturnValue;
            }

            return result;
        }
    }
}
