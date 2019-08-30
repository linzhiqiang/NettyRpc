using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.ProductService.DTO
{
    [ProtoContract]
    public class GetProductReq
    {
        [ProtoMember(1)]
        public int ProductId { get; set; }
    }

    [ProtoContract]
    public class GetProductRes
    {
        [ProtoMember(1)]
        public int ProductId { get; set; }

        [ProtoMember(2)]
        public string ProductName { get; set; }

        [ProtoMember(3)]
        public decimal Price { get; set; }
    }
}
