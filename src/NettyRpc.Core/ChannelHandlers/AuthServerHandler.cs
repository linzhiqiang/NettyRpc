using DotNetty.Transport.Channels;
using NettyRpc.Core.ProtoCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.ChannelHandlers
{
    public class AuthServerHandler : ChannelHandlerAdapter
    {
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            Message msg = message as Message;
            if (msg == null) return;
            if (msg.MessageType != MessageType.Auth)
            {
                msg.ReturnCode = -9999;
                msg.ReturnMessage = "请认证";
                context.WriteAndFlushAsync(msg);
            }
            else
            {
                // 开始认证了...

                //认证成功返回
                msg.Data = null;
                msg.ExData = null;
                msg.MessageType = MessageType.AuthRes;
                context.WriteAndFlushAsync(msg);
                context.Handler.HandlerRemoved(context);
            }
        }
    }
}
