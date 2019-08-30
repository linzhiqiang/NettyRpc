
using Demo.UserService;
using Demo.UserService.DTO;
using NettyRpc.Core.Common;
using NettyRpc.Core.DI;
using NettyRpc.Core.Exceptions;
using System;
using System.Threading.Tasks;

namespace Demo.UserServiceImpl
{
    public class UserService : IUserService
    {
        private RpcServiceProxyFactory _rpcServiceProxy;
        public UserService(RpcServiceProxyFactory rpcServiceProxy)
        {
            _rpcServiceProxy = rpcServiceProxy;
        }

        public async Task<RpcResult<GetUserRes>> GetUser(GetUserReq getUserReq)
        {
            var result = new RpcResult<GetUserRes>();

            try
            {
                await Task.CompletedTask;
               // await Task.Delay(1);
                result.Data = new GetUserRes
                {
                    UserId = getUserReq.UserId,
                    UserName = "admin"
                };
            }
            catch (Exception ex)
            {
                result.Code = ExceptionCode.INTETNAL_ERROR;
                result.Message = "内部错误";
            }

            return result;
        }
       public async Task<RpcResult> DeleteUser(GetUserReq getUserReq)
        {
            var result = new RpcResult();

            try
            {
                Console.WriteLine($"DeleteUser:{getUserReq.UserId}");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                result.Code = ExceptionCode.INTETNAL_ERROR;
                result.Message = "内部错误";
            }

            return result;
        }

        public async Task NoticeUser(GetUserReq getUserReq)
        {
            Console.WriteLine($"NoticeUser:{getUserReq.UserId}");
            await Task.CompletedTask;
        }
    }
}
