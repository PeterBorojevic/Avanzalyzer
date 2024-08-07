using Console.Interfaces;
using Console.Menu;
using Microsoft.Extensions.DependencyInjection;

namespace Console;

public static class MenuInstaller
{
    public static IServiceCollection AddMenues(this IServiceCollection services)
    {
        services.AddSingleton<IStartMenu, StartMenu>();
        services.AddTransient<IAssetMenu, AssetsMenu>();

        return services;
    }
}
