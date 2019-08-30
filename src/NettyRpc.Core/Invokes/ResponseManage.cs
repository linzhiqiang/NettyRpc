using NettyRpc.Core.ProtoCodes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NettyRpc.Core.Invokes
{
    public static class ResponseManage
    {
        public static ResponseManage<Message> Instance = new ResponseManage<Message>();
    }

    public class ResponseManage<T>
    {
        ConcurrentDictionary<int, TaskResultNode<T>> Hash = new ConcurrentDictionary<int, TaskResultNode<T>>();

        public TaskCompletionSource<T> RegisterRequest(int requestId, int timeout)
        {
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            Hash.TryAdd(requestId, new TaskResultNode<T>(tcs, timeout));
            Timeout(requestId, tcs, timeout);
            return tcs;
        }

        public void SetResult(int requestId, T value)
        {
            TaskResultNode<T> node;
            Hash.TryGetValue(requestId, out node);
            TaskResultNode<T> removeNode;
            this.Hash.TryRemove(requestId, out removeNode);

            if (node != null)
            {
                node.Tcs.TrySetResult(value);
            }

        }

        public void SetException(int requestId, Exception ex)
        {
            TaskResultNode<T> node;

            Hash.TryGetValue(requestId, out node);
            TaskResultNode<T> removeNode;
            this.Hash.TryRemove(requestId, out removeNode);

            if (node != null)
            {
                node.Tcs.TrySetException(ex);
            }
        }

        private void Timeout(int requestId, TaskCompletionSource<T> tsc, int milliseconds)
        {
            Task.Delay(TimeSpan.FromMilliseconds(milliseconds)).ContinueWith((task) =>
            {
                string errorMsg = string.Format("请求超时,requestId:{0}", requestId);
                SetException(requestId, new TimeoutException(errorMsg));
            });

        }
    }

    public class TaskResultNode<T>
    {
        public TaskCompletionSource<T> Tcs;
        private int Timeout;

        public TaskResultNode(TaskCompletionSource<T> tcs, int timeout)
        {
            this.Tcs = tcs;
            this.Timeout = timeout;
        }

    }
}
