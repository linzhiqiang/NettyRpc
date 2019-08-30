using DotNetty.Transport.Channels;
using NettyRpc.Core.Common;
using NettyRpc.Core.Exceptions;
using NettyRpc.Core.Invokes;
using NettyRpc.Core.ProtoCodes;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NettyRpc.Core.ChannelHandlers
{
    public class DispatchServerHandler : ChannelHandlerAdapter
    {
        Common.Logging.ILogger Logger = Common.Logging.LoggerFactory.Instance.GetLogger<DispatchServerHandler>();
        private InvokeLocal _invokeLocal;

        public DispatchServerHandler(InvokeLocal invokeLocal)
        {
            _invokeLocal = invokeLocal;
        }
        public override void ChannelActive(IChannelHandlerContext context)
        {
            Logger.Debug("连接建立");
            base.ChannelActive(context);
            IPEndPoint remoteIp = context.Channel.RemoteAddress as IPEndPoint;
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            Message msg = message as Message;
            if (msg == null) return;

            Task.Run(async () =>
            {
                await ChannelReadAsync(context, msg);
            });
        }

        public async Task ChannelReadAsync(IChannelHandlerContext context, Message message)
        {
            if (message.MessageType == MessageType.Request || message.MessageType == MessageType.Notify)
            {
               // Logger.Debug(string.Format("收到请求数据：MessageType={0},requestId={1},datalength={2}", message.MessageType.ToString(), message.RequestId, message.Data != null ? message.Data.Length : 0));

                //InvokeContext.Current = new InvokeContext { IsFirstCall = true, RequestMessage = message };
                AppLocalContext.Current.InvokeContext = new InvokeContext { IsFirstCall = true, RequestMessage = message };
                var result = await _invokeLocal.Invoke(message);

                if (message.MessageType == MessageType.Request)
                {
                    result.MessageType = MessageType.Response;
                    await context.WriteAndFlushAsync(result);
                }
            }

            //else if (msg.MessageType == MessageType.Response)
            //{
            //    Logger.Debug(string.Format("收到响应数据：MessageType={0},requestId={1},datalength={2}", msg.MessageType.ToString(), msg.RequestId, msg.Data != null ? msg.Data.Length : 0));
            //    ResponseManage.Instance.SetResult(msg.RequestId, result);
            //}
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        /// <summary>
        /// 断开连接后触发
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            IPEndPoint remoteIp = context.Channel.RemoteAddress as IPEndPoint;
            string ip = remoteIp.Address.MapToIPv4().ToString() + ":" + remoteIp.Port;
            string info = string.Format("连接断开,ip={0}", ip);
            Logger.Trace(info);

            base.ChannelInactive(context);
        }

        /// <summary>
        /// 发生异常
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            //这里应记下日志，关闭连接。业务异常就不要往这里抛。调用业务代码做好try catch
            context.CloseAsync();
            string ip = context.Channel.RemoteAddress.ToString();
            string info = string.Format("连接异常,ip={0},message={1}", ip, exception.Message);
            Logger.Error(info);
        }



    }
}
