using Core.Models;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using Application.Interfaces;

namespace Application.Features.Parsers;

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