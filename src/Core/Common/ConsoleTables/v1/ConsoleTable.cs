using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Common.ConsoleTables.v1;

public class ConsoleTable
{
    public IList<object> Columns { get; }
    public IList<object[]> Rows { get; }

    public ConsoleTableOptions Options { get; }
    public Type[] ColumnTypes { get; private set; }

    public IList<string> Formats { get; private set; }

    public static readonly HashSet<Type> NumericTypes = new HashSet<Type>
    {
        typeof(int),  typeof(double),  typeof(decimal),
        typeof(long), typeof(short),   typeof(sbyte),
        typeof(byte), typeof(ulong),   typeof(ushort),
        typeof(uint), typeof(float)
    };

    public ConsoleTable(params string[] columns)
        : this(new ConsoleTableOptions
        {
            Columns = new List<string>(columns),
            ColumnColors = Enumerable.Range(0, columns.Length)
                .Select(i => new Func<object, ConsoleColor>(o => ConsoleColor.White))
        })
    {
    }

    public ConsoleTable(ConsoleTableOptions options)
    {
        Options = options ?? throw new ArgumentNullException("options");
        Rows = new List<object[]>();
        Columns = new List<object>(options.Columns);
    }

    public ConsoleTable AddColumn(IEnumerable<string> names)
    {
        foreach (var name in names)
            Columns.Add(name);
        return this;
    }

    public ConsoleTable AddRow(params object[] values)
    {
        if (values == null)
            throw new ArgumentNullException(nameof(values));

        if (!Columns.Any())
            throw new Exception("Please set the columns first");

        if (Columns.Count != values.Length)
            throw new Exception(
                $"The number columns in the row ({Columns.Count}) does not match the values ({values.Length})");

        Rows.Add(values);
        return this;
    }

    public ConsoleTable Configure(Action<ConsoleTableOptions> action)
    {
        action(Options);
        return this;
    }


    public static ConsoleTable FromDictionary(Dictionary<string, Dictionary<string, object>> values)
    {
        var table = new ConsoleTable();

        var columNames = values.SelectMany(x => x.Value.Keys).Distinct().ToList();
        columNames.Insert(0, "");
        table.AddColumn(columNames);
        foreach (var row in values)
        {
            var r = new List<object> { row.Key };
            foreach (var columName in columNames.Skip(1))
            {
                r.Add(row.Value.TryGetValue(columName, out var value) ? value : "");
            }

            table.AddRow(r.Cast<object>().ToArray());
        }

        return table;
    }

    public static ConsoleTable From<T>(IEnumerable<T> values)
    {
        var table = new ConsoleTable
        {
            ColumnTypes = GetColumnsType<T>().ToArray()
        };

        var columns = GetColumns<T>().ToList();

        table.AddColumn(columns);

        foreach (
            var propertyValues
            in values.Select(value => columns.Select(column => GetColumnValue<T>(value, column)))
        ) table.AddRow(propertyValues.ToArray());

        return table;
    }

    public static ConsoleTable From(DataTable dataTable)
    {
        var table = new ConsoleTable();

        var columns = dataTable.Columns
            .Cast<DataColumn>()
            .Select(x => x.ColumnName)
            .ToList();

        table.AddColumn(columns);

        foreach (DataRow row in dataTable.Rows)
        {
            var items = row.ItemArray.Select(x => x is byte[] data ? Convert.ToBase64String(data) : x.ToString())
                .ToArray();
            table.AddRow(items);
        }

        return table;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        // find the longest column by searching each row
        var columnLengths = ColumnLengths();

        // set right alinment if is a number
        var columnAlignment = Enumerable.Range(0, Columns.Count)
            .Select(GetNumberAlignment)
            .ToList();

        // create the string format with padding ; just use for maxRowLength
        var format = Enumerable.Range(0, Columns.Count)
            .Select(i => " | {" + i + "," + columnAlignment[i] + columnLengths[i] + "}")
            .Aggregate((s, a) => s + a) + " |";

        SetFormats(ColumnLengths(), columnAlignment);

        // find the longest formatted line
        var maxRowLength = Math.Max(0, Rows.Any() ? Rows.Max(row => string.Format(format, row).Length) : 0);
        var columnHeaders = string.Format(Formats[0], Columns.ToArray());

        // longest line is greater of formatted columnHeader and longest row
        var longestLine = Math.Max(maxRowLength, columnHeaders.Length);

        // add each row
        var results = Rows.Select((row, i) => string.Format(Formats[i + 1], row)).ToList();

        // create the divider
        var divider = " " + string.Join("", Enumerable.Repeat("-", longestLine - 1)) + " ";

        builder.AppendLine(divider);
        builder.AppendLine(columnHeaders);

        foreach (var row in results)
        {
            builder.AppendLine(divider);
            builder.AppendLine(row);
        }

        builder.AppendLine(divider);

        if (Options.EnableCount)
        {
            builder.AppendLine("");
            builder.AppendFormat(" Count: {0}", Rows.Count);
        }

        return builder.ToString();
    }


