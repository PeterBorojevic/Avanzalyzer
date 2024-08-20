using Core.Common.ConsoleTables;
using Core.Common.Enums;
using System.Text;

namespace Core.Models.Data;
/* 
 * TODO Want to be able to extract dividends for a specific asset, eg. by ISIN or name, in order to calculate ROI
 *
 */
public class Dividends
{
    private readonly Dictionary<DateTime, decimal> _dividends = new();
    private readonly Dictionary<DateTime, decimal> _provisionalTax = new();
    private readonly Dictionary<DateTime, decimal> _foreignTax = new();
    private readonly Dictionary<string, decimal> _assetDividends = new();

    public void Add(Transaction transaction)
    {
        switch (transaction.TransactionType)
        {
            case TransactionType.Dividend:
                AddAssetDividend(transaction);
                AddDividend(transaction.Date, transaction.Amount);
                break;
            case TransactionType.ForeignTax:
                AddAssetDividend(transaction);
                AddForeignTax(transaction.Date, transaction.Amount);
                break;
            case TransactionType.DividendProvisionalTax:
                AddAssetDividend(transaction);
                AddProvisionalTax(transaction.Date, transaction.Amount);
                break;
            case TransactionType.Undefined:
            case TransactionType.Options:
            case TransactionType.Deposit:
            case TransactionType.Withdraw:
            case TransactionType.Buy:
            case TransactionType.Sell:
            case TransactionType.Interest:
            case TransactionType.ProvisionalTax:
            case TransactionType.AssetTransfer:
            case TransactionType.ShareLoanDisbursement:
            case TransactionType.Other:
            default:
                ExtendedConsole.WriteLine($"{"Unexpected dividend type, ignored":red}");
                break;
        };
    }

    public void AddAssetDividend(Transaction transaction)
    {
        if (!_assetDividends.ContainsKey(transaction.ISIN)) _assetDividends.Add(transaction.ISIN, transaction.Amount);
        else _assetDividends[transaction.ISIN] += transaction.Amount;
    }

    public void AddDividend(DateTime date, decimal amount)
    {
        if (!_dividends.ContainsKey(date))
        {
            _dividends.Add(date, amount);
        }
        else
            _dividends[date] += amount;
    }

    public void AddProvisionalTax(DateTime date, decimal amount)
    {
        if (!_provisionalTax.ContainsKey(date)) _provisionalTax.Add(date, amount);
        else _provisionalTax[date] += amount;
    }

    public void AddForeignTax(DateTime date, decimal amount)
    {
        if (!_foreignTax.ContainsKey(date)) _foreignTax.Add(date, amount);
        else _foreignTax[date] += amount;
    }

    public decimal this[DateTime date]
    {
        get => _dividends.ContainsKey(date) ? _dividends[date] : 0;
        set
        {
            if (!_dividends.ContainsKey(date)) _dividends.Add(date, value);
            else _dividends[date] = value;
        }
    }
    public decimal this[string ISIN]
    {
        get => _assetDividends.ContainsKey(ISIN) ? _assetDividends[ISIN] : 0;
        set
        {
            if (!_assetDividends.ContainsKey(ISIN)) _assetDividends.Add(ISIN, value);
            else _assetDividends[ISIN] = value;
        }
    }

    public decimal TotalDividends => _dividends.Sum(kvp => kvp.Value);
    public decimal TotalProvisionalTax => _provisionalTax.Sum(kvp => kvp.Value);
    public decimal TotalForeignTax => _foreignTax.Sum(kvp => kvp.Value);
    public decimal TotalDividendsAfterTax => TotalDividends + TotalProvisionalTax + TotalForeignTax;

    public override string ToString()
    {
        if (TotalDividends == decimal.Zero) return "No dividends";
        var sb = new StringBuilder();
        sb.Append("Total dividends: ").Append(TotalDividends.ToString("C"));
        sb.Append(", Tax: ").Append(TotalProvisionalTax.ToString("C"));
        sb.Append(", Foreign tax: ").Append(TotalForeignTax.ToString("C"));
        sb.Append(" | Net: ").Append(TotalDividendsAfterTax.ToString("C"));
        return sb.ToString();
    }
}
