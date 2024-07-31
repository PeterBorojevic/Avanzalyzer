using Core.Models;

namespace Application.Interfaces;

public interface IAccountParser
{
    List<Account> ParseAccounts(string filePath);
}
