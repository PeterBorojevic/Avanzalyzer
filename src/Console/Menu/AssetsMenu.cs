using Console.Interfaces;
using Console.Models;
using Core.Common.Enums;
using Core.Common.Interfaces.Application;
using Core.Extensions;
using Core.Interfaces.Repositories;
using Core.Models;

namespace Console.Menu;

public class AssetsMenu : IAssetMenu
{
    private readonly IFinancialAnalyzerService _analyzer;
    private readonly IAvanzaRepository _repository;

    public AssetsMenu(IFinancialAnalyzerService analyzer, IAvanzaRepository repository)
    {
        _analyzer = analyzer;
        _repository = repository;
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
                new MenuItem(1, "Profit or loss", OnClick: () => ShowProfitOrLoss(portfolio)),
                new MenuItem(2, "Show stocks", () => ShowAsset(AssetType.Stock, portfolio)),
                new MenuItem(3, "Show funds", () => ShowAsset(AssetType.Fund, portfolio)),
                new MenuItem(4, "Show ETFs", () => ShowAsset(AssetType.ExchangeTradedFund, portfolio))
            );
            menu.Display();
            shouldContinue = menu.GotoUserSelection();
        }

        return true;
    }

    private bool ShowProfitOrLoss(Portfolio portfolio)
    {
        _analyzer.Get(AnalysisCalculationType.ProfitOrLoss).PrintToConsole();
        portfolio.Print();
        return PressAnyKeyToReturn();
    }

    private bool ShowAsset(AssetType type, Portfolio portfolio)
    {
        portfolio.Select(type).PrintProfitOrLoss();
        portfolio.Print(type);

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
