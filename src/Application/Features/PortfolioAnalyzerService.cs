using Core.Common.ConsoleTables.v1;
using Core.Common.Enums;
using Core.Common.Interfaces.Application;
using Core.Extensions;
using Core.Interfaces.Repositories;
using Core.Models.Data;
using System.Transactions;
using Core.Common.ConsoleTables;
using Core.Models;
using Transaction = Core.Models.Data.Transaction;
using Core.Common.Constants;

namespace Application.Features;

/// <summary>
/// Analyses transactions to compute portfolio investment returns and performance.
/// </summary>
public class PortfolioAnalyzerService : IPortfolioAnalyzerService
{
    private readonly IAvanzaRepository _avanzaRepository;
    private readonly ITransactionAnalysisService _transactionAnalysis;
    public PortfolioAnalyzerService(IAvanzaRepository avanzaRepository, ITransactionAnalysisService transactionAnalysis)
    {
        _avanzaRepository = avanzaRepository;
        _transactionAnalysis = transactionAnalysis;
    }

    public void LoadTransactions()
    {
        var transactions = _avanzaRepository.LoadTransactions();
        _transactionAnalysis.ParseTransactions(transactions); // Test
        RoiPerAssetTraded(transactions);

        var buyOrSell = transactions
            .Where(t => t.TransactionType is TransactionType.Buy or TransactionType.Sell)
            .OrderBy(t => t.Date)
            .ToList();
        var dividends = transactions.SelectDividendRelatedTransactions()?.Sum(x => x.Amount) ?? 0;
        // Sum of all trades = realised P/L + acquisition of current portfolio
        var totalAcquisitionValue = buyOrSell.Sum(x => x.Amount);
        var currentMarketValue = _avanzaRepository.LoadPortfolioData().Value;
        var totalBrokerageFees = buyOrSell.Sum(x => x.BrokerageFee);
        var returns = currentMarketValue + dividends + totalAcquisitionValue - totalBrokerageFees;
        var altRoi = returns / (-totalAcquisitionValue + totalBrokerageFees);
        var roi = ROI(transactions, verbose: true);

        var tradedAssets = buyOrSell.Select(t => t.AssetNameOrDescription).Distinct();
    }

    public void RoiPerAssetTraded(List<Transaction> transactions)
    {
        var buyOrSell = transactions
            .Where(t => t.TransactionType is TransactionType.Buy or TransactionType.Sell)
            .OrderBy(t => t.Date)
            .ToList();
        var dividends = transactions.SelectDividendRelatedTransactions()?.ToList();

        var tradedAssetsPerAccount = buyOrSell.Concat(dividends).GroupBy(x => x.AccountName);
        var portfolio = _avanzaRepository.LoadPortfolioData();
        // TODO account for assets being moved between accounts
        // Maybe instead of using the portfolio, count the assets and use a repository for asset values

        foreach (var tradedAssets in tradedAssetsPerAccount)
        {
            var accountName = tradedAssets.Key;
            ExtendedConsole.WriteLine($"Account: {accountName:yellow}");
            var assets = tradedAssets.Select(a => a.AssetNameOrDescription).Distinct();
            var accountPortfolio = portfolio.Select(AccountNumbers.Get(accountName));
            var roi = ROI(tradedAssets.ToList(), accountPortfolio.MarketValue(), true);
        }
    }

    private static decimal TotalCosts(IList<Transaction> transactions, bool verbose = false)
    {
        var buys = transactions.Where(t => t.TransactionType is TransactionType.Buy).ToList();
        var sells = transactions.Where(t => t.TransactionType is TransactionType.Sell);

        var acquisitionCosts = buys.Sum(x => x.Amount); // Negative values
        var brokerageFees = -buys.Concat(sells).Sum(x => x.BrokerageFee /* Positive values */);

        var totalCosts = acquisitionCosts + brokerageFees;

        if (verbose)
        {
            var columns = new List<ColumnInColor>()
            {
                new("TOTAL COSTS", ConsoleColorFunctions.Normal),
                new(totalCosts.ToString("C"),  ConsoleColorFunctions.PositiveOrNegative)
            };
            var table = new ConsoleTable(columns);
            table.AddRow("Investments", $"{acquisitionCosts:##}");
            table.AddRow("Brokerage fees", $"{brokerageFees:##}");
            table.Write(Format.Color);
        }

        return totalCosts;
    }

    private decimal NetGain(IList<Transaction> transactions, decimal? marketValue = null, bool verbose = false)
    {
        var realisedProfitOrLoss = transactions.Where(t => t.TransactionType is TransactionType.Sell).Sum(x => x.Amount);
        var currentMarketValue = marketValue ?? _avanzaRepository.LoadPortfolioData().Value; //TODO this is the market value of the entire portfolio, should filter
        var dividends = transactions.SelectDividendRelatedTransactions()?.Sum(x => x.Amount) ?? 0;
        var costs = TotalCosts(transactions);

        var netGain = currentMarketValue + realisedProfitOrLoss + dividends + costs;

        if (verbose)
        {
            var columns = new List<ColumnInColor>()
            {
                new("NET GAINS", ConsoleColorFunctions.Normal),
                new(netGain.ToString("C"),  ConsoleColorFunctions.PositiveOrNegative)
            };
            var table = new ConsoleTable(columns);
            table.AddRow("Realised profit or loss", $"{realisedProfitOrLoss:##}");
            table.AddRow("Dividends", $"{dividends:##}");
            table.AddRow("Current market value of investments", $"{currentMarketValue:##}");
            table.AddRow("Total costs", $"{costs:##}");
            table.Write(Format.Color);
        }

        return netGain;
    }

    private decimal ROI(IList<Transaction> transactions, decimal? marketValue = null, bool verbose = false)
    {
        // ROI
        // = Net gain / initial cost
        // = (market value or sell price + dividends - initial cost) / initial cost
        // = (market value or sell price + dividends) / initial cost -1
        var netGain = NetGain(transactions, marketValue, verbose);
        var costs = -TotalCosts(transactions, verbose);
        var roi = netGain / (costs);

        if (verbose)
        {
            Console.WriteLine($"Net gain: {netGain:C}");
            Console.WriteLine($"Total costs: {costs:C}");
            Console.WriteLine($"ROI: {roi:P}");
        }

        return roi;
    }
}

/*
 * Market value can be extracted from positions excel (eg. portfolio)
 */