using DotNetty.Transport.Channels;
using NettyRpc.Core.Config;
using NettyRpc.Core.ConnectManage;
using NettyRpc.Core.Invokes;
using NettyRpc.Core.ProtoCodes;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NettyRpc.Core.ChannelHandlers
{
    public class DispatchClientHandler : ChannelHandlerAdapter
    {
        Common.Logging.ILogger Logger = Common.Logging.LoggerFactory.Instance.GetLogger<DispatchClientHandler>();

        ClientConnectManage _clientConnectManage;
        ClientOptions _clientOptions;

        public DispatchClientHandler(ClientConnectManage clientConnectManage, ClientOptions clientOptions)
        {
            _clientConnectManage = clientConnectManage;
            _clientOptions = clientOptions;
        }
        public override void ChannelActive(IChannelHandlerContext context)
        {
            Logger.Debug("连接建立");
            base.ChannelActive(context);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            Message msg = message as Message;
            if (msg == null) return;
            if (msg.MessageType == MessageType.Response)
            {
               // Logger.Debug(string.Format("收到响应数据：MessageType={0},returnCode={1},requestId={2},datalength={3}", msg.MessageType.ToString(), msg.ReturnCode, msg.RequestId, msg.Data != null ? msg.Data.Length : 0));
                ResponseManage.Instance.SetResult(msg.RequestId, msg);
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            IPEndPoint remoteIp = context.Channel.RemoteAddress as IPEndPoint;
            string ip = remoteIp.Address.MapToIPv4().ToString() + ":" + remoteIp.Port;
            string info = string.Format("连接断开,ip={0}", ip);
            Logger.Trace(info);

            _clientConnectManage.RemoveChannel(ip, context.Channel);

            //定时重启
            AutoReConnect(ip);

            base.ChannelInactive(context);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            string ip = context.Channel.RemoteAddress.ToString();
            string info = string.Format("连接异常,ip={0},message={1}", ip, exception.Message);
            Logger.Error(info);

            //异常就关闭连接
            context.CloseAsync();

        }


        private void AutoReConnect(string ip)
        {

            int interval = _clientOptions.AutoReConnectInterval;
            if (interval <= 0) return;

            Task.Run(async () =>
            {
                int count = 0;
                while (count <= _clientOptions.AutoReConnectMaxNumber)
                {
                    count++;
                    await Task.Delay(TimeSpan.FromSeconds(interval));
                    //interval = interval + 5;

                    bool isSucces = await ReConnect(ip);
                    if (isSucces)
                    {
                        break;
                    }
                }
            });

        }

        private async Task<bool> ReConnect(string ip)
        {
            try
            {
                await Task.CompletedTask;
                await _clientConnectManage.CreateConnect(ip, 1);
                Logger.Info("重连成功");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"重连失败：{ip},{ex.Message}");
            }
            return false;

        }
    }
}
