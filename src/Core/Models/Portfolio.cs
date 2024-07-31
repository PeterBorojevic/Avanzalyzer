using Core.Common.ConsoleTables.v2;
using Core.Common.Enums;

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
    public List<Position> Positions { get; set; }

    public IEnumerable<Position> Get(AssetType type)
    {
        return type switch
        {
            AssetType.Stock => Positions.Where(p => p.Type == "STOCK"),
            AssetType.Fund => Positions.Where(p => p.Type == "FUND"),
            AssetType.Etf => Positions.Where(p => p.Type == "EXCHANGE_TRADED_FUND"),
            AssetType.Certificate => Positions.Where(p => p.Type == "CERTIFICATE"),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

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
