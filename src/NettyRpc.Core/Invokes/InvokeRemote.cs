using DotNetty.Transport.Channels;
using NettyRpc.Core.ConnectManage;
using NettyRpc.Core.ProtoCodes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NettyRpc.Core.Invokes
{
    public class InvokeRemote
    {
        Common.Logging.ILogger Logger = Common.Logging.LoggerFactory.Instance.GetLogger<InvokeRemote>();

        private ServerManage _serverManage;
        public InvokeRemote(ServerManage serverManage)
        {
            _serverManage = serverManage;
        }
        public async Task<Message> Invoke(Message message, int timeout)
        {
            if (message.MessageType == MessageType.Request)
            {
                //int requestId = RequestIdGenerator.Instance.GetNewRequestId();
                //message.RequestId = requestId;
                var tcs = ResponseManage.Instance.RegisterRequest(message.RequestId, timeout);

                await Write(message);

                Message messageRes = null;
                try
                {
                    messageRes = await tcs.Task;
                }
                catch (TimeoutException ex)
                {
                    //记录log
                    Logger.Error($"{message.ServerName}.{message.MessageName}请求超时，{ex.Message}");
                    throw ex;
                }
                return messageRes;
            }
            else
            {
                await Write(message);
                return await Task.FromResult<Message>(null);
            }


        }

        private async Task Write(Message message)
        {
            IChannel channel = await _serverManage.GetChannel(message.ServerName, message.MessageName);
            await channel.WriteAndFlushAsync(message);
        }
    }
}