    private void SetFormats(List<int> columnLengths, List<string> columnAlignment)
    {
        var allLines = new List<object[]>();
        allLines.Add(Columns.ToArray());
        allLines.AddRange(Rows);

        Formats = allLines.Select(d =>
        {
            return Enumerable.Range(0, Columns.Count)
                .Select(i =>
                {
                    var value = d[i]?.ToString() ?? "";
                    var length = columnLengths[i] - (GetTextWidth(value) - value.Length);
                    return " | {" + i + "," + columnAlignment[i] + length + "}";
                })
                .Aggregate((s, a) => s + a) + " |";
        }).ToList();
    }

    public static int GetTextWidth(string value)
    {
        if (value == null)
            return 0;

        var length = value.ToCharArray().Sum(c => c > 255 ? 2 : 1); // previously 127
        return length;
    }

    public string ToMarkDownString()
    {
        return ToMarkDownString('|');
    }

    private string ToMarkDownString(char delimiter)
    {
        var builder = new StringBuilder();

        // find the longest column by searching each row
        var columnLengths = ColumnLengths();

        // create the string format with padding
        _ = Format(columnLengths, delimiter);

        // find the longest formatted line
        var columnHeaders = string.Format(Formats[0].TrimStart(), Columns.ToArray());

        // add each row
        var results = Rows.Select((row, i) => string.Format(Formats[i + 1].TrimStart(), row)).ToList();

        // create the divider
        var divider = Regex.Replace(columnHeaders, "[^|]", "-");

        builder.AppendLine(columnHeaders);
        builder.AppendLine(divider);
        results.ForEach(row => builder.AppendLine(row));

        return builder.ToString();
    }

    public string ToMinimalString()
    {
        return ToMarkDownString(char.MinValue);
    }

    public string ToStringAlternative()
    {
        var builder = new StringBuilder();

        // find the longest formatted line
        var columnHeaders = string.Format(Formats[0].TrimStart(), Columns.ToArray());

        // add each row
        var results = Rows.Select((row, i) => string.Format(Formats[i + 1].TrimStart(), row)).ToList();

        // create the divider
        var divider = Regex.Replace(columnHeaders, "[^| ]", "-");
        var dividerPlus = divider.Replace("|", "+").Replace(" ", "-");

        builder.AppendLine(dividerPlus);
        builder.AppendLine(columnHeaders);

        foreach (var row in results)
        {
            builder.AppendLine(dividerPlus);
            builder.AppendLine(row);
        }
        builder.AppendLine(dividerPlus);

        return builder.ToString();
    }

    public string WriteMultipleStringAlternative()
    {
        // find the longest formatted line
        var columnHeaders = string.Format(Formats[0].TrimStart(), Columns.ToArray());
        
        // create the divider
        var divider = Regex.Replace(columnHeaders, "[^| ]", "-");
        var dividerPlus = divider.Replace("|", "+").Replace(" ", "-");
        
        ExtendedConsole.WriteLine($"{dividerPlus}");
        ExtendedConsole.WriteLine($"{columnHeaders}");
        
        // Print each row
        // Attempt 2
        var columnLengths = ColumnLengths();
        var columnAlignment = Enumerable.Range(0, Columns.Count).Select(GetNumberAlignment).ToList();
        //  TODO  Add support for separate colors for each column/row
        // (Done) Add value based coloring, eg. Func<value, ConsoleColor>
        var rowIndex = 0;
        foreach (var row in Rows) // here allLines == Rows
        {
            ExtendedConsole.WriteLine($"{dividerPlus}");
            foreach (var col in row.Select((x, i) => new Col(Value: x?.ToString(), Index: i, RowIndex: rowIndex)))
            {
                var length = columnLengths[col.Index] - (GetTextWidth(col.Value) - col.Value.Length);
                // columnAlignment[col.Index] is either "" or "-"
                // 0 used to be col.Index, but now frozen as we format the string each time
                var t = "{" + 0 + "," + columnAlignment[col.Index] + length + "}"; 
                var formattedValue = string.Format(t, col.Value);

                WriteColoredColumn(col, formattedValue);
            }
            ExtendedConsole.WriteLine($"|");
            rowIndex++;
        }
        
        ExtendedConsole.WriteLine($"{dividerPlus}");

        return string.Empty;
    }

