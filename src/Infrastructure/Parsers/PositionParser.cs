using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Core.Common.Interfaces;
using Core.Models.Dtos.Csv;

namespace Infrastructure.Parsers;

public class PositionParser : IPositionParser
{
    public List<Position> ParsePositions(string filePath)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = true,
        };
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);
        return csv.GetRecords<Position>().ToList();
    }
}
