using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NettyRpc.Core.Utils
{
    public static class MethodUtils
    {
        public static async Task<object> InvokeMethodAsync(object obj, MethodInfo methodInfo, object[] parameters)
        {
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            Type returnType = methodInfo.ReturnType;

            if (returnType == typeof(void))
            {
                await Task.Run(() => { MethodInvoke(methodInfo, obj, parameters); });
                return null;
            }
            if (returnType == typeof(Task))
            {
                //await Task.Run(() => { MethodInvoke(methodInfo,obj, parameters); });
                //return await Task.FromResult<object>(null);

                dynamic dynTask = MethodInvoke(methodInfo, obj, parameters);
                await dynTask;
                return null;
            }
            if (returnType.GetTypeInfo().IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {//Task<>
                var task = MethodInvoke(methodInfo, obj, parameters);
                dynamic dynTask = task;
                return await dynTask;
            }

            if (returnType.GetTypeInfo().IsGenericType)
            {
                return await Task.Run(() =>
                {
                    var result = MethodInvoke(methodInfo, obj, parameters);// methodInfo.Invoke(service, inParms);
                    return result;
                });
            }
            else//非泛型
            {
                return await Task.Run(() =>
                {
                    var result = MethodInvoke(methodInfo, obj, parameters);// methodInfo.Invoke(service, inParms);
                    return result;
                });
            }
        }



        public static object MethodInvoke(MethodInfo method, object obj, object[] parameters)
        {
            try
            {
                Type returnType = method.ReturnType;
                if (returnType.IsGenericParameter)
                {
                    throw new Exception("不支持泛型参数");
                }
                else
                {
                    return method.Invoke(obj, parameters);
                }
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null) throw ex.InnerException;
                throw;
            }
            //catch (Exception)
            //{
            //    throw;
            //}
        }
    }
}
