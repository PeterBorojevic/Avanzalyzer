using Application;
using Core.Common.ConsoleTables;
using Core.Extensions;
using Core.Models;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    private static void TestMultipleLines(Portfolio portfolio)
    {
        var unformattedLines = new List<FormattableString>();
        var total = portfolio.Assets.MarketValue();
        ExtendedConsole.WriteLine($"Total: {total.ToString("##"):green} kr.");

        var totalProfit = portfolio.Assets.ProfitOrLoss();
        ExtendedConsole.WriteLine($"P/L: {totalProfit.ToString("##"):green} kr.");

        var percentageGain = decimal.Divide(totalProfit, total);
        ExtendedConsole.WriteLine($"Yield: {percentageGain.ToString("P"):yellow}.");

    }

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
        TestMultipleLines(portfolio);
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