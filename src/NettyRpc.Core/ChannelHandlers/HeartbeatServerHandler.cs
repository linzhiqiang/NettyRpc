using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using NettyRpc.Core.ProtoCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.ChannelHandlers
{
    public class HeartbeatServerHandler : ChannelHandlerAdapter
    {
        Common.Logging.ILogger Logger = Common.Logging.LoggerFactory.Instance.GetLogger<HeartbeatServerHandler>();
        //IdleStateHandler
        public static Message HeartbeatMessage = new Message { MessageType = MessageType.Heartbeat };

        public HeartbeatServerHandler()
        {

        }
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            Message msg = message as Message;
            if (msg != null && msg.MessageType == MessageType.Heartbeat)
            {

                context.WriteAndFlushAsync(HeartbeatMessage).Wait();
            }
            else
            {
                base.ChannelRead(context, message);
                /*
                基类代码：
                [Skip]
                public virtual void ChannelRead(IChannelHandlerContext context, object message) => context.FireChannelRead(message);
                context(channelHandler的包装)：是从下个handler链开始触发，这里应该用base.ChannelRead(context, message);或者context.FireChannelRead(message);
                如果用channel或者pipeline都是从整个管道执行，而context是从下一个context开始执行

                 */
            }

        }

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            var idelEvent = evt as IdleStateEvent;
            if (idelEvent == null)
            {
                base.UserEventTriggered(context, evt);
                return;
            }

            if (idelEvent.State == IdleState.AllIdle && idelEvent.First ==false)
            {
                Logger.Info($"心跳停止，关闭连接:{context.Channel.RemoteAddress}");
                context.CloseAsync().Wait();
            }

        }


    }
}
