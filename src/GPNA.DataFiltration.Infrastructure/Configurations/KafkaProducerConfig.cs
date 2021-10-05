using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace GPNA.DataFiltration.Infrastructure
{
    public class KafkaProducerConfig : ProducerConfig
    {
        public string BootstrapServers { get; set; }
        public string GroupId { get; set; }
        
        public KafkaProducerConfig(IConfiguration configuration)
        {
            BootstrapServers = configuration["KafkaProducer:BootstrapServers"];
            GroupId = configuration["KafkaProducer:GroupId"];
        }
    }
}
