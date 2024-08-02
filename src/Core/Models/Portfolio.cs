using Core.Common.ConsoleTables;
using Core.Common.Enums;
using Core.Extensions;
using Core.Models.Dtos;
using Core.Models.Securities;
using Core.Models.Securities.Base;

namespace Core.Models;

public class Portfolio
{
    public Portfolio(List<Asset> assets)
    {
        Assets = assets;
        Accounts = new List<Account>();
    }
    public Portfolio(IEnumerable<Account> accounts, List<Position> positions)
    {
        Accounts = accounts.Where(a => a.Balance != decimal.Zero).ToList();
        Assets = new List<Asset>(ConvertPositionsToAssets(positions));
    }

    private IEnumerable<Asset> ConvertPositionsToAssets(List<Position> positions)
    {
        var assets = positions.Select(p =>
        {
            return p.Type switch
            {
                "STOCK" => new Stock(p) as Asset,
                "FUND" => new Fund(p),
                "EXCHANGE_TRADED_FUND" => new ExchangeTradedFund(p),
                "CERTIFICATE" => new Certificate(p),
                _ => throw new ArgumentOutOfRangeException()
            };
        });

        return assets;
    }
    public List<Asset> Assets { get; }
    public List<Account> Accounts { get; set; }

    public decimal Value => Assets.MarketValue();
    public decimal Profit => Assets.ProfitOrLoss();

    public IList<Asset> Select(AssetType type)
    {
        return Assets.Where(a => a.Type == type).ToList();
    }


    //public IEnumerable<Asset> Get(Account type)
    //TODO Portfolio should not be the printer. We could make an interface that contains a method for extracting printable objects
    public void Print(AssetType type)
    {
        var assets = Select(type);
        var total = assets.MarketValue();
        ExtendedConsole.WriteLine($"Total: {total.ToString("##"):green} kr.");

        var totalProfit = assets.ProfitOrLoss();
        ExtendedConsole.WriteLine($"P/L: {totalProfit.ToString("##"):green} kr.");

        var percentageGain = decimal.Divide(totalProfit, total);
        ExtendedConsole.WriteLine($"Yield: {percentageGain.ToString("P"):yellow}.");
    }
    
}
