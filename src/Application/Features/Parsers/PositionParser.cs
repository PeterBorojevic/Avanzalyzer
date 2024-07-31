using Application.Interfaces;
using Core.Models;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;

namespace Application.Features.Parsers;

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