using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GPNA.DataFiltration.Application
{
    class FilterStoreService : BackgroundService
    {
        private readonly IFilterStore _filterStore;

        public FilterStoreService(IFilterStore filterStore)
        {
            _filterStore = filterStore;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _filterStore.StartCyclicUpdate();
            }
            catch
            {
                throw;
            }
        }
    }
}
