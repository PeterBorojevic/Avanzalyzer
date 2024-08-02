namespace Core.Common.ConsoleTables.v1;

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
            ? val >= value
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
