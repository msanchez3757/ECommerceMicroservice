using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.infrastructure.Data;
using ProductApi.infrastructure.Repositories;

namespace ProductApi.infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            // add database connectivity
            // add authentication scheme
            SharedServiceContainer.AddSharedServices<ProductDBContext>(services, config, config["MySerilog:FineName"]!);

            // create dependency injection (DI)
            services.AddScoped<IProduct, ProductRepository>();

            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            // register middleware such as:
            // global exception: handles external errors
            // listen to only api gateway : blocsk all outside calls
            SharedServiceContainer.UseSharedPolicies(app);
            return app;
        }
    }
}
