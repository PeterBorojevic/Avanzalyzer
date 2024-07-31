using Application.Features.Parsers;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceInstaller
{
    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        services.AddTransient<IAccountParser, AccountParser>();
        services.AddTransient<IPositionParser, PositionParser>();

        return services;
    }
}