using CsvHelper.Configuration;
using CsvHelper;
using CsvHelper.TypeConversion;

namespace Application.Features.Parsers;

public class CustomDecimalConverter : DecimalConverter
{
    public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text) || text == "-")
        {
            return 0m;
        }

        return base.ConvertFromString(text, row, memberMapData);
    }
}