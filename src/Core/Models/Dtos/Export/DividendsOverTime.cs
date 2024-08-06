using Core.Common.ConsoleTables;
using Core.Common.ConsoleTables.v1;
using Core.Common.Enums;
using Core.Common.Interfaces;
using Core.Models.Data;

namespace Core.Models.Dtos.Export;

public class DividendsOverTime : IPrintable
{
    private readonly IEnumerable<Transaction> _dividends;
    private readonly List<ColumnInColor> _columns = new()
    {
        new ColumnInColor("Date", _ => ConsoleColor.Yellow),
        new ColumnInColor("Utdelning [kr]", cell => ConsoleColor.Cyan),
        new ColumnInColor("Skatt [kr]", ColorFunctions.PositiveGreen_NegativeRed),
        new ColumnInColor("Totalt [kr]", ColorFunctions.PositiveGreen_NegativeRed),
    };

    private readonly bool _shouldGroupByYear;

    public DividendsOverTime(IEnumerable<Transaction> dividends, bool shouldGroupByYear = false)
    {
        _dividends = dividends;
        _shouldGroupByYear = shouldGroupByYear;
    }

    public void PrintToConsole()
    {
        var dividendBuckets = SplitDividendsIntoGroupsByDate();
        var sum = _dividends.Sum(d => d.Amount);
        var table = new ConsoleTable(_columns);
        foreach (var dividends in dividendBuckets)
        {
            var totalDividends = dividends.Where(d => d.TransactionType is TransactionType.Dividend).Sum(d => d.Amount);
            table.AddRow(
                GetGroupDate(dividends, _shouldGroupByYear),
                $"{totalDividends}",
                $"{dividends.Where(d => d.TransactionType is TransactionType.DividendProvisionalTax or TransactionType.ForeignTax).Sum(d => d.Amount):##}",
                $"{dividends.Sum(d => d.Amount)}"
                );
        }
        
        table.Write(Format.Color);

        ExtendedConsole.WriteLine($"Total dividends after taxes: {sum.ToString("C"):green} \n");
    }

    private IEnumerable<IGrouping<DateTime, Transaction>> SplitDividendsIntoGroupsByDate() =>
        _dividends.GroupBy(d => _shouldGroupByYear ? Year(d) : YearAndMonth(d));

    private static DateTime YearAndMonth(Transaction d) => new (d.Date.Year, d.Date.Month, 1);
    private static DateTime Year(Transaction d) => new (d.Date.Year, 1, 1);

    private static string GetGroupDate(IGrouping<DateTime, Transaction> grouping, bool byYear = false) => grouping.Key.ToString(byYear ? "yyyy" : "yyyy-MM");


}
