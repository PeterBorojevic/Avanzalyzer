using Core.Common.ConsoleTables;
using Core.Common.Enums;
using Core.Models.Securities;
using Core.Models.Securities.Base;
using System.Text;

namespace Core.Models.Data;

public class InvestmentPortfolio
{
    private readonly Dictionary<string, List<Asset>> _accountHoldings = new();
    private readonly AccountBalance _accountBalance = new();
    private readonly Dictionary<string, Dividends> _dividends = new();
    private readonly bool _verbose;

    public InvestmentPortfolio(bool verbose = false)
    {
        _verbose = verbose;
    }
    
    public void AddBuy(Transaction transaction)
    {
        AddAccountIfNotSeen(transaction.AccountName);
        var existingAsset = GetExistingAssetOrDefault(transaction);

        if (existingAsset != null)
        {
            if (_verbose) ExtendedConsole.Write($"Buy {transaction.Quantity.ToString("##")} {transaction.AssetNameOrDescription:yellow} for {transaction.Amount.ToString("C"):red}");
            var assetValueAtPurchase = Math.Abs(transaction.Amount - transaction.BrokerageFee) / transaction.Quantity;
            existingAsset.AvgAcquisitionCost = CalculateNewAverageAcquisitionCost(existingAsset, transaction);
            existingAsset.Quantity += transaction.Quantity;
            existingAsset.MarketValue = assetValueAtPurchase * existingAsset.Quantity;
        }
        else
        {
            if (_verbose) ExtendedConsole.Write($"Buy {transaction.Quantity.ToString("##")} {transaction.AssetNameOrDescription:yellow} for {transaction.Amount.ToString("C"):red}");
            // Can't tell by the transaction what type of asset was bought
            var newAsset = new Investment
            {
                AccountNumber = transaction.AccountName,
                ISIN = transaction.ISIN,
                Currency = transaction.Currency,
                Type = AssetType.UnknownAsset,
                Name = transaction.AssetNameOrDescription,
                Quantity = transaction.Quantity,
                MarketValue = Math.Abs(transaction.Amount) - transaction.BrokerageFee,
                // Amount = quantity * market value at the time + brokerage fees
                // For purchases the amount is negative, for sells it's positive
                AvgAcquisitionCost = Math.Abs(transaction.Amount)/transaction.Quantity
            };
            _accountHoldings[transaction.AccountName].Add(newAsset);
        }
        UpdateBalance(transaction);
    }

    public void UpdateBalance(Transaction transaction)
    {
        _accountBalance[transaction.AccountName] += transaction.Amount;
        LogBalance(transaction);
    }

    public void AddAsset()
    {
        // TODO ignore BTA
    }

    private static decimal CalculateNewAverageAcquisitionCost(Asset existingAsset, Transaction transaction)
    {
        var totalCost = (existingAsset.Quantity * existingAsset.AvgAcquisitionCost) +
                        (Math.Abs(transaction.Amount) + transaction.BrokerageFee);
        var totalQuantity = existingAsset.Quantity + transaction.Quantity;
        return totalCost / totalQuantity;
    }


    public void AddSell(Transaction transaction, TransactionType transactionType = TransactionType.Sell)
    {
        if (!_accountHoldings.ContainsKey(transaction.AccountName)) return;

        var existingAsset = _accountHoldings[transaction.AccountName]
            .FirstOrDefault(a => a.ISIN == transaction.ISIN);
        if (_verbose) ExtendedConsole.Write($"Sell {transaction.Quantity.ToString("##")} {transaction.AssetNameOrDescription:yellow} for {transaction.Amount.ToString("C"):green}");

        UpdateBalance(transaction);
        if (existingAsset == null) return;

        existingAsset.Quantity += transaction.Quantity;

        if (existingAsset.Quantity == 0)
        {
            _accountHoldings[transaction.AccountName].Remove(existingAsset);
        }

        if (transaction.Amount is decimal.Zero || transactionType is TransactionType.AssetTransfer)
        {
            var valueOfTrade = transaction.Quantity * transaction.Price;
            //_accountBalance[transaction.AccountName] += valueOfTrade;
        }
    }

    private void LogBalance(Transaction transaction)
    {
        if (_verbose)
        {
            //ExtendedConsole.Write($"\t\t | Account balance {transaction.AccountName:yellow}: {_accountBalance[transaction.AccountName].ToString("C"):green} \n");
            ExtendedConsole.Write($"\n{$"{" > Account balance",18}"} {$"{transaction.AccountName,38}":white}: {$"{_accountBalance[transaction.AccountName],14:C}":green} \n");
        }
            
    }

    public void AddDepositOrWithdrawal(Transaction transaction)
    {
        AddAccountIfNotSeen(transaction.AccountName);
        if (_verbose)
        {
            if (transaction.Amount > 0) 
                ExtendedConsole.Write($"{"Deposit":cyan} {$"{transaction.AccountName,20}":yellow}:{$"{transaction.Amount:C}":green}");
            else 
                ExtendedConsole.Write($"{"Withdrawal":magenta} {$"{transaction.AccountName,20}":yellow}: {$"{transaction.Amount:C}":red}");
        }
        UpdateBalance(transaction);
    }

    public void AddDividend(Transaction transaction)
    {
        switch(transaction.TransactionType)
        {
            case TransactionType.Dividend:
                _dividends[transaction.AccountName].AddDividend(transaction.Date, transaction.Amount);
                    break;
            case TransactionType.ForeignTax:
                _dividends[transaction.AccountName].AddForeignTax(transaction.Date, transaction.Amount);
                break;
            case TransactionType.DividendProvisionalTax:
                _dividends[transaction.AccountName].AddProvisionalTax(transaction.Date, transaction.Amount);
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
            default: ExtendedConsole.WriteLine($"{"Unexpected dividend type, ignored":red}");
                break;
        };
        _accountBalance[transaction.AccountName ] += transaction.Amount;
        if (_verbose) ExtendedConsole.Write($"Dividend {transaction.Amount.ToString("C"):green}");
        LogBalance(transaction);
    }
    private void AddAccountIfNotSeen(string accountName)
    {
        if (!_accountBalance.Contains(accountName)) _accountBalance[accountName] = 0;
        if (!_accountHoldings.ContainsKey(accountName)) _accountHoldings[accountName] = new List<Asset>();
        if (!_dividends.ContainsKey(accountName)) _dividends[accountName] = new Dividends();
    }

    private Asset? GetExistingAssetOrDefault(Transaction transaction)
    {
        var existingAsset = _accountHoldings[transaction.AccountName]
            .FirstOrDefault(a => a.ISIN == transaction.ISIN);
        return existingAsset;
    }
}

public class AccountBalance
{
    private readonly Dictionary<string, decimal> _accountBalance = new();

    public decimal this[string accountName]
    {
        get => _accountBalance[accountName];
        set
        {
            if (_accountBalance.ContainsKey(accountName)) _accountBalance[accountName] = value;
            else
            {
                _accountBalance.Add(accountName, value);
            }
        }
    }

    public bool Contains(string accountName) => _accountBalance.ContainsKey(accountName);

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var kvp in _accountBalance)
        {
            sb.Append($"{kvp.Key}: {kvp.Value:C} \n");
        }
        return sb.ToString();
    }
}

public class Dividends
{
    private readonly Dictionary<DateTime, decimal> _dividends = new();
    private readonly Dictionary<DateTime, decimal> _provisionalTax = new();
    private readonly Dictionary<DateTime, decimal> _foreignTax = new();

    public void AddDividend(DateTime date, decimal amount) => this[date] = amount;

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
            if (_dividends.ContainsKey(date)) _dividends[date] += value;
            else
            {
                _dividends.Add(date, value);
            }
        }
    }

    public decimal TotalDividends => _dividends.Sum(kvp => kvp.Value);
    public decimal TotalProvisionalTax => _provisionalTax.Sum(kvp => kvp.Value);
    public decimal TotalForeignTax => _foreignTax.Sum(kvp => kvp.Value);

    public override string ToString()
    {
        if (TotalDividends == decimal.Zero) return "No dividends";
        var sb = new StringBuilder();
        sb.Append("Total dividends: ").Append(TotalDividends.ToString("C"));
        sb.Append(", Tax: ").Append(TotalProvisionalTax.ToString("C"));
        sb.Append(", Foreign tax: ").Append(TotalForeignTax.ToString("C"));
        return sb.ToString();
    }
}
