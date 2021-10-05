using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GPNA.DataFiltration.Application
{
    class FilterStoreService : BackgroundService
    {
        private const int NEXT_TRY_CACHE_UPDATE_MILLISECONDS = 10_000;
        private readonly IFilterStore _filterStore;
        private readonly ILogger<FilterStoreService> _logger;

        public FilterStoreService(IFilterStore filterStore, ILogger<FilterStoreService> logger)
        {
            _filterStore = filterStore;
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
        }
    }
}
