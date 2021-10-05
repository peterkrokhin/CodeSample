using GPNA.DataFiltration.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GPNA.DataFiltration.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructureLayerServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<IAppDbContext, AppDbContext>(options => options.UseSqlite(connectionString));
            services.AddScoped<IFilterPoolRepo, FilterPoolRepo>();
            services.AddScoped<IFilterConfigRepo, FilterConfigRepo>();
            services.AddTransient<KafkaConsumerConfig>();
            services.AddTransient<KafkaProducerConfig>();
            services.AddSingleton<IMessageConsumer, KafkaMessageConsumer>();
            services.AddSingleton<IMessageProducer, KafkaMessageProducer>();
        }
    }
}
