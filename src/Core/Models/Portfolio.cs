using Core.Common.ConsoleTables.v2;
using Core.Common.Enums;
using Core.Models.Dtos;
using Core.Models.Securities;
using Core.Models.Securities.Base;
using System.Collections.Generic;

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

    public IEnumerable<Asset> Select(AssetType type)
    {
        return Assets.Where(a => a.Type == type);
    }

    //public IEnumerable<Asset> Get(Account type)

    public void Print(AssetType type)
    {
       //TODO Portfolio should not be the printer. We could make an interface that contains a method for extracting printable objects
    }
    
}
