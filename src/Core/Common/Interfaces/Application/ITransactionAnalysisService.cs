using Core.Models.Data;

namespace Core.Common.Interfaces.Application;

public interface ITransactionAnalysisService
{
    InvestmentPortfolio ParseTransactions(IList<Transaction> transactions, bool verbose = false);
}
