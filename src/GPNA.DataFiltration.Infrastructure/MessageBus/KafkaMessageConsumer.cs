using System;
using System.Threading;
using Confluent.Kafka;
using GPNA.DataFiltration.Application;

namespace GPNA.DataFiltration.Infrastructure
{
    class KafkaMessageConsumer : IMessageConsumer, IDisposable
    {
        private readonly IConsumer<Ignore, string> _consumer;

        public KafkaMessageConsumer(KafkaConsumerConfig config)
        {
            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        }

        public void SubscribeOnTopic(string topic, IMessageHandler messageHandler, CancellationToken cancellationToken)
        {
            _consumer.Subscribe(topic);
            while (!cancellationToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(cancellationToken);
                messageHandler.Handle(consumeResult.Topic, consumeResult.Message.Value);  
            }
            _consumer.Close();
        }

        public void Dispose()
        {
            _consumer.Dispose();
        }
    }
}
