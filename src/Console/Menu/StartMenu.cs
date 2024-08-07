using System;
using Console.Extensions;
using Console.Interfaces;
using Console.Models;
using Core.Common.Enums;
using Core.Common.Interfaces.Application;

namespace Console.Menu;

public class StartMenu : IStartMenu
{
    private readonly IFinancialAnalyzerService _analyzer;
    public StartMenu(IFinancialAnalyzerService analyzer)
    {
        _analyzer = analyzer;
    }
    public UserAction Next()
    {
        throw new NotImplementedException();
    }

    public UserAction Show()
    {
        Run();
        return new UserAction()
        {
            KeyPressed = ReadKey().Key,
            ShouldContinue = false
        };
    }

    public void Run()
    {
       
        
        var shouldContinue = true;
        while (shouldContinue)
        {
            System.Console.Clear();
            System.Console.WriteLine("START MENU \n\nPress a number \n ");

            var menu = new List<MenuItem>()
            {
                new(1, "Overview", ShowOverview),
                new(2, "Assets", ShowAssets),
                new(3, "Analysis", ShowAnalysis),
                new(4, "Transactions", ShowTransactions),
            };
            menu.Print();
            var key = ReadKey();
            menu.Goto(key);
            
            // TODO add function to continue
        }
    }

    public bool DoNothing() => true;

    private static ConsoleKeyInfo ReadKey() => System.Console.ReadKey(true);

    public bool ShowOverview()
    {
        System.Console.WriteLine("OVERVIEW \n");
        _analyzer.Get(AnalysisCalculationType.AccountTotals).PrintToConsole();
        return true;
    }

    public bool ShowAssets()
    {
        System.Console.WriteLine("ASSETS \n");

        _analyzer.Get(AnalysisCalculationType.ProfitOrLoss).PrintToConsole();
        // Print stocks
        //portfolio.Select(AssetType.Etf).PrintProfitOrLoss();
        //portfolio.Assets.PrintProfitOrLoss();
        //portfolio.Print(AssetType.Etf);
        //portfolio.Select(AssetType.Fund).PrintProfitOrLoss();
        //portfolio.Select(AssetType.Stock).PrintProfitOrLoss();
        return true;
    }

    public bool ShowAnalysis()
    {
        System.Console.WriteLine("ANALYSIS \n");
        _analyzer.GetDividends(GroupingType.ByYear).PrintToConsole();
        _analyzer.Get(AnalysisCalculationType.DepositsAndWithdrawals).PrintToConsole();
        _analyzer.Get(AnalysisCalculationType.DistributionOfSecurities).PrintToConsole();
        System.Console.WriteLine("\n Click any key to return");
        ReadKey();
        return true;
    }

    public bool ShowTransactions()
    {
        System.Console.WriteLine("TRANSACTIONS \n");

        System.Console.WriteLine("404 - Work in progress \n");
        return true;
    }

}
