using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NettyRpc.Core.Utils
{
    public static class MethodUtils
    {
        public static async Task<object> InvokeMethodAsync(object obj, MethodInfo methodInfo, object[] parameters, Type[] genericMethodParameterType = null)
        {
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            Type returnType = methodInfo.ReturnType;

            if (returnType == typeof(void))
            {
                MethodInvoke(methodInfo, obj, parameters, genericMethodParameterType);
                return null;
            }
            if (returnType == typeof(Task))
            {
                dynamic dynTask = MethodInvoke(methodInfo, obj, parameters, genericMethodParameterType);
                await dynTask;
                return null;
            }
            if (returnType.GetTypeInfo().IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {//Task<>
                var task = MethodInvoke(methodInfo, obj, parameters, genericMethodParameterType);
                dynamic dynTask = task; //用nuget添加Microsoft.CSharp即可
                return await dynTask;
            }

            if (returnType.GetTypeInfo().IsGenericType)
            {
                var result = MethodInvoke(methodInfo, obj, parameters, genericMethodParameterType);// methodInfo.Invoke(service, inParms);
                return result;
            }
            else//非泛型
            {
                var result = MethodInvoke(methodInfo, obj, parameters, genericMethodParameterType);// methodInfo.Invoke(service, inParms);
                return result;
            }
        }



        public static object MethodInvoke(MethodInfo method, object obj, object[] parameters, Type[] genericMethodParameterType)
        {
            try
            {
                //Type returnType = method.ReturnType;
                //if (returnType.IsGenericParameter)
                //{
                //    throw new Exception("不支持泛型参数");
                //}
                if (method.IsGenericMethod)
                {
                    // throw new Exception("不支持泛型参数");
                    var methodNew = method.MakeGenericMethod(genericMethodParameterType); //这里要求泛型参数的具体类型
                    return methodNew.Invoke(obj, parameters);
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
