using System;
using System.Threading.Tasks;
using DP.MessageQueue.Impl;

namespace DP.MessageQueue
{
    public interface IBaseQueueSend : IDisposable
    {
        void InitializeOutbound(QueueDescription queueDescription);

        Task SendAsync(EventMessage eventMessage);
    }
}