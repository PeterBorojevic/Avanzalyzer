using Core.Common.Interfaces;
using Infrastructure.Parsers;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class RepoInstaller
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IAccountParser, AccountParser>();
        services.AddTransient<IPositionParser, PositionParser>();

        return services;
    }
}
