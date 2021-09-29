using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace GPNA.DataFiltration.Application
{
    class FilterStoreService : BackgroundService
    {
        private const int NEXT_TRY_CACHE_UPDATE_MILLISECONDS = 10_000;
        private readonly IFilterStore _filterStore;

        public FilterStoreService(IFilterStore filterStore)
        {
            _filterStore = filterStore;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                _filterStore.CacheUpdate();
            }
            catch
            {
                // Логгировать

                // Повторять
                //await Task.Delay(NEXT_TRY_CACHE_UPDATE_MILLISECONDS, cancellationToken);
                //await ExecuteAsync(cancellationToken);

                // Временно
                throw;
            }
        }
    }
}
