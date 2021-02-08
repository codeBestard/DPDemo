using System;
using System.Collections.Generic;
using System.Text;
using Prometheus;
using Counter = Prometheus.Client.Counter;
using Metrics = Prometheus.Client.Metrics;

namespace DP.MessageQueue
{
    public interface IQueueMetric
    {
        void Init();
        void IncreaseCounter(string counterName);
    }
    public class QueueMetric
    {
        private readonly Counter _eventCounter;
                public QueueMetric()
        {
            _eventCounter = Metrics.CreateCounter(
                "SaveHandler_Events", "Event Count", "Host", "Status"
            );

                 }
    }
}
