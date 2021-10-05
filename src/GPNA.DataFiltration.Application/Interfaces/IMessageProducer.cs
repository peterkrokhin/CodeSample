using System.Threading.Tasks;

namespace GPNA.DataFiltration.Application
{
    public interface IMessageProducer
    {
        Task SendMessage(string topic, string message);
    }
}