using Microsoft.Extensions.DependencyInjection;

namespace GPNA.DataFiltration.Application
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationLayerServices(this IServiceCollection services)
        {
            services.AddSingleton<IFilterStore, FilterStore>();
            services.AddSingleton<IMessageHandler, MessageHandler>();
            services.AddSingleton<IFiltersApplicator, FiltersApplicator>();
            services.AddHostedService<FiltrationService>();
        }
    }
}
