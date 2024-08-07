﻿using Application;
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
            .AddSingleton<StartMenu>()
            .BuildServiceProvider();
        
        var menu = serviceProvider.GetService<StartMenu>();

        menu.Run();
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
 * Totals
 * Forecasted savings
 * Fetch ticker stock prices
 *
 * Backend
 * SQL server for storage and memory
 * Caching
 *
 */