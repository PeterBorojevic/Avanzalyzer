using Console.Menu;
using Console.Models;

namespace Console.Extensions;

public static class MenuItemExtensions
{
    public static void Goto(this IEnumerable<MenuItem> menuItems, ConsoleKeyInfo key)
    {
        var clickedMenu = menuItems.First(menuItem => menuItem.WasClicked(key.Key));
        System.Console.Clear();
        clickedMenu.OnClick.Invoke();
    }
    public static void Print(this IEnumerable<MenuItem> menuItems)
    {
        System.Console.WriteLine();
        foreach (var menuItem in menuItems)
        {
            System.Console.Write(menuItem.Id);
            System.Console.Write(" ");
            System.Console.Write(menuItem.Text);
            System.Console.Write(" \n");
        }
    }
}
