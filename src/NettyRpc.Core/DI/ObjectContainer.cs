using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace NettyRpc.Core.DI
{
  public  class ObjectContainer
    {
        public static ObjectContainer Instance = new ObjectContainer();

        private IServiceProvider _serviceProvider;

        private ObjectContainer()
        {

        }

        public void  Init(IServiceProvider  serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

       public  object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }

        public T GetService<T>()
        {
            return _serviceProvider.GetService<T>();
        }
    }
}
