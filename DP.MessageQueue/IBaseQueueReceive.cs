using System;
using System.Threading;
using System.Threading.Tasks;
using DP.MessageQueue.Impl;

namespace DP.MessageQueue
{
    public interface IBaseQueueReceive : IDisposable
    {
        void InitializeInbound(QueueDescription queueDescription);

        Task ReceiveAsync<TMessage>(Action<TMessage> onMessageReceived, CancellationToken cancellationToken)
            where TMessage : EventMessage;

    }
}