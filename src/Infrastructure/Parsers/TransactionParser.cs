using Core.Models.Dtos.Csv;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using Core.Common.Interfaces;
using Core.Models.Data;
using Infrastructure.ResponseModels;

namespace Infrastructure.Parsers;

public class TransactionParser : ITransactionParser
{
    public List<Transaction> ParseAccounts(string filePath)
    {
        var config = new CsvConfiguration(new CultureInfo("sv-SE", false))
        {
            Delimiter = ";",
            HasHeaderRecord = true,
        };
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);
        return csv.GetRecords<AvanzaTransactionDto>().Select(t => t.ToInternal()).ToList();
    }
}
