using Core.Common.Enums;
using Core.Models.Data;

namespace Core.Extensions;

public static class TransactionExtensions
{
    public static IEnumerable<Transaction>? SelectDividendRelatedTransactions(this IEnumerable<Transaction>? transactions)
    {
        return transactions?.Where(t =>
            t.TransactionType is
                TransactionType.Dividend or
                TransactionType.DividendProvisionalTax or
                TransactionType.ForeignTax);
    }
}
