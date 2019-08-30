using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using NettyRpc.Core.ProtoCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.ChannelHandlers
{
    public class HeartbeatClientHandler : ChannelHandlerAdapter
    {

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            //接收心跳包
            Message msg = message as Message;
            if (msg != null && msg.MessageType == MessageType.Heartbeat)
            {
                //Console.WriteLine("心跳正常{0}", DateTime.Now.ToString());
            }
            else
            {
                base.ChannelRead(context, message);
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
            if (idelEvent.State == IdleState.AllIdle)
            {
                //Console.WriteLine("发送心跳包{0}",DateTime.Now.ToString());
                context.WriteAndFlushAsync(HeartbeatServerHandler.HeartbeatMessage);//如果连接不在，这里会异常
            }
        }
    }
}
