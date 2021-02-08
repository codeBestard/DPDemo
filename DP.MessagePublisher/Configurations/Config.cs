using DP.Common.Configurations;

namespace DP.MessagePublisher.Configurations
{
    public class Config : BaseConfig
    {
        public static string Batches => Get("batches");

        public static string BatchSize => Get("batchSize");
    }
}
