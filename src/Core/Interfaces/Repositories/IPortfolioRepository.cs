using Core.Models;
using Core.Models.Data;

namespace Core.Interfaces.Repositories;

public interface IPortfolioRepository
{
    Portfolio LoadPortfolioData();
    List<Transaction> LoadTransactions();
}
