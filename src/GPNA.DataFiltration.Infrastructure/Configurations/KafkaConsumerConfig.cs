using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace GPNA.DataFiltration.Infrastructure
{
    public class KafkaConsumerConfig : ConsumerConfig
    {
        public KafkaConsumerConfig(IConfiguration configuration)
        {
            BootstrapServers = configuration["KafkaProducer:BootstrapServers"];
            GroupId = configuration["KafkaProducer:GroupId"];
            AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
        }
    }
}
