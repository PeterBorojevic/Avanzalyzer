using Application.Features;
using Application.Features.Parsers;
using Application.Interfaces;
using Core.Common.ConsoleTables;
using Core.Common.ConsoleTables.v1;
using Core.Common.Enums;
using Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static void TestDictionaryTable()
    {
        Dictionary<string, Dictionary<string, object>> data = new Dictionary<string, Dictionary<string, object>>()
        {
            {"1", new Dictionary<string, object>()
            {
                { "M", true },
                { "B", false },
                { "C", true },
            }},
            {"2", new Dictionary<string, object>()
            {
                { "A", false },
                { "B", true },
                { "C", false },
            }},
            {"3", new Dictionary<string, object>()
            {
                { "A", false },
                { "B", false },
                { "C", true },
            }}
        };
        var table = ConsoleTable.FromDictionary(data);

        Console.WriteLine(table.ToString());
    }
    static void Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IAccountParser, AccountParser>()
            .AddSingleton<IPositionParser, PositionParser>()
            .AddSingleton<PortfolioManager>()
            .BuildServiceProvider();

        var portfolioManager = serviceProvider.GetService<PortfolioManager>();

        var portfolio = portfolioManager.LoadPortfolioData();
        // Print stocks
        portfolio.Get(AssetType.Etf).PrintProfitOrLoss();
        portfolio.Get(AssetType.Fund).PrintProfitOrLoss();
        portfolio.Get(AssetType.Stock).PrintProfitOrLoss();

    }
}
