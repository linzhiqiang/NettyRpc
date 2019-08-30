using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.ProtoCodes
{
    public class MessageEncoder : MessageToByteEncoder<Message>
    {
        protected override void Encode(IChannelHandlerContext context, Message message, IByteBuffer output)
        {
            output.WriteByte(message.Reserved);
            output.WriteByte(message.SerializeType);
            output.WriteByte(message.Version);
            output.WriteByte((byte)message.MessageType);

            int bodyLength = 0;
            int lengthIndex = 4;
            output.WriteInt(bodyLength);//临时设为0，最后才知道长度，在最后设置了

            if (message.MessageType == MessageType.Request || message.MessageType == MessageType.Notify || message.MessageType == MessageType.Response)
            {
                output.WriteInt(message.RequestId);
                WriterLV(message.ServerName, output);
                WriterLV(message.MessageName, output);
            }

            if (message.MessageType == MessageType.Response)
            {
                output.WriteInt(message.ReturnCode);
                if (message.ReturnCode != 0)
                {
                    WriterLV(message.ReturnMessage, output);
                }
            }

            if (message.ExData != null)
            {
                output.WriteUnsignedShort((ushort)message.ExData.Count);
                foreach (var item in message.ExData)
                {
                    output.WriteUnsignedShort(item.Key);
                    WriterLV(item.Value, output);
                }
            }
            else
            {
                output.WriteUnsignedShort((ushort)0);
            }

            if (message.Data != null && message.Data.Length > 0)
            {
                output.WriteBytes(message.Data);
            }

            output.SetInt(lengthIndex, output.ReadableBytes);
        }

        private int GetLength(Message message)
        {
            int dataLegnth = message.Data != null ? message.Data.Length : 0;
            return dataLegnth;
        }


        /// <summary>
        /// |---value length 2byte--|-------value---------|
        /// </summary>
        /// <param name="value"></param>
        /// <param name="output"></param>
        private void WriterLV(string value, IByteBuffer output)
        {
            if (value == null)
            {
                value = string.Empty;
            }

            byte[] data = GetStringBytes(value);

            if (data != null)
            {
                output.WriteUnsignedShort((ushort)data.Length);
                output.WriteBytes(data);
            }
            else

            {
                output.WriteUnsignedShort((ushort)0);
            }

        }

        private byte[] GetStringBytes(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            return Encoding.UTF8.GetBytes(str);
        }


    }
}
