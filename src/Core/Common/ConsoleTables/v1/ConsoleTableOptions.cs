namespace Core.Common.ConsoleTables.v1;

public class ConsoleTableOptions
{
    public IEnumerable<string> Columns { get; set; } = new List<string>();
    public IEnumerable<Func<string, ConsoleColor>> ColumnColors { get; set; } = new List<Func<string, ConsoleColor>>();
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

public class ColumnInColor
{
    public string Value { get; set; }
    public ConsoleColor Color => _colorFunc.Invoke(Value);
    private Func<string, ConsoleColor> _colorFunc;

    public ColumnInColor(string value, ConsoleColorFunctions colorPresets = ConsoleColorFunctions.Normal)
    {
        Value = value;
        _colorFunc = colorPresets switch
        {
            ConsoleColorFunctions.Normal => ColorFunctions.Normal,
            ConsoleColorFunctions.PositiveOrNegative => ColorFunctions.PositiveGreen_NegativeRed,
            ConsoleColorFunctions.Percentage => ColorFunctions.PercentagePositiveBlue_NegativeRed,
            _ => throw new ArgumentOutOfRangeException(nameof(colorPresets), colorPresets, null)
        };
    }

    public ColumnInColor(string value, Func<string, ConsoleColor> colorFunc)
    {
        Value = value;
        _colorFunc = colorFunc;
    }

    public void SetColor(Func<string, ConsoleColor> colorFunc)
    {
        _colorFunc = colorFunc;
    }

    public Func<string, ConsoleColor> GetColorFunction() => _colorFunc;

}

public static class ColumnInColorExtensions
{
    public static IEnumerable<string> Values(this IEnumerable<ColumnInColor> columns)
    {
        return columns.Select(c => c.Value);
    }

    public static IEnumerable<ConsoleColor> Colors(this IEnumerable<ColumnInColor> columns)
    {
        return columns.Select(c => c.Color);
    }

    public static IEnumerable<Func<string, ConsoleColor>> ColorFunctions(this IEnumerable<ColumnInColor> columns)
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
}

public static class ColorFunctions
{
    public static Func<string, ConsoleColor> Normal => _ => ConsoleColor.White;

    /// <summary>
    /// Checks the decimal if it's positive (green) or negative (red). Other values are white.
    /// </summary>
    public static Func<string, ConsoleColor> PositiveGreen_NegativeRed => input
        => decimal.TryParse(input, out var val)
            ? val > 0
                ? ConsoleColor.Green
                : ConsoleColor.Red
            : ConsoleColor.White;

    public static Func<string, ConsoleColor> PercentagePositiveBlue_NegativeRed => input
        => decimal.TryParse(input.Split(" ")[0], out var val)
            ? val > 0
                ? ConsoleColor.Cyan
                : ConsoleColor.Red
            : ConsoleColor.White;

    public static Func<string, ConsoleColor> ValuesAbove(decimal value, ConsoleColor color) => input
        => decimal.TryParse(input, out var val)
            ? val > value
                ? color
                : ConsoleColor.White
            : ConsoleColor.White;
    public static Func<string, ConsoleColor> ValuesBelow(decimal value, ConsoleColor color) => input
        => decimal.TryParse(input, out var val)
            ? val < value
                ? color
                : ConsoleColor.White
            : ConsoleColor.White;
    //public static Func<string, ConsoleColor> Function1 => input =>
    //public static Func<string, ConsoleColor> Function2 => input =>
    //public static Func<string, ConsoleColor> Function3 => input => 
}