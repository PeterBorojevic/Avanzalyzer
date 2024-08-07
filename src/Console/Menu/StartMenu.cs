using System;
using Core.Common.Enums;
using Core.Common.Interfaces.Application;

namespace Console.Menu;

public class StartMenu
{
    private readonly IFinancialAnalyzerService _analyzer;
    public StartMenu(IFinancialAnalyzerService analyzer)
    {
        _analyzer = analyzer;
    }

    public void Run()
    {
        System.Console.WriteLine("START MENU \n\nPress a number \n ");
        System.Console.WriteLine(" 1 - Overview \n 2 - Assets \n 3 - Analysis \n 4 - Transactions \n");
        
        var shouldContinue = true;
        while (shouldContinue)
        {
            var key = System.Console.ReadKey();
            System.Console.Clear();
            var clicked = key.Key;

            shouldContinue = clicked switch
            {
                ConsoleKey.None => DoNothing(),
                ConsoleKey.D1 => ShowOverview(),
                ConsoleKey.D2 => ShowAssets(),
                ConsoleKey.D3 => ShowAnalysis(),
                ConsoleKey.D4 => ShowTransactions(),
                _ => DoNothing()
            };

        }
    }

    public bool DoNothing() => true;

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
        System.Console.ReadKey();
        return true;
    }

    public bool ShowTransactions()
    {
        System.Console.WriteLine("TRANSACTIONS \n");

        System.Console.WriteLine("404 - Work in progress \n");
        return false;
    }
}

public class UserAction
{

}