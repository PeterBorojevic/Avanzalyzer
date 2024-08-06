using Core.Models;
using Core.Models.Data;

namespace Core.Interfaces.Repositories;

public interface IAvanzaRepository
{
    Portfolio LoadPortfolioData();
    List<Transaction> LoadTransactions();
}
