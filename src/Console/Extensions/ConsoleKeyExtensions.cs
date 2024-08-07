namespace Console.Extensions;

public static class ConsoleKeyExtensions
{
    public static int ToInteger(this ConsoleKey key)
    {
        return key switch
        {
            ConsoleKey.None => 0,
            ConsoleKey.D0 => 0,
            ConsoleKey.NumPad0 => 0,
            ConsoleKey.D1 => 1,
            ConsoleKey.NumPad1 => 1,
            ConsoleKey.D2 => 2,
            ConsoleKey.NumPad2 => 2,
            ConsoleKey.D3 => 3,
            ConsoleKey.NumPad3 => 3,
            ConsoleKey.D4 => 4,
            ConsoleKey.NumPad4 => 4,
            ConsoleKey.D5 => 5,
            ConsoleKey.NumPad5 => 5,
            ConsoleKey.D6 => 6,
            ConsoleKey.NumPad6 => 2,
            ConsoleKey.D7 => 7,
            ConsoleKey.NumPad7 => 7,
            ConsoleKey.D8 => 8,
            ConsoleKey.NumPad8 => 8,
            ConsoleKey.D9 => 9,
            ConsoleKey.NumPad9 => 9,
            _ => 0
        };
    }
}
