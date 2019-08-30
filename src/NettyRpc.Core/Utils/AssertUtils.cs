using NettyRpc.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.Utils
{
    internal class AssertUtils
    {
        public static void IsTrue(bool condition, int errorCode, String errorText)
        {
            if (!condition)
            {
                ThrowException(errorCode, errorText);
            }
        }

        public static void IsFalse(bool condition, int errorCode, String errorText)
        {
            IsTrue(!condition, errorCode, errorText);
        }

        public static void IsNull(Object obj, int errorCode, String errorText)
        {
            IsTrue(obj == null, errorCode, errorText);
        }

        public static void IsNotNull(Object obj, int errorCode, String errorText)
        {
            IsTrue(obj != null, errorCode, errorText);
        }

        public static void IsEmpty(Object obj, int errorCode, String errorText)
        {
            IsTrue(obj == null || obj.ToString() == string.Empty, errorCode, errorText);
        }

        public static void IsNotEmpty(Object obj, int errorCode, String errorText)
        {
            IsTrue(obj != null && obj.ToString() != string.Empty, errorCode, errorText);
        }

        public static void ThrowException(int errorCode, string errorText)
        {
            throw new RpcException(errorCode, errorText);
        }

    }
}
