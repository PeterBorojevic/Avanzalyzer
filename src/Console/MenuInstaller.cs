using Console.Interfaces;
using Console.Menu;
using Microsoft.Extensions.DependencyInjection;

namespace Console;

public static class MenuInstaller
{
    public static IServiceCollection AddMenues(this IServiceCollection services)
    {
        services.AddTransient<IStartMenu, StartMenu>();
        services.AddTransient<IAssetMenu, AssetMenu>();

        return services;
    }
}
