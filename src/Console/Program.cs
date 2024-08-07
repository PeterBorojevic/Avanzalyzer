using Application;
using Console;
using Console.Interfaces;
using Console.Menu;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static void Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddRepositories()
            .AddFeatures()
            .AddMenues()
            .BuildServiceProvider();
        
        var menu = serviceProvider.GetService<IStartMenu>();

        _ = menu.Run();
    }
}


/*  TODO
 * (doing) Menu service to access different features
 * Add user action flow between menus (menus in menus)
 * or state of what is visible
 * Add interfaces and abstract models for menus
 * Add storing settings, eg. exclude accounts
 *
 *
 * Add features
 * Calculations
 * Dividend yield
 *
 * Totals
 * Forecasted savings
 * Fetch ticker stock prices
 *
 * Backend
 * SQL server for storage and memory
 * Caching
 *
 */