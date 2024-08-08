using Core.Common.ConsoleTables.v1;
using Core.Common.Enums;
using Core.Common.Interfaces;
using Core.Models.Data;
using Core.Models.Securities.Base;

namespace Core.Models.Dtos.Export;

public class ProfitOrLoss : IPrintable
{
    private readonly List<Asset> _assets;
    private readonly IEnumerable<Transaction>? _dividends;
    private readonly Portfolio _portfolio;

    public ProfitOrLoss(Portfolio portfolio, IEnumerable<Transaction>? dividends = null)
    {
        _dividends = dividends;
        _assets = portfolio.Assets;
        _portfolio = portfolio;
    }

    private static ConsoleTable CreateTable()
    {
        var columnInColors = new List<ColumnInColor>()
        {
            new("Namn"),
            new("Värde", ColorFunctions.ValuesAbove(1000, ConsoleColor.Blue)
                .And(ColorFunctions.ValuesBelow(1000, ConsoleColor.DarkBlue))),
            new("Sedan köp [kr]", ConsoleColorFunctions.PositiveOrNegative),
            new("Sedan köp [%]", ConsoleColorFunctions.Percentage),
        };
        return new ConsoleTable(columnInColors);
    }
    
    private static ConsoleTable CreateTableWithDividends()
    {
        var columnInColors = new List<ColumnInColor>()
        {
            new("Namn"),
            new("Värde", ColorFunctions.ValuesAbove(1000, ConsoleColor.Blue)
                .And(ColorFunctions.ValuesBelow(1000, ConsoleColor.DarkBlue))),
            new("Sedan köp [kr]", ConsoleColorFunctions.PositiveOrNegative),
            new("Sedan köp [%]", ConsoleColorFunctions.Percentage),
            new("Utdelningar [kr]", cell => ConsoleColor.Green),
            new("Sedan köp inkl. utd [%]", ConsoleColorFunctions.Percentage),
        };
        return new ConsoleTable(columnInColors);
    }

    public void PrintToConsole()
    {
        var table = _dividends is null ? CreateTable() : CreateTableWithDividends();
        // Add each value
        foreach (var position in _assets.OrderByDescending(p => p.MarketValue))
        {
            var row = CreateRow(position);
            table.AddRow(row);
        }
        // Print
        table.Write(format: Format.Color);
    }

    private object[] CreateRow(Asset asset)
    {
        var row = new List<object>
        {
            $"{asset.Name}",
            $"{asset.MarketValue:##}",
            $"{asset.ProfitOrLoss:##}",
            $"{asset.PercentageChange:P}"
        };
        if (_dividends is not null)
        {
            //var accounts = _portfolio.Accounts.Where(a => a.AccountNumber)
            var assetDividends = _dividends.Where(d => d.AssetNameOrDescription.Contains(asset.Name)).Sum(d => d.Amount);
            row.Add($"{assetDividends:##}");
            row.Add($"{decimal.Divide(asset.MarketValue + assetDividends, asset.AcquisitionCost) - 1:P}");
        }

        return row.ToArray();
    }
    
}
