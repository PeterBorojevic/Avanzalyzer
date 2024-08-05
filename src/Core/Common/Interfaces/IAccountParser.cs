using Core.Models.Dtos.Csv;

namespace Core.Common.Interfaces;

public interface IAccountParser
{
    List<Account> ParseAccounts(string filePath);
}
