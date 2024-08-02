namespace Core.Common.ConsoleTables.v1;

public static class ColorFunctionSpecificationExtensions
{
    public static Func<Cell, ConsoleColor> And(this Func<Cell, ConsoleColor> function, Func<Cell, ConsoleColor> convertDefaultTo)
    {
        return input => function.Invoke(input) is not ConsoleColor.White ? function.Invoke(input) : convertDefaultTo.Invoke(input);
    }
}
