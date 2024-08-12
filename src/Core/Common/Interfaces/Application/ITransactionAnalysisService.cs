using Core.Models.Data;

namespace Core.Common.Interfaces.Application;

public interface ITransactionAnalysisService
{
    void ParseTransactions(IList<Transaction> transactions);
}
