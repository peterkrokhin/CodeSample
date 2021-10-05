using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace GPNA.DataFiltration.Infrastructure
{
    public class KafkaProducerConfig : ProducerConfig
    {   
        public KafkaProducerConfig(IConfiguration configuration)
        {
            BootstrapServers = configuration["KafkaProducer:BootstrapServers"];
        }
    }
}
