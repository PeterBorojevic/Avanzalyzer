using Application;
using Core.Common.ConsoleTables;
using Core.Common.ConsoleTables.v1;
using Core.Common.Enums;
using Core.Extensions;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static void Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddRepositories()
            .AddFeatures()
            .AddSingleton<PortfolioManager>()
            .BuildServiceProvider();

        var portfolioManager = serviceProvider.GetService<PortfolioManager>();

        
        var portfolio = portfolioManager.LoadPortfolioData();
        // Print stocks
        portfolio.Select(AssetType.Etf).PrintProfitOrLoss();
        portfolio.Print(AssetType.Etf);
        //portfolio.Select(AssetType.Fund).PrintProfitOrLoss();
        //portfolio.Select(AssetType.Stock).PrintProfitOrLoss();

    }
}


/*  TODO
 *  Menu service to access different features
 *
 * Add features
 * Calculations
 * Totals
 * Forecasted savings
 * Fetch ticker stock prices
 *
 * Backend
 * SQL server for storage and memory
 * Caching
 *
 */