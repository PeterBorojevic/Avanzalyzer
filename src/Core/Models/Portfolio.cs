using Core.Common.ConsoleTables;
using Core.Common.Enums;
using Core.Extensions;
using Core.Models.Dtos.Csv;
using Core.Models.Securities.Base;

namespace Core.Models;

public class Portfolio
{
    public Portfolio(List<Asset> assets)
    {
        Assets = assets;
        Accounts = new List<Account>();
    }
    public Portfolio(IEnumerable<Account> accounts, IEnumerable<Position> positions)
    {
        Accounts = accounts.Where(a => a.Balance != decimal.Zero).ToList();
        Assets = positions.Select(p => p.ToAsset()).ToList();
    }
    
    public List<Asset> Assets { get; }
    public List<Account> Accounts { get; set; }

    public decimal Value => Assets.MarketValue();
    public decimal Profit => Assets.ProfitOrLoss();

    public IList<Asset> Select(AssetType type)
    {
        return Assets.Where(a => a.Type == type).ToList();
    }

    public IList<Asset> Select(string accountNumber)
    {
        return Assets.Where(a => a.AccountNumber == accountNumber).ToList();
    }


    //public IEnumerable<Asset> Get(Account type)
    //TODO Portfolio should not be the printer. We could make an interface that contains a method for extracting printable objects
    public void Print(AssetType type)
    {
        var assets = Select(type);
        var total = assets.MarketValue();
        ExtendedConsole.WriteLine($"Total {Enum.GetName(type)}: {total.ToString("##"):green} kr.");

        var totalProfit = assets.ProfitOrLoss();
        ExtendedConsole.WriteLine($"P/L: {totalProfit.ToString("##"):green} kr.");

        var percentageGain = decimal.Divide(totalProfit, total);
        ExtendedConsole.WriteLine($"Yield: {percentageGain.ToString("P"):yellow}.");
    }
    
    public void Print()
    {
        var total = Assets.MarketValue();
        ExtendedConsole.WriteLine($"Total: {total.ToString("##"):green} kr.");

        var totalProfit = Assets.ProfitOrLoss();
        ExtendedConsole.WriteLine($"P/L: {totalProfit.ToString("##"):green} kr.");

        var percentageGain = decimal.Divide(totalProfit, total);
        ExtendedConsole.WriteLine($"Yield: {percentageGain.ToString("P"):yellow}.");
    }
    
}
