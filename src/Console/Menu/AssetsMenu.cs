using Console.Interfaces;
using Console.Models;
using Core.Common.Enums;
using Core.Common.Interfaces.Application;
using Core.Extensions;
using Core.Interfaces.Repositories;
using Core.Models;
using Core.Models.Settings;

namespace Console.Menu;

public class AssetsMenu : IAssetMenu
{
    private readonly IFinancialAnalyzerService _analyzer;
    private readonly IAvanzaRepository _repository;
    private readonly IPortfolioAnalyzerService _portfolioAnalyzer;

    public AssetsMenu(IFinancialAnalyzerService analyzer, IAvanzaRepository repository, IPortfolioAnalyzerService portfolioAnalyzer)
    {
        _analyzer = analyzer;
        _repository = repository;
        _portfolioAnalyzer = portfolioAnalyzer;
    }

    public UserAction Next()
    {
        throw new NotImplementedException();
    }

    public bool Run()
    {
        var portfolio = _repository.LoadPortfolioData();
        var shouldContinue = true;
        while (shouldContinue)
        {
            var menu = new NumberedMenu("ASSETS \n\nPress a number \n ",
                new MenuItem(0, "Return", Return),
                new MenuItem(1, "Profit or loss", OnClick: () => ShowProfitOrLoss(portfolio)),
                new MenuItem(2, "Show stocks", () => ShowAsset(AssetType.Stock, portfolio)),
                new MenuItem(3, "Show funds", () => ShowAsset(AssetType.Fund, portfolio)),
                new MenuItem(4, "Show ETFs", () => ShowAsset(AssetType.ExchangeTradedFund, portfolio)),
                new MenuItem(5, "Return of investments", ReturnOfInvestments)
            );
            menu.Display();
            shouldContinue = menu.GotoUserSelection();
        }

        return true;
    }

    private bool Return() => false;

    private bool ShowProfitOrLoss(Portfolio portfolio)
    {
        _analyzer.Get(AnalysisCalculationType.ProfitOrLoss, options => options.UseTransactions = true).PrintToConsole();
        portfolio.Print();
        return PressAnyKeyToReturn();
    }

    private bool ShowAsset(AssetType type, Portfolio portfolio)
    {
        portfolio.Select(type).PrintProfitOrLoss();
        portfolio.Print(type);

        return PressAnyKeyToReturn();
    }

    private bool ReturnOfInvestments()
    {
        _portfolioAnalyzer.LoadTransactions();
        return PressAnyKeyToReturn();
    }


    // ######## Move these to base class ########
    public bool PressAnyKeyToReturn()
    {
        System.Console.WriteLine("\n Click any key to return");
        ReadKey();
        return true;
    }
    private static ConsoleKeyInfo ReadKey() => System.Console.ReadKey(true);
    // ##########################################
}
