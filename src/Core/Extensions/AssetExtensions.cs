using Core.Common.ConsoleTables;
using Core.Common.ConsoleTables.v1;
using Core.Models.Dtos;
using Core.Models.Securities.Base;

namespace Core.Extensions;

public static class AssetExtensions
{
    public static decimal MarketValue(this IEnumerable<Asset> assets)
    {
        return assets.Sum(a => a.MarketValue);
    }
    public static decimal ProfitOrLoss(this IEnumerable<Asset> assets)
    {
        return assets.Sum(a => a.ProfitOrLoss);
    }

    public static void PrintProfitOrLoss(this IList<Asset> assets)
    {
        // Header and color setup TODO refactor
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
        foreach (var position in assets.OrderByDescending(p => p.MarketValue))
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

public static class StringExtensions
{
    public static bool IsDecimal(this string input)
    {
        return decimal.TryParse(input, out _);
    }
    
}
