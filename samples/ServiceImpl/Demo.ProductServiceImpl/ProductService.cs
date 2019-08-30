using Demo.ProductService;
using Demo.ProductService.DTO;
using Demo.UserService;
using NettyRpc.Core.Common;
using NettyRpc.Core.DI;
using NettyRpc.Core.Exceptions;
using System;
using System.Threading.Tasks;

namespace Demo.ProductServiceImpl
{
    public class ProductService : IProductService
    {
        private RpcServiceProxyFactory _rpcServiceProxy;
        public ProductService(RpcServiceProxyFactory rpcServiceProxy)
        {
            _rpcServiceProxy = rpcServiceProxy;
        }


        public async Task<RpcResult<GetProductRes>> GetProduct(GetProductReq req)
        {
            var result = new RpcResult<GetProductRes>();

            try
            {
                await Task.CompletedTask;
                //await Task.Delay(1);
                var userService = _rpcServiceProxy.GetService<IUserService>();
                var user = await userService.GetUser(new UserService.DTO.GetUserReq { UserId = req.ProductId });


                result.Data = new GetProductRes
                {
                    ProductId = req.ProductId,
                    ProductName = "商品名称" + req.ProductId,
                    Price = 100.50M
                };
            }
            catch (Exception ex)
            {
                result.Code = ExceptionCode.INTETNAL_ERROR;
                result.Message = "内部错误";
            }

            return result;
        }
    }
}
