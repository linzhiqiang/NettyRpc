using NettyRpc.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.ProductService
{
    [RpcService]
    public interface IProductCategoryService : IRpcService
    {
    }
}
