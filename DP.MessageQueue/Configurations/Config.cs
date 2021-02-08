using DP.Common.Configurations;

namespace DP.MessageQueue.Configurations
{
    public class Config : BaseConfig
    {
        public static string QueueLibrary => Get("queuelib");

        public static string MessageQueueConnectionString => Get("mqConnectionString");
    }
}
