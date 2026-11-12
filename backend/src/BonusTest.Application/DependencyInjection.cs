
using Microsoft.Extensions.DependencyInjection;
using BonusTest.Application.Catalogs;
using BonusTest.Application.Bonuses;

namespace BonusTest.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // Servicio de catálogos
        services.AddScoped<CatalogService>();

        // Servicio del módulo de bonos
        services.AddScoped<BonusService>();

        return services;
    }
}
