using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.ProtoCodes
{
    /* 应用协议
 0........8........16........24........32
1  |--预留--|--协议---|---版本--|--type---| //预留,版本号,消息类型
2  |--------length------------------------| //包长(总长度)
3  |--------RequestId---------------------| //请求号

4  |ServerName长度----|-- ServerName body......| //服务名称 ServerName长度=2byte  ServerName body = n byte
5  |MessageName长度---|--MessageName body......| //服务名称 MessageName长度=2byte  MessageName body = n byte

6  |---------return Code------------------| //  响应时存在该字段，0 成功，其他为失败
7  |returnmessage长度-|returnmessage body......| //  响应时且return Code！=0时存在，为失败原因


8  |---扩展字段个数-|   2byte
 |------type-----|---- value length-----|------value body ------|  type=2byte  valu length=4byte   
 type=1   traceid, 
 type=2   spanid
 ...
//以下是body
9  |---------data..............................|//请求数据或响应数据
*/

    /// <summary>
    /// 通信消息
    /// </summary>
    public class Message
    {
        public Message()
        {
            this.Version = 0;
            this.SerializeType = 0;
        }
        /// <summary>
        /// 预留字段
        /// </summary>
        public byte Reserved { get; set; }

        /// <summary>
        ///  0：google propo ,1:json
        /// </summary>
        public byte SerializeType { get; set; }
        public byte Version { get; set; }

        public MessageType MessageType { get; set; }

        public int RequestId { get; set; }

        public string ServerName { get; set; }

        public string MessageName { get; set; }

        #region 响应字段
        public int ReturnCode { get; set; }

        public string ReturnMessage { get; set; }

        #endregion

        public IDictionary<ushort, string> ExData { get; set; }

        public byte[] Data { get; set; }


        public string GetExtData(ExtDataType extDataType)
        {
            string result = null;

            if (ExData != null && ExData.ContainsKey((ushort)extDataType))
            {
                result = ExData[(ushort)extDataType];
            }

            return result;
        }

        public Message CopyToResponse()
        {
            Message result = new Message
            {
                Reserved = this.Reserved,
                SerializeType = this.SerializeType,
                Version = this.Version,
                MessageType = this.MessageType,
                RequestId = this.RequestId,
                ServerName = this.ServerName,
                MessageName = this.MessageName,

                //ReturnCode = this.ReturnCode,
                //ReturnMessage = this.ReturnMessage,
                //ExData = this.ExData,
                //Data = this.Data

            };

            return result;
        }

    }

    /// <summary>
    /// 扩展字段，扩展框架使用
    /// </summary>
    public enum ExtDataType : sbyte
    {
        TraceId = 1,

        SpanId = 2
    }
}
