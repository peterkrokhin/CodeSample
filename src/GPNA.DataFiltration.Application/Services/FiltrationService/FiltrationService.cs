using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GPNA.DataFiltration.Application
{
    public class FiltrationService : BackgroundService
    {
        private const int NEXT_TRY_CACHE_UPDATE_MILLISECONDS = 10_000;
        private readonly IFilterStore _filterStore;
        private readonly IMessageConsumer _messageConsumer;
        private readonly IMessageHandler _messageHandler;
        private readonly ILogger<FiltrationService> _logger;

        public FiltrationService(
            IFilterStore filterStore,
            IMessageConsumer messageConsumer,
            IMessageHandler messageHandler,
            ILogger<FiltrationService> logger)
        {
            _filterStore = filterStore;
            _messageConsumer = messageConsumer;
            _messageHandler = messageHandler;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                _filterStore.CacheUpdate();
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, e.Message);

                // Повторять
                //await Task.Delay(NEXT_TRY_CACHE_UPDATE_MILLISECONDS, cancellationToken);
                //await ExecuteAsync(cancellationToken);

                // Временно
                throw;
            }

            var topics = _filterStore.GetSourceTopics();
            foreach (var topic in topics)
            {
                await Task.Run(() => _messageConsumer.SubscribeOnTopic(topic, _messageHandler, cancellationToken), cancellationToken);
            }
        }
    }
}
