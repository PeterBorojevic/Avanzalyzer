using Core.Common.Enums;
using Core.Common.Interfaces;
using Core.Models;
using Core.Models.Dtos.Export;

namespace Application.Features;

public class Calculator
{
    public IPrintable Get(CalculationType type, Portfolio portfolio)
    {
        return type switch
        {
            CalculationType.AccountTotals => new AccountTotals(portfolio),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
