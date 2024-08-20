using Core.Common.Interfaces;
using Core.Models;
using Core.Models.Data;
using Core.Models.Securities.Base;
using System.Globalization;
using Core.Interfaces.Repositories;

namespace Infrastructure.Repositories;
// TODO Move filepath to Options classes and dependency injection instead of hard coded mess
public class AvanzaRepository : IAvanzaRepository
{
    private readonly IAccountParser _accountParser;
    private readonly IPositionParser _positionParser;
    private readonly ITransactionParser _transactionParser;
    private readonly string _solutionRoot;
    private const string AccountFileSuffix = "_konto.csv";
    private const string PositionFileSuffix = "_positioner.csv";
    private readonly Dictionary<string, decimal> _assetValue = new();

    public AvanzaRepository(IAccountParser accountParser, IPositionParser positionParser, ITransactionParser transactionParser)
    {
        _accountParser = accountParser;
        _positionParser = positionParser;
        _transactionParser = transactionParser;
        _solutionRoot = AppDomain.CurrentDomain.BaseDirectory;
    }

    /// <summary>
    /// Searches for the latest file, starting from current day and backwards, for a maximum of ten days back.
    /// </summary>
    /// <returns>A portfolio</returns>
    public Portfolio LoadPortfolioData()
    {
        var currentDate = DateTime.UtcNow;
        for (var i = 0; i < 10; i++)
        {
            try
            {
                var date = currentDate.AddDays(-i).ToString("d", new CultureInfo("sv-SE"));
                var accountsFilePath = Path.GetFullPath(Path.Combine(_solutionRoot, "..", "..", "..", "..", "..", "data", date + AccountFileSuffix));
                var positionsFilePath = Path.GetFullPath(Path.Combine(_solutionRoot, "..", "..", "..", "..", "..", "data", date + PositionFileSuffix));
                var accounts = _accountParser.ParseAccounts(accountsFilePath);
                var positions = _positionParser.ParsePositions(positionsFilePath);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Files found from " + date);
                Console.ResetColor();
                return new Portfolio(accounts, positions);
            }
            catch (Exception)
            {
                continue;
            }
        }

        return new Portfolio(new List<Asset>());
    }
    
    public List<Transaction> LoadTransactions()
    {
        const string fileName = "transaktioner_2017-09-25_2024-08-20.csv";
        var filePath = Path.GetFullPath(Path.Combine(_solutionRoot, "..", "..", "..", "..", "..", "data", fileName));
        var transactions = _transactionParser.ParseAccounts(filePath);

        return transactions;
    }

    //TODO extract market value of each asset type,
    //TODO store in a format that can be queried. For now dictionary
    public void LoadPortfolioAssetsMarketValue()
    {
        var portfolio = LoadPortfolioData();
        foreach (var asset in portfolio.Assets)
        {
            var name = asset.Name;
            var marketValuePerUnit = decimal.Divide(asset.MarketValue, asset.Quantity);
            _assetValue.Add(name, marketValuePerUnit);
        }
    }

    public decimal GetAssetMarketValue(string assetName)
    {
        if (!_assetValue.Any()) LoadPortfolioAssetsMarketValue();
        return _assetValue.ContainsKey(assetName) ? _assetValue[assetName] : 0;
    } 
}
