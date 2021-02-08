

namespace DP.MessageQueue.Impl
{
    public class QueueDescription
    {
       public virtual string QueueName { get; set; }
    }

    public sealed class ServiceBusQueueDescription : QueueDescription
    {
        public string Topic { get; set; }
    }

    public sealed class NatsQueueDescription : QueueDescription
    {
        public string Subject { get; set; }
    }
}
