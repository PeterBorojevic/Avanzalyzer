namespace Console.Menu.Base;

public abstract class Menu
{
    internal static bool PressAnyKeyToReturn
    {
        get
        {
            System.Console.WriteLine("\n Click any key to return");
            ReadKey();
            return true;
        }
    }

    internal static ConsoleKeyInfo ReadKey() => System.Console.ReadKey(true);

    internal bool Return() => false;
}
