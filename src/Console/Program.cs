using Application;
using Core.Interfaces.Repositories;
using Core.Models.Dtos;
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
            .BuildServiceProvider();

        var portfolioRepository = serviceProvider.GetService<IPortfolioRepository>();

        
        var portfolio = portfolioRepository.LoadPortfolioData();
        var totals = new AccountTotals(portfolio);
        totals.PrintToConsole();

        var transactions = portfolioRepository.LoadTransactions();

        // Print stocks
        //portfolio.Select(AssetType.Etf).PrintProfitOrLoss();
        //portfolio.Assets.PrintProfitOrLoss();
        //portfolio.Print(AssetType.Etf);
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