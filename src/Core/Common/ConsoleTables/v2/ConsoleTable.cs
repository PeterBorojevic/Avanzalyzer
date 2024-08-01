namespace Core.Common.ConsoleTables.v2;

public class ConsoleTable
{
    
    public ConsoleTableOptions Options { get; set; }

    public ConsoleTable(params Column[] headerColumns) : this(new ConsoleTableOptions(){HeaderColumns = headerColumns})
    {
    }

    public List<string[]> Columns { get; set; }

    public List<string[]> Rows { get; set; }

    public ConsoleTable(ConsoleTableOptions options)
    {
        Options = options;
        Rows = new List<string[]>();
        //Columns = new List<string[]>() { options.HeaderColumns.Select(h => h.Value) };
    }

    public void Write()
    {
        // Writes each line of the table to the console.
        // Should be able to print each column in a desired color

        //var columnWidth = Columns
        //    .Select((t, i) => Rows.Select(x => x[i])
        //        .Union(new[] { Columns[i] })
        //        .Where(x => x != null)
        //        .Select(x => x.ToString().ToCharArray().Sum(c => c > 255 ? 2 : 1)).Max())
        //    .ToList();
        //return columnLengths;
    }

    private void SetFormats()
    {

    }
}


public class ConsoleTableOptions
{
    public Column[] HeaderColumns { get; set; }
}

public class Column
{
    public Column(string value, ConsoleColor color = ConsoleColor.White)
    {
        Value = value;
        Color = color;
    }

    public string Value { get; set; }
    public ConsoleColor Color { get; set; }
}