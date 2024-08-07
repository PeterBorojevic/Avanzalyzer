using Console.Models;

namespace Console.Menu;

public interface IMenu
{
    UserAction Next();

    /// <summary>
    /// Presents the menu to the console
    /// </summary>
    /// <returns>true to continue, otherwise false</returns>
    bool Run();
}
