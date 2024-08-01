using Core.Common.ConsoleTables.v2;
using Core.Common.Enums;
using Core.Models.Dtos;
using Core.Models.Securities;
using Core.Models.Securities.Base;

namespace Core.Models;

public class Portfolio
{
    public Portfolio()
    {
        Accounts = new List<Account>();
        Positions = new List<Position>();
    }
    public Portfolio(IEnumerable<Account> accounts, List<Position> positions)
    {
        Accounts = accounts.Where(a => a.Balance != decimal.Zero).ToList();
        Positions = positions;
    }
    public List<Account> Accounts { get; set; }
    private List<Position> Positions { get; set; }

    public IEnumerable<Asset> Get(AssetType type)
    {
        return type switch
        {
            AssetType.Stock => Positions.Where(p => p.Type == "STOCK").Select(p => new Stock(p)),
            AssetType.Fund => Positions.Where(p => p.Type == "FUND").Select(p => new Fund(p)),
            AssetType.Etf => Positions.Where(p => p.Type == "EXCHANGE_TRADED_FUND").Select(p => new ExchangeTradedFund(p)),
            AssetType.Certificate => Positions.Where(p => p.Type == "CERTIFICATE").Select(p => new Certificate(p)),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    //public IEnumerable<Asset> Get(Account type)

    public void Print(AssetType type)
    {
        var assets = Get(type);
        var headers = new Column[]
        {
            new("Name", ConsoleColor.White),
            new("Value", ConsoleColor.Yellow),
        };
        var table = new ConsoleTable(headers);
        
    }
    
}
