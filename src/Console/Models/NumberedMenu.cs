using System.Text;

namespace Console.Models;

public class NumberedMenu
{
    public string Header { get; set; }
    public List<MenuItem> Items { get; set; }

    public NumberedMenu(string header, params MenuItem[] menuItems)
    {
        Header = header;
        Items = menuItems.ToList();
    }

    public NumberedMenu(List<MenuItem> menuItems)
    {
        Items = menuItems;
    }

    public void Display(bool shouldClear = true)
    {
        if (shouldClear) System.Console.Clear();
        System.Console.WriteLine(Header);
        var sb = new StringBuilder();
        foreach (var menuItem in Items)
        {
            sb.Append(menuItem.Id);
            sb.Append(" - ");
            sb.Append(menuItem.Text);
            sb.Append(" \n");
        }
        System.Console.WriteLine(sb.ToString());
    }

    public MenuItem GetUserSelectedChoice()
    {
        var key = System.Console.ReadKey(true);
        
        return Items.FirstOrDefault(menuItem => menuItem.WasClicked(key.Key)) ?? TryAgain();
    }

    public MenuItem TryAgain()
    {
        System.Console.WriteLine("Input must be a number from the menu");
        return GetUserSelectedChoice();
    }

    /// <summary>
    /// Gets the user selected menu item and executes its on-click function.
    /// </summary>
    /// <returns>true if the user want to continue, otherwise false.</returns>
    public bool GotoUserSelection()
    {
        var selectedMenu = GetUserSelectedChoice();
        System.Console.Clear();
        return selectedMenu.OnClick.Invoke();
    }
}
