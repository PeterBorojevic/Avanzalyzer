using Core.Common.Interfaces;
using Core.Models;
using Core.Models.Data;
using Core.Models.Securities.Base;
using System.Globalization;
using Core.Interfaces.Repositories;

namespace Infrastructure.Repositories;

public class AvanzaRepository : IAvanzaRepository
{
    private readonly IAccountParser _accountParser;
    private readonly IPositionParser _positionParser;
    private readonly ITransactionParser _transactionParser;
    private readonly string _solutionRoot;
    private const string AccountFileSuffix = "_konto.csv";
    private const string PositionFileSuffix = "_positioner.csv";

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
        var fileName = "transaktioner_2017-09-25_2024-08-02.csv";
        var filePath = Path.GetFullPath(Path.Combine(_solutionRoot, "..", "..", "..", "..", "..", "data", fileName));
        var transactions = _transactionParser.ParseAccounts(filePath);

        return transactions;
    }
}