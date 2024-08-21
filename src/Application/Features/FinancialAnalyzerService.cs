using Core.Common.Enums;
using Core.Common.Interfaces;
using Core.Common.Interfaces.Application;
using Core.Extensions;
using Core.Interfaces.Repositories;
using Core.Models;
using Core.Models.Data;
using Core.Models.Dtos.Export;
using Core.Models.Settings;

namespace Application.Features;

public class FinancialAnalyzerService : IFinancialAnalyzerService
{
    private readonly IAvanzaRepository _avanzaRepository;

    public FinancialAnalyzerService(IAvanzaRepository avanzaRepository)
    {
        _avanzaRepository = avanzaRepository;
    }

    public IPrintable Get(AnalysisCalculationType type, Portfolio? portfolio = null, List<Transaction>? transactions = null)
    {
        return type switch
        {
            AnalysisCalculationType.Dividends => GetDividends(GroupingType.ByYear, transactions),
            AnalysisCalculationType.DepositsAndWithdrawals => GetDepositsAndWithdrawals(),
            AnalysisCalculationType.AccountTotals => GetAccountTotals(),
            AnalysisCalculationType.DistributionOfSecurities => GetDistributionOfSecurities(),
            AnalysisCalculationType.SectoralBreakdown => throw new NotImplementedException(), // Need external information
            AnalysisCalculationType.ProfitOrLoss => GetProfitOrLoss(portfolio, transactions),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public IPrintable Get(AnalysisCalculationType type, Action<FinancialAnalysisOptions> options)
    {
        var settings = new FinancialAnalysisOptions();
        options.Invoke(settings);
        var transactions = settings.UseTransactions ? _avanzaRepository.LoadTransactions() : null;

        return Get(type, null, transactions);
    }

    private IPrintable GetProfitOrLoss(Portfolio? portfolio, List<Transaction>? transactions = null)
    {
        portfolio ??= _avanzaRepository.LoadPortfolioData();
        return new ProfitOrLoss(portfolio, transactions.SelectDividendRelatedTransactions());
    }

    private IPrintable GetDividends(GroupingType groupBy = GroupingType.ByYear, List<Transaction>? transactions = null)
    {
        transactions ??= _avanzaRepository.LoadTransactions();
        var dividends = transactions.SelectDividendRelatedTransactions();

        return new DividendsOverTime(dividends, shouldGroupByYear: groupBy is GroupingType.ByYear);
    }

    private IPrintable GetDepositsAndWithdrawals()
    {
        var transactions = _avanzaRepository.LoadTransactions();
        var depositsAndWithdrawals = transactions.Where(t =>
            t.TransactionType is TransactionType.Deposit or TransactionType.Withdraw);

        return new DepositsAndWithdrawals(depositsAndWithdrawals);
    }

    private IPrintable GetAccountTotals()
    {
        var portfolio = _avanzaRepository.LoadPortfolioData();
        return new AccountTotals(portfolio);
    }

    private IPrintable GetDistributionOfSecurities()
    {
        var portfolio = _avanzaRepository.LoadPortfolioData();
        return new DistributionOfSecurities(portfolio);
    }

    

    // Analyze transactions
    /*
     * (done) Dividends over years/months
     * (todo) Extract current portfolio, with historic profit/loss data
     * 
     */


    // Analyze portfolio
}
