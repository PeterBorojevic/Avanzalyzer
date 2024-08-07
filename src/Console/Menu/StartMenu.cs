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
            var menu = new NumberedMenu("START MENU \n\nPress a number \n ",
                new MenuItem(1, "Overview", OnClick: ShowOverview),
                new MenuItem(2, "Assets", ShowAssets),
                new MenuItem(3, "Analysis", ShowAnalysis),
                new MenuItem(4, "Transactions", ShowTransactions)
            );
            menu.Display();
            shouldContinue = menu.GotoUserSelection();
            
            // TODO add function to continue
        }
    }

    public bool DoNothing() => true;

    public bool PressAnyKeyToReturn()
    {
        System.Console.WriteLine("\n Click any key to return");
        ReadKey();
        return true;
    }

    private static ConsoleKeyInfo ReadKey() => System.Console.ReadKey(true);

    public bool ShowOverview()
    {
        System.Console.WriteLine("OVERVIEW \n");
        _analyzer.Get(AnalysisCalculationType.AccountTotals).PrintToConsole();
        return PressAnyKeyToReturn();
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
        return PressAnyKeyToReturn();
    }

    public bool ShowAnalysis()
    {
        System.Console.WriteLine("ANALYSIS \n");
        _analyzer.GetDividends(GroupingType.ByYear).PrintToConsole();
        _analyzer.Get(AnalysisCalculationType.DepositsAndWithdrawals).PrintToConsole();
        _analyzer.Get(AnalysisCalculationType.DistributionOfSecurities).PrintToConsole();

        return PressAnyKeyToReturn();
    }

    public bool ShowTransactions()
    {
        System.Console.WriteLine("TRANSACTIONS \n");

        System.Console.WriteLine("404 - Work in progress \n");
        return PressAnyKeyToReturn();
    }

}
