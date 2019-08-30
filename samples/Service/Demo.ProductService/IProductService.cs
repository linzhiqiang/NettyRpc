using Demo.ProductService.DTO;
using NettyRpc.Core.Common;
using System;
using System.Threading.Tasks;

namespace Demo.ProductService
{
    [RpcService]
    public interface IProductService : IRpcService
    {
        Task<RpcResult<GetProductRes>> GetProduct(GetProductReq req);
    }
}
