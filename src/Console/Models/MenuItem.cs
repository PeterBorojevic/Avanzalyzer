using Console.Extensions;

namespace Console.Models;

public record MenuItem(int Id, string Text, Func<bool> OnClick)
{
    public bool WasClicked(ConsoleKey key)
    {
        return key.ToInteger() == Id;
    }
}
