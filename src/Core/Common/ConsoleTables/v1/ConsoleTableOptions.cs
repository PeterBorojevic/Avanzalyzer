using Core.Common.ConsoleTables.v2;

namespace Core.Common.ConsoleTables.v1;

public class ConsoleTableOptions
{
    public IEnumerable<string> Columns { get; set; } = new List<string>();
    public IEnumerable<Func<Cell, ConsoleColor>> ColumnColors { get; set; } = new List<Func<Cell, ConsoleColor>>();
    public bool EnableCount { get; set; } = true;

    /// <summary>
    /// Enable only from a list of objects
    /// </summary>
    public Alignment NumberAlignment { get; set; } = Alignment.Left;

    /// <summary>
    /// The <see cref="TextWriter"/> to write to. Defaults to <see cref="Console.Out"/>.
    /// </summary>
    public TextWriter OutputTo { get; set; } = Console.Out;
}

/// <summary>
/// Contain column headers and column settings. Such as functions to decide the cells color
/// </summary>
public class ColumnInColor
{
    public Cell Value { get; set; }
    public ConsoleColor Color => _colorFunc.Invoke(Value);
    private Func<Cell, ConsoleColor> _colorFunc;

    public ColumnInColor(string value, ConsoleColorFunctions colorPresets = ConsoleColorFunctions.Normal)
    {
        Value = new Cell(value, 0);
        _colorFunc = colorPresets switch
        {
            ConsoleColorFunctions.Normal => ColorFunctions.Normal,
            ConsoleColorFunctions.PositiveOrNegative => ColorFunctions.PositiveGreen_NegativeRed,
            ConsoleColorFunctions.Percentage => ColorFunctions.PercentagePositiveBlue_NegativeRed,
            _ => throw new ArgumentOutOfRangeException(nameof(colorPresets), colorPresets, null)
        };
    }

    public ColumnInColor(string value, Func<Cell, ConsoleColor> colorFunc)
    {
        Value = new Cell(value, 0);
        _colorFunc = colorFunc;
    }

    public void SetColor(Func<Cell, ConsoleColor> colorFunc)
    {
        _colorFunc = colorFunc;
    }

    public Func<Cell, ConsoleColor> GetColorFunction() => _colorFunc;

}

public static class ColumnInColorExtensions
{
    public static IEnumerable<string> Values(this IEnumerable<ColumnInColor> columns)
    {
        return columns.Select(c => c.Value.Value);
    }

    public static IEnumerable<ConsoleColor> Colors(this IEnumerable<ColumnInColor> columns)
    {
        return columns.Select(c => c.Color);
    }

    public static IEnumerable<Func<Cell, ConsoleColor>> ColorFunctions(this IEnumerable<ColumnInColor> columns)
    {
        return columns.Select(c => c.GetColorFunction());
    }
}

public enum ConsoleColorFunctions
{
    Normal,
    PositiveOrNegative,
    Percentage
}

public static class SpecificationExtensions
{
    public static Func<string, ConsoleColor> And(this Func<string, ConsoleColor> function, Func<string, ConsoleColor> convertDefaultTo)
    {
        return input => function.Invoke(input) is not ConsoleColor.White ? function.Invoke(input) : convertDefaultTo.Invoke(input);
    }
    public static Func<Cell, ConsoleColor> And(this Func<Cell, ConsoleColor> function, Func<Cell, ConsoleColor> convertDefaultTo)
    {
        return input => function.Invoke(input) is not ConsoleColor.White ? function.Invoke(input) : convertDefaultTo.Invoke(input);
    }
}

public static class ColorFunctions
{
    public static Func<Cell, ConsoleColor> Normal => _ => ConsoleColor.White;
    
    public static Func<Cell, ConsoleColor> PositiveGreen_NegativeRed => column
        => decimal.TryParse(column.Value, out var val)
            ? val > 0
                ? ConsoleColor.Green
                : ConsoleColor.Red
            : ConsoleColor.White;

    public static Func<Cell, ConsoleColor> PercentagePositiveBlue_NegativeRed => column
        => decimal.TryParse(column.Value.Split(" ")[0], out var val)
            ? val > 0
                ? ConsoleColor.Cyan
                : ConsoleColor.Red
            : ConsoleColor.White;

    public static Func<Cell, ConsoleColor> ValuesAbove(decimal value, ConsoleColor color) => column
        => decimal.TryParse(column.Value, out var val)
            ? val > value
                ? color
                : ConsoleColor.White
            : ConsoleColor.White;
    public static Func<Cell, ConsoleColor> ValuesBelow(decimal value, ConsoleColor color) => column
        => decimal.TryParse(column.Value, out var val)
            ? val < value
                ? color
                : ConsoleColor.White
            : ConsoleColor.White;


    //public static Func<Cell, ConsoleColor> Function1 => input =>
    //public static Func<Cell, ConsoleColor> Function2 => input =>
    //public static Func<Cell, ConsoleColor> Function3 => input => 
}