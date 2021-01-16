using DotNetty.Transport.Channels;
using NettyRpc.Core.Config;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NettyRpc.Core.ConnectManage
{
    public class ClientConnectManage
    {
        //public static ClientConnectManage Instance = new ClientConnectManage();
        //private ClientConnectManage() { }

        public ClientOptions _clientOptions = null;

        private SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1);
        public ClientConnectManage(ClientOptions clientOptions)
        {
            _clientOptions = clientOptions;
        }

        private ConcurrentDictionary<string, ChannelWrap> Hash = new ConcurrentDictionary<string, ChannelWrap>();

        private int SyncLock = 1;

        //static SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

        public async Task<IChannel> GetChannelOld(string serverAddress)
        {
            IChannel result = GetChannel0(serverAddress);
            if (result != null)
            {
                return result;
            }
            try
            {
                while (Interlocked.Exchange(ref SyncLock, 0) != 1)
                {
                    //Console.WriteLine("自旋等待");
                    await Task.Delay(60);
                };
                result = GetChannel0(serverAddress);//再次获取,
                if (result != null)
                {
                    return result;
                }
                //Console.WriteLine("初始化连接开始");
                await CreateConnect(serverAddress);//创建连接
                                                   // Console.WriteLine("初始化连接完成");
                result = GetChannel0(serverAddress);//再次获取一次连接
            }
            finally
            {
                Interlocked.Exchange(ref SyncLock, 1);
            }

            return result;

        }

        public async Task<IChannel> GetChannel(string serverAddress)
        {
            IChannel result = null;
           await SemaphoreSlim.WaitAsync();
            try
            {
                result = GetChannel0(serverAddress);
                if (result != null)
                {
                    return result;
                }
                else
                {
                    await CreateConnect(serverAddress);//创建连接
                    result = GetChannel0(serverAddress);//再次获取一次连接
                }
            }
            finally
            {
                SemaphoreSlim.Release();
            }

            return result;

        }

        public async Task CreateConnect(string serverAddress)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var clientBootstrap = GlobalContext.Current.ClientBootstrap;
            int perClientConnectCount = _clientOptions.PerClientConnectCount;
            await CreateConnect(serverAddress, perClientConnectCount);
            //for (int i = 0; i < perClientConnectCount; i++)
            //{
            //   var channel = await clientBootstrap.ConnectAsync(serverAddress);
            //    RegisterChannel(serverAddress, channel);
            //}
            stopwatch.Stop();
            //Console.WriteLine("创建连接时间: {0}ms", stopwatch.ElapsedMilliseconds);
        }

        public async Task CreateConnect(string serverAddress, int connectCount)
        {
            var clientBootstrap = GlobalContext.Current.ClientBootstrap;
            for (int i = 0; i < connectCount; i++)
            {
                if (GetChannelCount(serverAddress) < _clientOptions.PerClientConnectCount)//客户端连接数不能大于配置的数值
                {
                    var channel = await clientBootstrap.ConnectAsync(serverAddress);
                    RegisterChannel(serverAddress, channel);
                }
            }
        }

       

        /// <summary>
        /// 第一次创建连接 或 连接断开后重启成功后把连接加入到容器
        /// </summary>
        /// <param name="serverAddress"></param>
        /// <param name="channel"></param>
        public void RegisterChannel(string serverAddress, IChannel channel)
        {
            ChannelWrap cc;
            lock (Hash)
            {
                Hash.TryGetValue(serverAddress, out cc);
                if (cc == null)
                {
                    cc = new ChannelWrap();
                    Hash.TryAdd(serverAddress, cc);
                }
                cc.AddChannel(channel);
            }
        }

        /// <summary>
        /// 连接断开后，删除该连接
        /// </summary>
        /// <param name="serverAddress"></param>
        /// <param name="channel"></param>
        public void RemoveChannel(string serverAddress, IChannel channel)
        {
            ChannelWrap cc;
            lock (Hash)
            {
                Hash.TryGetValue(serverAddress, out cc);
                if (cc != null)
                {
                    cc.RemoveChannel(channel);
                }
            }
        }

        #region 
        private IChannel GetChannel0(string serverAddress)
        {
            string key = serverAddress;
            IChannel result = null;

            ChannelWrap cc;
            Hash.TryGetValue(key, out cc);
            if (cc != null)
            {
                result = cc.GetNext();
            }
            return result;
        }

        /// <summary>
        /// 获取该地址对应的客户端连接数
        /// </summary>
        /// <param name="serverAddress"></param>
        /// <returns></returns>
        private int GetChannelCount(string serverAddress)
        {
            if (Hash.TryGetValue(serverAddress, out ChannelWrap cc))
            {
                return cc.Count();
            }
            return 0;
        }

        #endregion
    }

    /// <summary>
    /// 一个ip:port的channel管理
    /// </summary>
    public class ChannelWrap
    {
        private int Sequence = 0;

        private List<IChannel> ChannelList = new List<IChannel>();

        private ReaderWriterLockSlim ReaderWriterLock = new ReaderWriterLockSlim();

        public IChannel GetNext()
        {
            try
            {
                ReaderWriterLock.EnterReadLock();
                if (ChannelList.Count == 0)
                {
                    return null;
                }
                int id = Interlocked.Increment(ref this.Sequence);
                return this.ChannelList[Math.Abs(id % this.ChannelList.Count)];
                //判断当前channel是否为open，不是open(循环找到一个open的)
            }
            finally
            {
                ReaderWriterLock.ExitReadLock();
            }

        }

        public void AddChannel(IChannel channel)
        {
            try
            {
                ReaderWriterLock.EnterWriteLock();
                if (!ChannelList.Exists(item => item.Id == channel.Id))
                {
                    ChannelList.Add(channel);
                }
            }
            finally
            {
                ReaderWriterLock.ExitWriteLock();
            }
        }

        public void RemoveChannel(IChannel channel)
        {
            try
            {
                ReaderWriterLock.EnterWriteLock();
                IChannel existsChannel = ChannelList.Find((item => item.Id == channel.Id));
                if (existsChannel != null)
                {
                    this.ChannelList.Remove(existsChannel);
                }
            }
            finally
            {
                ReaderWriterLock.ExitWriteLock();
            }

        }

        public void Clear()
        {
            try
            {
                ReaderWriterLock.EnterWriteLock();
                ChannelList.Clear();
            }
            finally
            {
                ReaderWriterLock.ExitWriteLock();
            }

        }

        public int Count()
        {
            try
            {
                ReaderWriterLock.EnterReadLock();
                return ChannelList.Count;
            }
            finally
            {
                ReaderWriterLock.ExitReadLock();
            }

        }


    }
}
