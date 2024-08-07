namespace Console.Models;

public class UserAction
{
    public ConsoleKey KeyPressed { get; set; } = ConsoleKey.None;
    public bool ShouldContinue { get; set; } = true;
}
