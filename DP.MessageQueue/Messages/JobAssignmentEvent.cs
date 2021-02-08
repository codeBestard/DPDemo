using System.Collections.Generic;
using System.Linq;
using DP.Common;

namespace DP.MessageQueue
{
    public class JobAssignmentEvent : EventMessage
    {
        public const string MessageSubject = "events.job.assignment";

        public override string Subject => MessageSubject;

        public IEnumerable<JobDetails> JobDetails { get; set; } = Enumerable.Empty<JobDetails>();
    }
}