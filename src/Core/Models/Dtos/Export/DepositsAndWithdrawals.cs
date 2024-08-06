using Core.Common.ConsoleTables;
using Core.Common.ConsoleTables.v1;
using Core.Common.Enums;
using Core.Common.Interfaces;
using Core.Models.Data;

namespace Core.Models.Dtos.Export;

public class DepositsAndWithdrawals : IPrintable
{
    private readonly IEnumerable<Transaction> _depositsAndWithdrawals;
    private readonly List<ColumnInColor> _columns = new()
    {
        new ColumnInColor("Date", _ => ConsoleColor.Yellow),
        new ColumnInColor("Deposits [kr]", ColorFunctions.PositiveGreen_NegativeRed),
        new ColumnInColor("Withdrawals [kr]", ColorFunctions.PositiveGreen_NegativeRed),
        new ColumnInColor("Total [kr]", ColorFunctions.PositiveGreen_NegativeRed),
    };
    public DepositsAndWithdrawals(IEnumerable<Transaction> transactions)
    {
        _depositsAndWithdrawals = transactions;
        // TODO add filter for account(s)
    }

    public void PrintToConsole()
    {
        var table = new ConsoleTable(_columns);
        foreach (var depositsAndWithdrawals in _depositsAndWithdrawals.GroupBy(x => new DateTime(x.Date.Year, 1, 1)))
        {
            table.AddRow(
                $"{depositsAndWithdrawals.Key:yyyy}",
                $"{depositsAndWithdrawals.Where(x => x.TransactionType is TransactionType.Deposit).Sum(x => x.Amount)}",
                $"{depositsAndWithdrawals.Where(x => x.TransactionType is TransactionType.Withdraw).Sum(x => x.Amount)}",
                $"{depositsAndWithdrawals.Sum(x => x.Amount)}");
        }
        table.Write(Format.Color);

        ExtendedConsole.WriteLine($"Total deposits or withdrawals: {_depositsAndWithdrawals.Sum(x => x.Amount).ToString("C"):green} \n");
    }
}
