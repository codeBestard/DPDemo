using System;
using DP.MessageQueue.Configurations;
using DP.MessageQueue.Impl;

namespace DP.MessageQueue
{
    public interface IMessageQueueFactory
    {
        IBaseQueueSend BuildOutboundQueue(string @eventType);
        IBaseQueueReceive BuildInboundQueue(string @eventType);
    }

    public class MessageQueueFactory
    {
        public static IMessageQueueFactory GetMessageQueueFactory()
        {
            var queuelib = Config.QueueLibrary.ToUpper();
            
            Console.WriteLine($"Message Queue Factory : {queuelib}");

            if (queuelib == "AZURE")
            {
                return new AzureQueueFactory();
            }

            return new NatsQueueFactory();
        }
    }


    public class AzureQueueFactory : IMessageQueueFactory
    {
                

        public IBaseQueueSend BuildOutboundQueue(string @eventType)
        {
            var _azureConnStr = Config.MessageQueueConnectionString;

            var azureServiceBusQueue = new AzureServiceBusQueue(_azureConnStr);

            switch (@eventType)
            {
                case "pubJob":
                    azureServiceBusQueue.InitializeOutbound(new QueueDescription { QueueName = JobAssignmentEvent.MessageSubject });
                    return azureServiceBusQueue;

                case "pubResponse":
                    azureServiceBusQueue.InitializeOutbound(new QueueDescription { QueueName = JobResponseEvent.MessageSubject });
                    return azureServiceBusQueue;

                default:
                    throw new NotSupportedException(nameof(@eventType));
            }
        }

        public IBaseQueueReceive BuildInboundQueue(string eventType)
        {
            var _azureConnStr = Config.MessageQueueConnectionString;
            var azureServiceBusQueue = new AzureServiceBusQueue(_azureConnStr);

            switch (eventType)
            {
                case "subJob":
                    azureServiceBusQueue.InitializeInbound(new QueueDescription { QueueName = JobAssignmentEvent.MessageSubject });
                    return azureServiceBusQueue;

                case "subResponse":
                    azureServiceBusQueue.InitializeInbound(new QueueDescription { QueueName = JobResponseEvent.MessageSubject });
                    return azureServiceBusQueue;
                default:
                    throw new NotSupportedException(eventType);
            }
        }

        
    }


    public class NatsQueueFactory : IMessageQueueFactory
    {
        
        public  IBaseQueueSend BuildOutboundQueue(string eventType)
        {
            var _natsConnStr = Config.MessageQueueConnectionString;
            var natQueue = new NatsQueue(_natsConnStr);

            switch (eventType)
            {
                case "pubJob":
                    natQueue.InitializeOutbound(new NatsQueueDescription { Subject = JobAssignmentEvent.MessageSubject });
                    return natQueue;

                case "pubResponse":
                    natQueue = new NatsQueue(_natsConnStr);
                    natQueue.InitializeOutbound(new NatsQueueDescription { Subject = JobResponseEvent.MessageSubject });
                    return natQueue;
                default:
                    throw new NotSupportedException(nameof(eventType));
            }
        }

        public IBaseQueueReceive BuildInboundQueue(string eventType)
        {
            var _natsConnStr = Config.MessageQueueConnectionString;
            var natQueue = new NatsQueue(_natsConnStr);

            switch (eventType)
            {
                case "subJob":
                    natQueue.InitializeInbound(new NatsQueueDescription { Subject = JobAssignmentEvent.MessageSubject, QueueName = "job-assignment" });
                    return natQueue;

                case "subResponse":
                    natQueue.InitializeInbound(new NatsQueueDescription { Subject = JobResponseEvent.MessageSubject, QueueName = "job-response" });
                    return natQueue;
                default:
                    throw new NotSupportedException(nameof(eventType));
            }
        }


    }

}

