using Core.Models.Data;

namespace Core.Common.Interfaces.Application;

public interface IInvestmentPortfolioCreator
{ 
    InvestmentPortfolio Create(IList<Transaction>? transactions = null, bool verbose = false);
}
