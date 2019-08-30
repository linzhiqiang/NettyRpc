using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.ProtoCodes
{
    //1个byte，取值如下。
    //0x01: 客户端到服务器的握手请求以及服务器到客户端的握手响应
    //0x02: 客户端到服务器的握手ack
    //0x03: 心跳包
    //0x04: 认证 //服务器主动断开连接通知
    //0x05: 数据包的request(需要返回值的)
    //0x06: 数据包的request(不需要返回值的)
    //0x07: 数据包的request(对应5的返回值)
    //0x08: 数据包的推送(服务端向客户端的推送)

    /// <summary>
    /// 通信消息类型
    /// </summary>
    public enum MessageType
    {
        Handshake = 1,
        HandshakeAck = 2,
        Heartbeat = 3,
        Auth = 4,//认证
        AuthRes = 5,//认证响应

        Request = 11,
        Notify = 12,
        Response = 13
        //Push = 8
    }
}
