using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Core.Common.Converters;

public class CustomDecimalConverter : DecimalConverter
{
    public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text) || text == "-")
        {
            return 0m;
        }

        return base.ConvertFromString(text, row, memberMapData)!;
    }
}
