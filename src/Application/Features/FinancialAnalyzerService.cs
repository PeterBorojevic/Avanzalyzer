using Core.Common.Enums;
using Core.Common.Interfaces;
using Core.Common.Interfaces.Application;
using Core.Interfaces.Repositories;
using Core.Models.Dtos;
using Core.Models.Dtos.Export;

namespace Application.Features;

public class FinancialAnalyzerService : IFinancialAnalyzerService
{
    private readonly IAvanzaRepository _avanzaRepository;

    public FinancialAnalyzerService(IAvanzaRepository avanzaRepository)
    {
        _avanzaRepository = avanzaRepository;

    }

    public IPrintable Get(AnalysisCalculationType type)
    {
        return type switch
        {
            AnalysisCalculationType.Dividends => GetDividends(),
            AnalysisCalculationType.AccountTotals => GetAccountTotals(),
            AnalysisCalculationType.DistributionOfSecurities => GetDistributionOfSecurities(),
            AnalysisCalculationType.SectoralBreakdown => GetDistributionOfSecurities(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public IPrintable GetDividends(GroupingType groupBy = GroupingType.ByYear)
    {
        var transactions = _avanzaRepository.LoadTransactions();
        var dividends = transactions.Where(t =>
            t.TransactionType is 
                TransactionType.Dividend or 
                TransactionType.DividendProvisionalTax or 
                TransactionType.ForeignTax);

        return new DividendsOverTime(dividends, shouldGroupByYear: groupBy is GroupingType.ByYear);
    }

    public IPrintable GetDepositsAndWithdrawals()
    {
        var transactions = _avanzaRepository.LoadTransactions();
        var depositsAndWithdrawals = transactions.Where(t =>
            t.TransactionType is TransactionType.Deposit or TransactionType.Withdraw);

        return new DepositsAndWithdrawals(depositsAndWithdrawals);
    }

    public IPrintable GetDistributionOfSecurities()
    {
        return new DistributionOfSecurities();
    }

    public IPrintable GetAccountTotals()
    {
        var portfolio = _avanzaRepository.LoadPortfolioData();
        return new AccountTotals(portfolio);
    }

    // Analyze transactions
    /*
     * (done) Dividends over years/months
     * (todo) Extract current portfolio, with historic profit/loss data
     * 
     */


    // Analyze portfolio
}
