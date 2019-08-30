using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.Exceptions
{
  public  class ExceptionCode
    {
        /// <summary>
        /// 一般错误业务异常
        /// </summary>
        public const int BIZ_ERROR = -10240000;


        /// <summary>
        /// 内部错误
        /// </summary>
        public const int INTETNAL_ERROR = -10240500;

        public const int NOTFOUND_ERROR = -10240400;
    }
}
