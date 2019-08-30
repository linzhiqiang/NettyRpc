using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.Utils
{
   public class StringUtils
    {
        /// <summary>
        /// 超过最大值的部分截断
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string Truncation(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (value.Length <= maxLength) return value;
            return value.Substring(0, maxLength);
        }
    }
}