    private void WriteColoredColumn(Col col, string formattedValue)
    {
        switch (Options.ColumnColors.ToArray()[col.Index](col.Value))
        {
            case ConsoleColor.Black:
                ExtendedConsole.Write($"| {formattedValue:black} ");
                break;
            case ConsoleColor.DarkBlue:
                ExtendedConsole.Write($"| {formattedValue:darkblue} ");
                break;
            case ConsoleColor.DarkGreen:
                ExtendedConsole.Write($"| {formattedValue:darkgreen} ");
                break;
            case ConsoleColor.DarkCyan:
                ExtendedConsole.Write($"| {formattedValue:darkcyan} ");
                break;
            case ConsoleColor.DarkRed:
                ExtendedConsole.Write($"| {formattedValue:darkred} ");
                break;
            case ConsoleColor.DarkMagenta:
                ExtendedConsole.Write($"| {formattedValue:darkmagenta} ");
                break;
            case ConsoleColor.DarkYellow:
                ExtendedConsole.Write($"| {formattedValue:darkyellow} ");
                break;
            case ConsoleColor.Gray:
                ExtendedConsole.Write($"| {formattedValue:gray} ");
                break;
            case ConsoleColor.DarkGray:
                ExtendedConsole.Write($"| {formattedValue:darkgray} ");
                break;
            case ConsoleColor.Blue:
                ExtendedConsole.Write($"| {formattedValue:blue} ");
                break;
            case ConsoleColor.Green:
                ExtendedConsole.Write($"| {formattedValue:green} ");
                break;
            case ConsoleColor.Cyan:
                ExtendedConsole.Write($"| {formattedValue:cyan} ");
                break;
            case ConsoleColor.Red:
                ExtendedConsole.Write($"| {formattedValue:red} ");
                break;
            case ConsoleColor.Magenta:
                ExtendedConsole.Write($"| {formattedValue:magenta} ");
                break;
            case ConsoleColor.Yellow:
                ExtendedConsole.Write($"| {formattedValue:yellow} ");
                break;
            case ConsoleColor.White:
                ExtendedConsole.Write($"| {formattedValue:white} ");
                break;
            default:
                ExtendedConsole.Write($"| {formattedValue:white} ");
                break;
        }
    }


    private string Format(List<int> columnLengths, char delimiter = '|')
    {
        // set right alignment if is a number
        var columnAlignment = Enumerable.Range(0, Columns.Count)
            .Select(GetNumberAlignment)
            .ToList();

        SetFormats(columnLengths, columnAlignment);

        var delimiterStr = delimiter == char.MinValue ? string.Empty : delimiter.ToString();
        var format = (Enumerable.Range(0, Columns.Count)
            .Select(i => " " + delimiterStr + " {" + i + "," + columnAlignment[i] + columnLengths[i] + "}")
            .Aggregate((s, a) => s + a) + " " + delimiterStr).Trim();
        return format;
    }

    private string GetNumberAlignment(int i)
    {
        return Options.NumberAlignment == Alignment.Right
               && ColumnTypes != null
               && NumericTypes.Contains(ColumnTypes[i])
            ? ""
            : "-";
    }

    private List<int> ColumnLengths()
    {
        var columnLengths = Columns
            .Select((t, i) => Rows.Select(x => x[i])
                .Union(new[] { Columns[i] })
                .Where(x => x != null)
                .Select(x => x.ToString().ToCharArray().Sum(c => c > 255 ? 2 : 1)).Max())
            .ToList();
        return columnLengths;
    }

    public void Write(Format format = v1.Format.Default)
    {
        SetFormats(ColumnLengths(), Enumerable.Range(0, Columns.Count).Select(GetNumberAlignment).ToList());

        ExtendedConsole.WriteLine($"{CreateString(format)}");
    }

    private string CreateString(Format format = v1.Format.Default)
    {
        return format switch
        {
            v1.Format.Default => ToString(),
            v1.Format.MarkDown => ToMarkDownString(),
            v1.Format.Alternative => ToStringAlternative(),
            v1.Format.Minimal => ToMinimalString(),
            v1.Format.Color => WriteMultipleStringAlternative(),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }

    /// <summary>
    /// Use '[', ']' before and after message to be written in the specified <paramref name="color"/>.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="color"></param>
    private void WriteColor(string message, ConsoleColor color)
    {
        var pieces = Regex.Split(message, @"(\[[^\]]*\])");

        for (int i = 0; i < pieces.Length; i++)
        {
            string piece = pieces[i];

            if (piece.StartsWith("[") && piece.EndsWith("]"))
            {
                Console.ForegroundColor = color;
                piece = piece.Substring(1, piece.Length - 2);
            }

            Options.OutputTo.Write(piece);
            Console.ResetColor();
        }

        Console.WriteLine();
    }

    private static IEnumerable<string> GetColumns<T>()
    {
        return typeof(T).GetProperties().Select(x => x.Name).ToArray();
    }

    private static object GetColumnValue<T>(object target, string column)
    {
        return typeof(T).GetProperty(column)?.GetValue(target, null);
    }

    private static IEnumerable<Type> GetColumnsType<T>()
    {
        return typeof(T).GetProperties().Select(x => x.PropertyType).ToArray();
    }
}

public enum Format
{
    Default = 0,
    MarkDown = 1,
    Alternative = 2,
    Minimal = 3,
    Color = 4
}

public enum Alignment
{
    Left,
    Right
}
