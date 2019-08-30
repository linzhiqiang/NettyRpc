using Demo.OrderService.DTO;
using NettyRpc.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Service
{
    [RpcService]
    public interface IOrderService : IRpcService
    {
        Task<RpcResult<SubmitOrderRes>> SubmitOrder(SubmitOrderReq req);
    }
}
