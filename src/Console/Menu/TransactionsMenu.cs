using Console.Interfaces;
using Console.Models;
using Core.Common.Interfaces.Application;

namespace Console.Menu;

public class TransactionsMenu : ITransactionsMenu
{
    private readonly ITransactionAnalysisService _transactionAnalysisService;
    public TransactionsMenu(ITransactionAnalysisService transactionAnalysisService)
    {
        _transactionAnalysisService = transactionAnalysisService;
    }

    public UserAction Next()
    {
        throw new NotImplementedException();
    }

    public bool Run()
    {
        throw new NotImplementedException();
    }
}
