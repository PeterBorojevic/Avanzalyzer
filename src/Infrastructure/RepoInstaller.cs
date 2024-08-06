using Core.Common.Interfaces;
using Core.Interfaces.Repositories;
using Infrastructure.Parsers;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class RepoInstaller
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IAccountParser, AccountParser>();
        services.AddTransient<IPositionParser, PositionParser>();
        services.AddTransient<ITransactionParser, TransactionParser>();
        // Repositories
        services.AddSingleton<IPortfolioRepository, PortfolioRepository>();

        return services;
    }
}
