using Core.Models.Dtos;

namespace Application.Interfaces;

public interface IAccountParser
{
    List<Account> ParseAccounts(string filePath);
}
