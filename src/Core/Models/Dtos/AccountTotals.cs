using Core.Common.ConsoleTables.v1;
using Core.Common.Interfaces;
using Core.Extensions;
using Core.Models.Securities.Base;

namespace Core.Models.Dtos;

public class AccountTotals : IPrintable
{
    private Dictionary<string, List<Asset>> _accountAssets = new();
    private List<ColumnInColor> _columns = new()
    {
        new ColumnInColor("Konto"),
        new ColumnInColor("Value", cell => ConsoleColor.Cyan),
        new ColumnInColor("Gain [kr]", ColorFunctions.PositiveGreen_NegativeRed),
        new ColumnInColor("Gain [%]", ColorFunctions.PercentagePositiveBlue_NegativeRed),
    };

    public AccountTotals(Portfolio portfolio)
    {
        var accountNumbers = portfolio.Accounts.Select(a => a.AccountNumber);
        foreach (var accountNumber in accountNumbers)
        {
            var assets = portfolio.Assets.Where(a => a.AccountNumber == accountNumber).ToList();
            if (assets.Count > 0) _accountAssets.Add(accountNumber, assets);
        }
    }
    public void PrintToConsole()
    {
        var table = new ConsoleTable(_columns);
        foreach (var kvp in _accountAssets)
        {
            table.AddRow(
                kvp.Key, 
                $"{kvp.Value.MarketValue():##}", 
                $"{kvp.Value.ProfitOrLoss():##}",
                $"{kvp.Value.TotalPercentageGain():P}");
        }
        table.Write(Format.Color);
    }
}
