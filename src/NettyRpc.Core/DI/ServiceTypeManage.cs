using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.DI
{
   public class ServiceTypeManage
    {
        public static ServiceTypeManage Instance = new ServiceTypeManage();

        public ConcurrentDictionary<string, Type> Dict = new ConcurrentDictionary<string, Type>();

        public ServiceTypeManage RegisterType(string name, Type type)
        {
            Dict.TryAdd(name, type);
            return this;
        }

        public Type GetType(string name)
        {
            Type result;
            Dict.TryGetValue(name, out result);
            return result;

        }
    }
}
