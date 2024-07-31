using Core.Common.ConsoleTables;
using Core.Common.ConsoleTables.v1;
using Core.Models;

namespace Core.Extensions;

public static class PositionExtensions
{

    public static void PrintProfitOrLoss(this IEnumerable<Position> positions)
    {
        var table = new ConsoleTable(new ConsoleTableOptions()
        {
            Columns = new List<string>(){ "Value", "Name", "P/L", "%"},
            NumberAlignment = Alignment.Right
        });
        foreach (var position in positions.OrderByDescending(p => p.MarketValue))
        {
            table.AddRow(
                $"{position.MarketValue:##}",
                $"{position.Name:blue}",
                $"{position.ProfitOrLoss:##}",
                $"{position.PercentageChange:P}"
            );
        }
        table.Write(format: Format.Color);
        var total = positions.Sum(p => p.MarketValue);
        ExtendedConsole.WriteLine($"Total: {total.ToString("##"):green} kr.");
        var totalProfit = positions.Sum(p => p.ProfitOrLoss);
        ExtendedConsole.WriteLine($"P/L: {totalProfit.ToString("##"):green} kr.");
        var percentageGain = decimal.Divide(totalProfit, total);
        ExtendedConsole.WriteLine($"Yield: {percentageGain.ToString("P"):yellow}.");
    }
}

public static class StringExtensions
{
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