using Console.Interfaces;
using Console.Models;
using Core.Common.ConsoleTables;
using Core.Common.ConsoleTables.v1;
using Core.Common.Interfaces.Application;
using Core.Extensions;
using Core.Models.Data;
using Core.Models.Securities.Base;

namespace Console.Menu;

public class TransactionsMenu : Base.Menu, ITransactionsMenu
{
    private readonly ITransactionAnalysisService _transactionAnalysisService;
    public TransactionsMenu(ITransactionAnalysisService transactionAnalysisService)
    {
        _transactionAnalysisService = transactionAnalysisService;
    }

    public UserAction Next()
    {
        throw new NotImplementedException();
    }

    public bool Run()
    {
        var portfolio = _transactionAnalysisService.ParseTransactions();
        var shouldContinue = true;
        while (shouldContinue)
        {
            var menu = new NumberedMenu("TRANSACTIONS \n\nPress a number \n ",
                new MenuItem(0, "Return", Return),
                new MenuItem(1, "Search specific asset", OnClick: () => Search(portfolio))
            );
            menu.Display();
            shouldContinue = menu.GotoUserSelection();
        }

        return true;
    }

    private bool Search(InvestmentPortfolio portfolio)
    {
        System.Console.Clear();
        var tradedAssets = portfolio.TradedAssets;
        var substring = "";
        var assets = tradedAssets.ToList();

        while (!substring.Contains("\r") && assets.Any())
        {
            ExtendedConsole.WriteLine($"\n{"Search for specific asset":magenta}");
            ExtendedConsole.Write($"{"Name: ":white}{substring:yellow}");
            var input = char.ToLower(ReadCharInput());
            if (input == '\r') break;
            substring += input;

            assets = tradedAssets.Where(a => a.ToLower().Contains(substring)).ToList();
            if (assets.Count == 1) break;
            System.Console.Clear();
            foreach (var asset in assets)
            {
                ExtendedConsole.WriteLine($"{asset:cyan}");
            }
        }

        if (!assets.Any())
        {
            ExtendedConsole.WriteLine($"\n{"No asset found, press any key to try again":red}");
            return PressAnyKeyToReturn;
        }

        var menuItems = assets.Select((a, i) => new MenuItem(i, a, () => PrintAsset(portfolio, a)));
        
        var shouldContinue = true;
        while (shouldContinue)
        {
            var menu = new NumberedMenu("SELECT", menuItems.ToArray());
            menu.Display();
            shouldContinue = menu.GotoUserSelection();
        }

        return PressAnyKeyToReturn;
    }
    

    private bool PrintAsset(InvestmentPortfolio portfolio, string assetName)
    {
        // TODO 
        // Beräkna ROI
        // Antal trades
        // Courtage
        // GAV
        // Marknadsvärde
        // Antal aktier/andelar eller om såld
        
        // - Nedlagda kostnader
        // + Intäkter vid försäljning
        // + Utdelningar
        //(+)Marknadsvärde
        // = Profit or loss

        ExtendedConsole.WriteLine($"\nYou have selected {assetName:cyan}");
        var isin = portfolio.AssetNameToISIN[assetName];
        var asset = portfolio.GetAsset(isin);
        asset?.PrintProfitOrLoss();

        return false;
    }

    private void Print(Asset asset, InvestmentPortfolio portfolio)
    {
        // Header and color setup
        var columnInColors = new List<ColumnInColor>()
        {
            new("Namn"),
            new("Antal", ColorFunctions.ValuesAbove(100, ConsoleColor.Blue)
                .And(ColorFunctions.ValuesBelow(100, ConsoleColor.DarkBlue))),
            new("Sedan köp [kr]", ConsoleColorFunctions.PositiveOrNegative),
            new("Sedan köp [%]", ConsoleColorFunctions.Percentage),
            new("Nedlagda kostnader", cell => ConsoleColor.Red),
            new("Utdelningar", cell => ConsoleColor.Green),
            new("Realiserad vinst", ConsoleColorFunctions.PositiveOrNegative),
            new("Marknadsvärde", cell => ConsoleColor.Red),
        };
        var table = new ConsoleTable(columnInColors);
        // Add each value

        table.AddRow(
            $"{asset.Name}",
            $"{asset.Quantity:##}",
            $"{asset.ProfitOrLoss:##}",
            $"{asset.PercentageChange:P}",
            $"{asset.PercentageChange:P}",
            $"{asset.PercentageChange:P}",
            $"{asset.PercentageChange:P}"
        );

        // Print
        table.Write(format: Format.Color);
    }

    internal static char ReadCharInput() => System.Console.ReadKey(false).KeyChar;
}

/*
 *  Enter = "\r"
 *
 */