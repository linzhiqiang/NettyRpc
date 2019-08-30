using DotNetty.Transport.Channels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NettyRpc.Core.ConnectManage
{
    /// <summary>
    /// 服务管理
    /// service_method->[ip1,ip2,...]或者 service_*->[ip1,ip2,...]
    /// </summary>
    public class ServerManage
    {
        ConcurrentDictionary<string, ServerAddressWrap> Hash = new ConcurrentDictionary<string, ServerAddressWrap>();
        private object SyncObj = new object();

        private ClientConnectManage _clientConnectManage;
        public  ServerManage(ClientConnectManage clientConnectManage)
        {
            _clientConnectManage = clientConnectManage;
        }



        public void RegisterService(string serverName, string messageName, List<string> serverAdderssList)
        {
            foreach (var item in serverAdderssList)
            {
                RegisterService(serverName,messageName,item);
            }
        }
        public void RegisterService(string serverName, string messageName, string serverAdderss)
        {
            string key = string.Format("{0}_{1}", serverName, messageName);

            lock (SyncObj)
            {
                if (Hash.ContainsKey(key))
                {
                    Hash[key].AddServerAddress(serverAdderss);
                }
                else
                {
                    var container = new ServerAddressWrap();
                    container.AddServerAddress(serverAdderss);
                    Hash.TryAdd(key, container);

                    //请把返回值改为async ClientConnectManage.Instance.CreateConnect(serverAdderss);
                    //不要调用wait，请把返回值改为async Task,然后使用await
                }
            }
        }

        public async Task<IChannel> GetChannel(string serverName, string messageName)
        {
            ServerAddressWrap serverAddressWrap = GetServerAddressWrap(serverName, messageName);
            string serverAdderss = serverAddressWrap != null ? serverAddressWrap.GetNext() : null;
            if (serverAdderss == null)
            {
                throw new Exception($"{serverName}.{messageName}服务不存在，请检查配置");
            }
            IChannel channel = await _clientConnectManage.GetChannel(serverAdderss);
            
            return channel;
        }

        public bool IsLocalService(string serverName, string messageName)
        {
            ServerAddressWrap serverAddressWrap = GetServerAddressWrap(serverName, messageName);
            return serverAddressWrap == null;
        }

        private ServerAddressWrap GetServerAddressWrap(string serverName, string messageName)
        {
            string key = string.Format("{0}_{1}", serverName, messageName);
            ServerAddressWrap serverAddressWrap;
            if (Hash.TryGetValue(key, out serverAddressWrap))
            {
                return serverAddressWrap;
            }
            key = string.Format("{0}_*", serverName);
            if (Hash.TryGetValue(key, out serverAddressWrap))
            {
                return serverAddressWrap;
            }

            return null;
        }
    }

    public class ServerAddressWrap
    {
        private int Sequence = 0;

        private List<string> ChannelList = new List<string>();

        private ReaderWriterLockSlim ReaderWriterLock = new ReaderWriterLockSlim();

        public string GetNext()
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
            }
            finally
            {
                ReaderWriterLock.ExitReadLock();
            }

        }

        public void AddServerAddress(string serverAddress)
        {
            try
            {
                ReaderWriterLock.EnterWriteLock();
                if (!ChannelList.Exists(item => item == serverAddress))
                {
                    ChannelList.Add(serverAddress);
                }
            }
            finally
            {
                ReaderWriterLock.ExitWriteLock();
            }
        }

        public void RemoveServerAddress(string serverAddress)
        {
            try
            {
                ReaderWriterLock.EnterWriteLock();
                this.ChannelList.Remove(serverAddress);
            }
            finally
            {
                ReaderWriterLock.ExitWriteLock();
            }

        }
    }
}
