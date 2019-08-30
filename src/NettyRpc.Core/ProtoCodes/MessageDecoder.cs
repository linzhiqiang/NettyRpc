using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.ProtoCodes
{
    public class MessageDecoder : MessageToMessageDecoder<IByteBuffer>
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
        {
            Message msg = new Message();
            msg.Reserved = message.ReadByte();
            msg.SerializeType = message.ReadByte();
            msg.Version = message.ReadByte();
            msg.MessageType = (MessageType)message.ReadByte();
            int length = message.ReadInt();


            if (msg.MessageType == MessageType.Request || msg.MessageType == MessageType.Notify || msg.MessageType == MessageType.Response)
            {
                msg.RequestId = message.ReadInt();
                msg.ServerName = GetLV(message);
                msg.MessageName = GetLV(message);
            }

            if (msg.MessageType == MessageType.Response)
            {
                msg.ReturnCode = message.ReadInt();
                if (msg.ReturnCode != 0)
                {
                    msg.ReturnMessage = GetLV(message);
                }

            }

            //解析扩展字段
            var extendDataCount = message.ReadUnsignedShort();
            if (extendDataCount > 0)
            {
                Dictionary<ushort, string> exData = new Dictionary<ushort, string>();
                for (int i = 0; i < extendDataCount; i++)
                {
                    var key = message.ReadUnsignedShort();
                    var value = GetLV(message);

                    exData.Add(key, value);
                    //Console.WriteLine("key={0},value= {1}",key,value);
                }

                msg.ExData = exData;
            }


            if (message.ReadableBytes > 0)
            {
                byte[] data = new byte[message.ReadableBytes];
                message.ReadBytes(data);
                msg.Data = data;
            }

            output.Add(msg);
        }

        private string ByteToString(byte[] data)
        {
            if (data == null)
            {
                return null;
            }
            return Encoding.UTF8.GetString(data);
        }

        private string GetLV(IByteBuffer message)
        {
            ushort valueLength = message.ReadUnsignedShort();
            byte[] valueBytes = new byte[valueLength];
            message.ReadBytes(valueBytes);

            return Encoding.UTF8.GetString(valueBytes);

        }
    }
}
