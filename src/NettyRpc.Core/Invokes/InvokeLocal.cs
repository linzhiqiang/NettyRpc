using NettyRpc.Core.Common;
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
   public class InvokeLocal
    {
        Common.Logging.ILogger _logger = Common.Logging.LoggerFactory.Instance.GetLogger<InvokeLocal>();
        private RpcServiceProxyFactory _rpcServiceProxyFactory;
        public InvokeLocal(RpcServiceProxyFactory rpcServiceProxyFactory)
        {
            _rpcServiceProxyFactory = rpcServiceProxyFactory;
        }
        public async Task<Message> Invoke(Message message)
        {
            var result = message.CopyToResponse();
            //找到本地方法 调用
            byte serializeType = message.SerializeType;
            string serverName = message.ServerName;
            string methodName = message.MessageName;
            try
            {
                ISerialize serialize = SerializeManage.Instance.GetSerialize(serializeType);//获取序列化对象
                Type serviceType = ServiceTypeManage.Instance.GetType(serverName);
                AssertUtils.IsNotNull(serviceType, ExceptionCode.NOTFOUND_ERROR, "服务不存在");

                 object service = _rpcServiceProxyFactory.GetService(serviceType);// _serviceProvider.GetService(serviceType);
                //object service = _serviceProvider.GetService(serviceType);
                MethodInfo methodInfo = service.GetType().GetMethod(methodName);
                ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                object[] inParms = null;
                if (parameterInfos.Length > 0)
                {
                    object requestObj = serialize.Deserialize(parameterInfos[0].ParameterType, message.Data);
                    inParms = new object[] { requestObj };
                }
                var callResult = await MethodUtils.InvokeMethodAsync(service, methodInfo, inParms) as RpcResult;
                AssertUtils.IsNotNull(callResult, ExceptionCode.INTETNAL_ERROR, "返回空");

                result.ReturnCode = callResult != null ? callResult.Code : 0;
                result.ReturnMessage = callResult != null ? callResult.Message : "";

                result.Data = serialize.Serialize(GetRpcResultData(callResult));


            }
            catch (Exception ex)
            {
                result.ReturnCode = ExceptionCode.INTETNAL_ERROR;
                result.ReturnMessage = "内部错误";
                _logger.Error($"{serverName}.{methodName}, {ex.Message},{ex.StackTrace}");
            }
            return result;//serialize.Serialize(result);
        }

        private object GetRpcResultData(RpcResult rpcResult)
        {
            if (rpcResult == null) return null;

            var type = rpcResult.GetType();
            if (!type.IsGenericType) return null;


            return type.GetProperty("Data").GetValue(rpcResult);
        }
    }
}
