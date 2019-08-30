using Demo.UserService.DTO;
using NettyRpc.Core.Common;
using System.Threading.Tasks;

namespace Demo.UserService
{
    [RpcService]
    public interface IUserService: IRpcService
    {
        Task<RpcResult<GetUserRes>> GetUser(GetUserReq getUserReq);

        Task<RpcResult> DeleteUser(GetUserReq getUserReq);

        Task NoticeUser(GetUserReq getUserReq);
    }
}
