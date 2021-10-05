using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using GPNA.DataFiltration.Application;

namespace GPNA.DataFiltration.Infrastructure
{
    class KafkaMessageProducer : IMessageProducer, IDisposable
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaMessageProducer(KafkaProducerConfig config)
        {
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task SendMessage(string topic, string message)
        {
            var kafkaMessage = new Message<Null, string> { Value = message };
            await _producer.ProduceAsync(topic, kafkaMessage);
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}
