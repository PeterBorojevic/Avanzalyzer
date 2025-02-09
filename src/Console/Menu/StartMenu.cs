﻿using Console.Interfaces;
using Console.Models;
using Core.Common.Enums;
using Core.Common.Interfaces.Application;

namespace Console.Menu;

public class StartMenu : Base.Menu, IStartMenu
{
    private readonly IFinancialAnalyzerService _analyzer;
    private readonly IAssetMenu _assetMenu;
    private readonly ITransactionsMenu _transactionsMenu;
    public StartMenu(IFinancialAnalyzerService analyzer, IAssetMenu assetMenu, ITransactionsMenu transactionsMenu)
    {
        _analyzer = analyzer;
        _assetMenu = assetMenu;
        _transactionsMenu = transactionsMenu;
    }
    public UserAction Next()
    {
        throw new NotImplementedException();
    }

    public bool Run()
    {
        var shouldContinue = true;
        while (shouldContinue)
        {
            var menu = new NumberedMenu("START MENU",
                new MenuItem(1, "Overview", OnClick: ShowOverview),
                new MenuItem(2, "Assets", ShowAssets),
                new MenuItem(3, "Analysis", ShowAnalysis),
                new MenuItem(4, "Transactions", ShowTransactions)
            );
            menu.Display();
            shouldContinue = menu.GotoUserSelection();
            
            // TODO add function to continue
        }

        return false;
    }

    public bool DoNothing() => true;

    public bool ShowOverview()
    {
        System.Console.WriteLine("OVERVIEW \n");
        _analyzer.Get(AnalysisCalculationType.AccountTotals).PrintToConsole();
        return PressAnyKeyToReturn;
    }

    public bool ShowAssets()
    {
        return _assetMenu.Run();
    }

    public bool ShowAnalysis()
    {
        System.Console.WriteLine("ANALYSIS \n");
        _analyzer.Get(AnalysisCalculationType.Dividends).PrintToConsole();
        _analyzer.Get(AnalysisCalculationType.DepositsAndWithdrawals).PrintToConsole();
        _analyzer.Get(AnalysisCalculationType.DistributionOfSecurities).PrintToConsole();

        return PressAnyKeyToReturn;
    }

    public bool ShowTransactions()
    {
        return _transactionsMenu.Run();
    }

}
