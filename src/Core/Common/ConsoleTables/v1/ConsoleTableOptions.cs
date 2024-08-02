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
