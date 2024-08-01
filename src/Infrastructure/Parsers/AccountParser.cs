using System.Globalization;
using Core.Common.Interfaces;
using Core.Models.Dtos;
using CsvHelper;
using CsvHelper.Configuration;

namespace Infrastructure.Parsers;

public class AccountParser : IAccountParser
{
    public List<Account> ParseAccounts(string filePath)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = true,
        };
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);
        return csv.GetRecords<Account>().ToList();
    }

}