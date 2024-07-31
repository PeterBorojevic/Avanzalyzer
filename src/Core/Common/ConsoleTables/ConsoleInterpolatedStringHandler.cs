using System.Runtime.CompilerServices;

namespace Core.Common.ConsoleTables;

[InterpolatedStringHandler]
public ref struct ConsoleInterpolatedStringHandler
{
    private static readonly Dictionary<string, ConsoleColor> colors;
    private readonly IList<Action> actions;

    static ConsoleInterpolatedStringHandler() =>
        colors = Enum.GetValues<ConsoleColor>().ToDictionary(x => x.ToString().ToLowerInvariant(), x => x);

    public ConsoleInterpolatedStringHandler(int literalLength, int formattedCount)
    {
        actions = new List<Action>();
    }

    public void AppendLiteral(string s)
    {
        actions.Add(() => Console.Write(s));
    }

    public void AppendFormatted<T>(T t)
    {
        actions.Add(() => Console.Write(t));
    }

    public void AppendFormatted<T>(T t, string format)
    {
        if (!colors.TryGetValue(format, out var color))
            throw new InvalidOperationException($"Color '{format}' not supported");

        actions.Add(() =>
        {
            Console.ForegroundColor = color;
            Console.Write(t);
            Console.ResetColor();
        });
    }

    public void AppendFormatted<T>(T t, ConsoleColor color)
    {
        actions.Add(() =>
        {
            Console.ForegroundColor = color;
            Console.Write(t);
            Console.ResetColor();
        });
    }

    internal void WriteLine() => Write(true);
    internal void Write() => Write(false);

    private void Write(bool newLine)
    {
        foreach (var action in actions)
            action();

        if (newLine)
            Console.WriteLine();
    }
}

public static class ExtendedConsole
{
    public static void WriteLine(ConsoleInterpolatedStringHandler builder)
    {
        builder.WriteLine();
    }

    public static void Write(ConsoleInterpolatedStringHandler builder)
    {
        builder.Write();
    }
}
