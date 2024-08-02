namespace Core.Common.ConsoleTables.v1;

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
