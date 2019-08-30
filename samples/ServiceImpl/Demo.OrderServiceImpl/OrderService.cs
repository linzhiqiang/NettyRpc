using Demo.OrderService.DTO;
using Demo.ProductService;
using Demo.Service;
using Demo.UserService;
using NettyRpc.Core.Common;
using NettyRpc.Core.DI;
using NettyRpc.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.OrderServiceImpl
{
    public class OrderService : IOrderService
    {
        private RpcServiceProxyFactory _rpcServiceProxy;
        public OrderService(RpcServiceProxyFactory rpcServiceProxy)
        {
            _rpcServiceProxy = rpcServiceProxy;
        }
        private static int OrderId = 0;
        public async Task<RpcResult<SubmitOrderRes>> SubmitOrder(SubmitOrderReq req)
        {
            var result = new RpcResult<SubmitOrderRes>();

            try
            {
                //var userService = _rpcServiceProxy.GetService<IUserService>();
                //var user = await userService.GetUser(new UserService.DTO.GetUserReq { UserId = req.UserId });

                var productService = _rpcServiceProxy.GetService<IProductService>();
                foreach (var item in req.Lines ?? new List<SubmitOrderLine>())
                {
                    var skuInfo = await productService.GetProduct(new ProductService.DTO.GetProductReq { ProductId = item.SkuId });
                }


                result.Data = new SubmitOrderRes
                {
                    OrderId = OrderId++
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
