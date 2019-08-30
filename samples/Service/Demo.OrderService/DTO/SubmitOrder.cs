using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.OrderService.DTO
{
    [ProtoContract]
    public class SubmitOrderReq
    {
        [ProtoMember(1)]
        public int UserId { get; set; }

        [ProtoMember(2)]
        public decimal Amount { get; set; }

        [ProtoMember(3)]
        public List<SubmitOrderLine> Lines { get; set; }
    }

    [ProtoContract]
    public class SubmitOrderLine
    {
        [ProtoMember(1)]
        public int SkuId { get; set; }

        [ProtoMember(2)]
        public int Quantity { get; set; }

    }

    [ProtoContract]
    public class SubmitOrderRes
    {
        [ProtoMember(1)]
        public int OrderId { get; set; }
    }

}
