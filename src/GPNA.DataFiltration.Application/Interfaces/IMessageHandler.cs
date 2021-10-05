namespace GPNA.DataFiltration.Application
{
    public interface IMessageHandler
    {
        void Handle(string sourceTopic, string sourceMessage);
    }
}