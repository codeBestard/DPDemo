using System;
using DP.Common;

namespace DP.MessageQueue
{
    public class JobResponseEvent : EventMessage
    {
        public const string MessageSubject = "events.job.response";

        public override string Subject => MessageSubject;

        public JobStatus JobStatus { get; set; }

        public Guid WorkerId { get; set; }

    }

   
}