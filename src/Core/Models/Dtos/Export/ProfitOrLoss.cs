using Core.Common.ConsoleTables.v1;
using Core.Common.Interfaces;
using Core.Models.Securities.Base;

namespace Core.Models.Dtos.Export;

public class ProfitOrLoss : IPrintable
{
    private readonly List<Asset> _assets;

    public ProfitOrLoss(Portfolio portfolio)
    {
        _assets = portfolio.Assets;
    }

    public void PrintToConsole()
    {
        var columnInColors = new List<ColumnInColor>()
        {
            new("Namn"),
            new("Värde", ColorFunctions.ValuesAbove(1000, ConsoleColor.Blue)
                .And(ColorFunctions.ValuesBelow(1000, ConsoleColor.DarkBlue))),
            new("Sedan köp [kr]", ConsoleColorFunctions.PositiveOrNegative),
            new("Sedan köp [%]", ConsoleColorFunctions.Percentage),
        };
        var table = new ConsoleTable(columnInColors);
        // Add each value
        foreach (var position in _assets.OrderByDescending(p => p.MarketValue))
        {
            table.AddRow(
                $"{position.Name}",
                $"{position.MarketValue:##}",
                $"{position.ProfitOrLoss:##}",
                $"{position.PercentageChange:P}"
            );
        }
        // Print
        table.Write(format: Format.Color);
    }
}
