using System;
using GPNA.DataFiltration.Application;
using GPNA.DataFiltration.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GPNA.DataFiltration.WebApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            services.AddInfrastructureLayerServices(connectionString);
            services.AddWebApiLayerServices(_configuration);
            services.AddApplicationLayerServices();
        }

        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env,
            ILogger<Startup> logger,
            IFiltrationService filtrationService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GPNA.DataFiltration.WebApi v1"));
            }
            app.UseExceptionHandler("/api/errors");

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            try
            {
                filtrationService.Start();
            }
            catch (Exception e)
            {
                throw new Exception("Не удалось запустить FiltrationService", e);
            }
            
        }
    }
}
