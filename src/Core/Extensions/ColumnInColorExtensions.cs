using Core.Common.ConsoleTables.v1;

namespace Core.Extensions;

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
