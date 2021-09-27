using Microsoft.Extensions.DependencyInjection;

namespace GPNA.DataFiltration.Application
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationLayerServices(this IServiceCollection services)
        {
            services.AddSingleton<IFilterStore, FilterStore>();
            services.AddHostedService<FilterStoreService>();
        }
    }
}
