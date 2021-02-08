using System;

namespace DP.MessageQueue
{
    public abstract class EventMessage
    {
        public string CorrelationId { get; set; } = Guid.NewGuid().ToString();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public abstract string Subject { get; }

    }
}
