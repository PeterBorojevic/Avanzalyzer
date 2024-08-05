using Core.Common.ConsoleTables.v1;
using Core.Extensions;

namespace Core.Common.ConsoleTables.v2;

public class ColoredConsoleTable : ConsoleTable
{
    public ColoredConsoleTable()
    {
        var columnInColors = new List<ColumnInColor>()
        {
            new("Namn"),
            new("Värde", ColorFunctions.ValuesAbove(1000, ConsoleColor.Blue)
                .And(ColorFunctions.ValuesBelow(1001, ConsoleColor.DarkBlue))),
            new("Sedan köp [kr]", ConsoleColorFunctions.PositiveOrNegative),
            new("Sedan köp [%]", ConsoleColorFunctions.Percentage),
        };
        var table = new ConsoleTable(new ConsoleTableOptions()
        {
            Columns = columnInColors.Values(),
            ColumnColors = columnInColors.ColorFunctions(),
            NumberAlignment = Alignment.Right
        });
    }
}

