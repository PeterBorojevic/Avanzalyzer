using Core.Common.Interfaces;

namespace Core.Models.Dtos;

public class AccountTotals : IPrintable
{
    public AccountTotals(Portfolio portfolio)
    {
        
    }
    public void PrintToConsole()
    {
        throw new NotImplementedException();
    }
}
