using Castle.DynamicProxy;
using NettyRpc.Core.Common;
using NettyRpc.Core.Config;
using NettyRpc.Core.DI;
using NettyRpc.Core.Exceptions;
using NettyRpc.Core.ProtoCodes;
using NettyRpc.Core.Serializes;
using NettyRpc.Core.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NettyRpc.Core.Invokes
{
    public class InvokeProcessor
    {
        private MethodInfo InvokeRemoteRequestMethod = typeof(InvokeProcessor).GetMethod("InvokeRemoteRequest", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        private MethodInfo InvokeRemoteRequestWithNoResultMethod = typeof(InvokeProcessor).GetMethod("InvokeRemoteRequestWithNoResult", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        private IServiceProvider _serviceProvider;
        private InvokeRemote _invokeRemote;
        private ClientOptions _clientOptions;

        public InvokeProcessor(IServiceProvider serviceProvider, InvokeRemote invokeRemote, ClientOptions clientOptions)
        {
            _serviceProvider = serviceProvider;
            //_rpcServiceProxyFactory = rpcServiceProxyFactory;
            _invokeRemote = invokeRemote;
            _clientOptions = clientOptions;
        }

        public object InvokeRemoteMethod(IInvocation invocation, byte serializeType)
        {
            string serviceName = invocation.Method.DeclaringType.Name.TrimStart('I');
            string messageName = invocation.Method.Name;
            var arguments = invocation.Arguments;
            object firstP = arguments.Length > 0 ? arguments[0] : null;
            MethodInfo method = invocation.Method;
            Type returnType = method.ReturnType;

            RpcServiceAttribute rpcSericeAttr = invocation.Method.DeclaringType.GetCustomAttribute<RpcServiceAttribute>(true);
            var timeout = rpcSericeAttr != null && rpcSericeAttr.Timeout>0 ? rpcSericeAttr.Timeout : _clientOptions.Timeout; //可以从接口特性上解析，没有配置才取默认配置
            return InvokeRemoteMethod(serviceName, invocation.Method, firstP, timeout, serializeType);
        }

        public object InvokeRemoteMethod(string serviceName, MethodInfo methodInfo, object argument, int timeout, byte serializeType)
        {
            string messageName = methodInfo.Name;
            object firstP = argument;
            Type returnType = methodInfo.ReturnType;

            object result = null;
            if (returnType == typeof(void))
            {
#pragma warning disable CS4014
                InvokeRemoteNotify(serviceName, messageName, firstP, timeout, serializeType);
#pragma warning restore CS4014
                return null;
            }
            if (returnType == typeof(Task))
            {
#pragma warning disable CS4014
                InvokeRemoteNotify(serviceName, messageName, firstP, timeout, serializeType);
#pragma warning restore CS4014
                return Task.CompletedTask;
            }

            if (returnType.GetTypeInfo().IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {//Task<>
                var realReturnType = returnType.GetGenericArguments()[0];
                if (realReturnType == typeof(RpcResult))
                {
                    result = InvokeRemoteRequestWithNoResultMethod.Invoke(this, new object[] { serviceName, messageName, firstP, timeout, serializeType });
                    return result;
                }
                else
                {
                    var genericMethod = InvokeRemoteRequestMethod.MakeGenericMethod(realReturnType.GetGenericArguments()[0]);
                    result = genericMethod.Invoke(this, new object[] { returnType, serviceName, messageName, firstP, timeout, serializeType });
                    return result;
                }

            }
            else
            {
                throw new Exception("服务定义返回类型错误,请返回 【void、Task、Task<RpcResult>、Task<RpcResult<>>】");
            }

        }

        #region


        private async Task<RpcResult> InvokeRemoteRequestWithNoResult(string serviceName, string methodName, object data, int timeout, byte serializeType)
        {
            var responseMessage = await InvokeRemote(MessageType.Request, serviceName, methodName, data, timeout, serializeType);
            byte[] byteDatas = responseMessage.Data;

            RpcResult result = new RpcResult();
            result.Code = responseMessage.ReturnCode;
            result.Message = responseMessage.ReturnMessage;
            return result;
        }

        private async Task<RpcResult<T>> InvokeRemoteRequest<T>(Type returnType, string serviceName, string methodName, object data, int timeout, byte serializeType)
        {

            var responseMessage = await InvokeRemote(MessageType.Request, serviceName, methodName, data, timeout, serializeType);
            byte[] byteDatas = responseMessage.Data;

            ISerialize serialize = SerializeManage.Instance.GetSerialize(serializeType);
            RpcResult<T> result = new RpcResult<T>();
            result.Code = responseMessage.ReturnCode;
            result.Message = responseMessage.ReturnMessage;
            result.Data = serialize.Deserialize<T>(responseMessage.Data);
            return result;
            /*
            var realReturnType = returnType.GetGenericArguments()[0];
            if (realReturnType == typeof(RpcResult))
            {
                return new RpcResult { Code = responseMessage.ReturnCode, Message = responseMessage.ReturnMessage };
            }
            else
            {
                var realObject = serialize.Deserialize(realReturnType.GetGenericArguments()[0], responseMessage.Data);
                var result = Activator.CreateInstance(realReturnType) as RpcResult;
                result.Code = responseMessage.ReturnCode;
                result.Message = responseMessage.ReturnMessage;

                SetRpcResultData(result, realObject);

                return result;
            }*/


        }


        private async Task InvokeRemoteNotify(string serviceName, string methodName, object data, int timeout, byte serializeType)
        {
            await InvokeRemote(MessageType.Notify, serviceName, methodName, data, timeout, serializeType);
        }

        private async Task<Message> InvokeRemote(MessageType messageType, string serviceName, string methodName, object data, int timeout, byte serializeType)
        {
            var requestMessage = AppLocalContext.Current.InvokeContext?.RequestMessage;
            var traceId = requestMessage?.GetExtData(ExtDataType.TraceId);
            if (string.IsNullOrEmpty(traceId)) traceId = Guid.NewGuid().ToString();

            ISerialize serialize = SerializeManage.Instance.GetSerialize(serializeType);
            Message message = new Message
            {
                SerializeType = serializeType,
                MessageType = messageType,
                RequestId = requestMessage != null ? requestMessage.RequestId : RequestIdGenerator.Instance.GetNewRequestId(),
                ServerName = serviceName,
                MessageName = methodName,
                Data = serialize.Serialize(data),
                ExData = new Dictionary<ushort, string> {
                    { (ushort)ExtDataType.TraceId,traceId},
                }
            };
            return await _invokeRemote.Invoke(message, timeout);
        }


        #endregion
    }
}
