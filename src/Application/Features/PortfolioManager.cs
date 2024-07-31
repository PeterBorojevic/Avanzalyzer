using System.Globalization;
using Application.Interfaces;
using Core.Models;

namespace Application.Features;

public class PortfolioManager
{
    private readonly IAccountParser _accountParser;
    private readonly IPositionParser _positionParser;
    private readonly string _solutionRoot;
    private const string AccountFileSuffix = "_konto.csv";
    private const string PositionFileSuffix = "_positioner.csv";

    public PortfolioManager(IAccountParser accountParser, IPositionParser positionParser)
    {
        _accountParser = accountParser;
        _positionParser = positionParser;
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

        return new Portfolio();
    }

}