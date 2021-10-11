using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace GPNA.DataFiltration.Infrastructure
{
    public class KafkaConsumerConfig : ConsumerConfig
    {
        public KafkaConsumerConfig(IConfiguration configuration)
        {
            BootstrapServers = configuration["KafkaConsumer:BootstrapServers"];
            GroupId = configuration["KafkaConsumer:GroupId"];
            AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
        }
    }
}
