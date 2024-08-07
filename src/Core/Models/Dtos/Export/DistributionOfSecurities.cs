using Core.Common.ConsoleTables.v1;
using Core.Common.Enums;
using Core.Common.Interfaces;

namespace Core.Models.Dtos.Export;

public class DistributionOfSecurities : IPrintable
{
    private readonly Portfolio _portfolio;
    private readonly IEnumerable<AssetType> _assetTypes;
    private readonly List<ColumnInColor> _columns = new()
    {
        new ColumnInColor("Asset", _ => ConsoleColor.Yellow),
        new ColumnInColor("Percentage [%]", ColorFunctions.PercentagePositiveBlue_NegativeRed),
        new ColumnInColor("Amount [kr]", ColorFunctions.PositiveGreen_NegativeRed),
    };

    public DistributionOfSecurities(Portfolio portfolio)
    {
        _portfolio = portfolio;
        _assetTypes = portfolio.Assets.Select(a => a.Type);
    }
    public void PrintToConsole()
    {
        var grouping = _portfolio.Assets.GroupBy(a => a.Type);
        var table = new ConsoleTable(_columns);
        var totalValue = _portfolio.Value;
        foreach (var assets in grouping)
        {
            table.AddRow(
                assets.Key.PluralName(), 
                $"{decimal.Divide(assets.Sum(a => a.MarketValue), totalValue):P}",
                $"{assets.Sum(a => a.MarketValue):##}"
                );
        }

        table.Write(Format.Color);
    }
}
