using Core.Common.ConsoleTables;
using Core.Common.ConsoleTables.v1;
using Core.Models.Dtos;
using Core.Models.Securities.Base;

namespace Core.Extensions;

public static class AssetExtensions
{

    public static void PrintProfitOrLoss(this IEnumerable<Asset> assets)
    {
        // Header and color setup TODO refactor
        var columnInColors = new List<ColumnInColor>()
        {
            new("Namn"),
            new("Värde", ColorFunctions.ValuesAbove(1000, ConsoleColor.Blue)
                .And(ColorFunctions.ValuesBelow(1001, ConsoleColor.DarkBlue))),
            new("Sedan köp [kr]", ConsoleColorFunctions.PositiveOrNegative),
            new("Sedan köp [%]", ConsoleColorFunctions.Percentage),
        };
        var table = new ConsoleTable(new ConsoleTableOptions()
        {
            Columns = columnInColors.Values(),
            ColumnColors = columnInColors.ColorFunctions(),
            NumberAlignment = Alignment.Right
        });
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

        var total = assets.Sum(p => p.MarketValue);
        ExtendedConsole.WriteLine($"Total: {total.ToString("##"):green} kr.");

        var totalProfit = assets.Sum(p => p.ProfitOrLoss);
        ExtendedConsole.WriteLine($"P/L: {totalProfit.ToString("##"):green} kr.");

        var percentageGain = decimal.Divide(totalProfit, total);
        ExtendedConsole.WriteLine($"Yield: {percentageGain.ToString("P"):yellow}.");
    }
}

public static class StringExtensions
{
    public static bool IsDecimal(this string input)
    {
        return decimal.TryParse(input, out _);
    }
    public static void Print(this IList<string> input)
    {
        var maxWidth = input.Max(x => x.Length);
        Table.WriteLine(maxWidth);
        foreach (var line in input)
        {
            var text = $"| {line} |";
            Console.WriteLine(text);
        }
        Table.WriteLine(maxWidth);
    }
}

public class Table
{
    /// <summary>
    /// Prints a line "+---+" of a certain width.
    /// Minimum width = 2. eg. "++"
    /// </summary>
    /// <param name="width"></param>
    public static void WriteLine(int width)
    {
        width = Math.Max(2, width);
        Console.WriteLine($"+{string.Concat(Enumerable.Repeat("-", width - 2))}+");
    }
}