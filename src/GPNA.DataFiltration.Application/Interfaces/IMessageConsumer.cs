using System.Threading;

namespace GPNA.DataFiltration.Application
{
    public interface IMessageConsumer
    {
        void SubscribeOnTopic(string topic, IMessageHandler messageHandler, CancellationToken cancelationToken);
    }
}
