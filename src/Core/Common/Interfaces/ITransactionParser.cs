using Core.Models.Data;

namespace Core.Common.Interfaces;

public interface ITransactionParser
{
    List<Transaction> ParseAccounts(string filePath);
}
