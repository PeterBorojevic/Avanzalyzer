namespace Console.Menu.Base;

public abstract class Menu
{
    public static bool PressAnyKeyToReturn
    {
        get
        {
            System.Console.WriteLine("\n Click any key to return");
            ReadKey();
            return true;
        }
    }

    private static ConsoleKeyInfo ReadKey() => System.Console.ReadKey(true);
}
