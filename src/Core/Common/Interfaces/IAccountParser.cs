using Core.Models.Dtos;

namespace Core.Common.Interfaces;

public interface IAccountParser
{
    List<Account> ParseAccounts(string filePath);
}
