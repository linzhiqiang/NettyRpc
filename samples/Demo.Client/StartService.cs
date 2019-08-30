using Demo.OrderService.DTO;
using Demo.Service;
using Demo.UserService;
using Demo.UserService.DTO;
using Microsoft.Extensions.Hosting;
using NettyRpc.Core.ConnectManage;
using NettyRpc.Core.DI;
using NettyRpc.Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Client
{
    public class StartService : IHostedService
    {
        ServerManage _serverManage;
        RpcClientProxyFactory _rpcClientProxyFactory;
        ClientConnectManage _clientConnectManage;

        public StartService(ServerManage serverManage, RpcClientProxyFactory rpcClientProxyFactory
            , ClientConnectManage clientConnectManage)
        {
            _serverManage = serverManage;
            _rpcClientProxyFactory = rpcClientProxyFactory;
            _clientConnectManage = clientConnectManage;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            string ip = "192.168.234.130:8007";
            ip = "127.0.0.1:8007";

            var userServiceHost = new List<string> { "127.0.0.1:8007" };// new List<string> { "127.0.0.1:8007", "127.0.0.1:8008" };
            var orderServiceHost = new List<string> { "127.0.0.1:8007" };// new List<string> { "127.0.0.1:8007", "127.0.0.1:8008", "127.0.0.1:8009", "127.0.0.1:8010" };
            _serverManage.RegisterService("UserService", "*", userServiceHost);
            _serverManage.RegisterService("OrderService", "*", orderServiceHost);
            //await _clientConnectManage.CreateConnect(ip);
            await StartTestAsync(cancellationToken);
        }
        public Task StartTestAsync(CancellationToken cancellationToken)
        {
            GC.Collect();
            Console.WriteLine("TotalMemory:{0}", GC.GetTotalMemory(true) * 1.0 / (1024 * 1024));
            Console.WriteLine("CollectionCount:{0}", GC.CollectionCount(0));
            Task.Run(async () =>
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                int TaskCount = 1;
                Task[] taskArry = new Task[TaskCount];
                for (int i = 0; i < TaskCount; i++)
                {
                    int index = i;
                    taskArry[i] = Task.Run(async () =>
                    {
                        await Test(10000, index);
                    });
                }
                await Task.WhenAll(taskArry);

                stopwatch.Stop();

                {
                    Console.WriteLine("--------------------------------------------------");
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    Console.WriteLine("请求数：{0}", Count);
                    Console.WriteLine("执行时间: {0}ms", stopwatch.ElapsedMilliseconds);
                    Console.WriteLine("每秒执行: {0}次", (int)(Count * 1000.0 / stopwatch.ElapsedMilliseconds));
                    Console.WriteLine("每次执行: {0}毫秒", stopwatch.ElapsedMilliseconds * 1.0 / Count);


                    Console.WriteLine("TotalMemory:{0}", GC.GetTotalMemory(true) * 1.0 / (1024 * 1024));
                    Console.WriteLine("CollectionCount:{0}", GC.CollectionCount(0));
                }
            });

            return Task.CompletedTask;
        }

        //private async Task Test()
        //{
        //    var userService = _rpcClientProxyFactory.GetService<IUserService>();
        //    for (int i = 0; i < 10 * 10000; i++)
        //    {
        //        var userInfo = await userService.GetUser(new Service.DTO.GetUserReq { UserId = i });
        //        Console.WriteLine(JsonUtils.ToJson(userInfo));
        //    }
        //}

        static object SyncLock = new object();
        static int Count = 0;
        private async Task Test(int count, int taskId)
        {
            var userService = _rpcClientProxyFactory.GetService<IUserService>();
            var orderService = _rpcClientProxyFactory.GetService<IOrderService>();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                for (int i = 0; i < count; i++)
                {
                    // var data = await userService.GetUser(new GetUserReq { UserId = i });

                    var submitOrderReq = new SubmitOrderReq
                    {
                        UserId = i,
                        Amount = i + 100,
                        Lines = new List<SubmitOrderLine> {
                                new SubmitOrderLine{  SkuId=i, Quantity=i},
                                new SubmitOrderLine{  SkuId=i+1, Quantity=i+1},
                                new SubmitOrderLine{  SkuId=i+2, Quantity=i+2}
                           }
                    };
                    var data = await orderService.SubmitOrder(submitOrderReq);
                    var currentCount = Interlocked.Increment(ref Count);
                    //if (currentCount % 100 == 0)
                    {
                        Console.WriteLine($"{currentCount}返回数据：{JsonUtils.ToJson(data)}");
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("系统异常：Code=-99999999,Message={0},StackTrace={1}", ex.Message, ex.StackTrace);
            }

            stopwatch.Stop();

            lock (SyncLock)
            {
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                Console.WriteLine("task：{0}", taskId);
                Console.WriteLine("请求数：{0}", count);
                Console.WriteLine("执行时间: {0}ms", stopwatch.ElapsedMilliseconds);
                Console.WriteLine("每秒执行: {0}次", (int)(count * 1000.0 / stopwatch.ElapsedMilliseconds));
                Console.WriteLine("每次执行: {0}毫秒", stopwatch.ElapsedMilliseconds * 1.0 / count);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Console.WriteLine("***************stop*************");
            return Task.CompletedTask;
        }
    }
}
