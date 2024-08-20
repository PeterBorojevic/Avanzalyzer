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
    public HashSet<string> TradedAssets { get; } = new();

    public InvestmentPortfolio(bool verbose = false)
    {
        _verbose = verbose;
    }

    public void UpdateBalance(Transaction transaction)
    {
        _accountBalance[transaction.AccountName] += transaction.Amount;
        LogBalance(transaction);
    }

    public void AddTradedAsset(string assetName) => TradedAssets.Add(assetName);

    public void AddBuy(Transaction transaction)
    {
        AddAccountIfNotSeen(transaction.AccountName);
        var existingAsset = GetExistingAssetOrDefault(transaction);

        if (existingAsset != null)
        {
            if (_verbose) ExtendedConsole.Write($"Buy {transaction.Quantity.ToString("##")} {transaction.AssetNameOrDescription:yellow} for {transaction.Amount.ToString("C"):red}");
            existingAsset.AvgAcquisitionCost = CalculateNewAverageAcquisitionCost(existingAsset, transaction);
            existingAsset.Quantity += transaction.Quantity;
            existingAsset.MarketValue = transaction.Price * existingAsset.Quantity;
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

    public void AddAsset(Transaction transaction)
    {
        AddAccountIfNotSeen(transaction.AccountName);
        var existingAsset = GetExistingAssetOrDefault(transaction);
        if (existingAsset != null)
        {
            if (_verbose) ExtendedConsole.Write($"Add {transaction.Quantity.ToString("##"):white} {transaction.AssetNameOrDescription:yellow} for {transaction.Amount.ToString("C"):red}");
            existingAsset.AvgAcquisitionCost = CalculateNewAverageAcquisitionCost(existingAsset, transaction);
            existingAsset.Quantity += transaction.Quantity;
            existingAsset.MarketValue = transaction.Price * existingAsset.Quantity;
        }
        else
        {
            if (_verbose) ExtendedConsole.Write($"Add {transaction.Quantity.ToString("##"):white} {transaction.AssetNameOrDescription:yellow} for {transaction.Amount.ToString("C"):red}");
            // Can't tell by the transaction what type of asset was added
            var newAsset = new Investment
            {
                AccountNumber = transaction.AccountName,
                ISIN = transaction.ISIN,
                Currency = transaction.Currency,
                Type = AssetType.UnknownAsset,
                Name = transaction.AssetNameOrDescription,
                Quantity = transaction.Quantity,
                MarketValue = transaction.Quantity * transaction.Price,
                AvgAcquisitionCost = transaction.Quantity * transaction.Price,
            };
            _accountHoldings[transaction.AccountName].Add(newAsset);
        }
        UpdateBalance(transaction);
    }

    public void RemoveAsset(Transaction transaction)
    {
        AddAccountIfNotSeen(transaction.AccountName);
        var existingAsset = GetExistingAssetOrDefault(transaction);
        if (_verbose) ExtendedConsole.Write($"Remove {transaction.Quantity.ToString("##"):white} {transaction.AssetNameOrDescription:yellow} for {transaction.Amount.ToString("C"):green}");

        UpdateBalance(transaction);
        if (existingAsset is null) return;

        existingAsset.Quantity += transaction.Quantity;

        if (existingAsset.Quantity == 0)
        {
            _accountHoldings[transaction.AccountName].Remove(existingAsset);
        }
    }

    private static decimal CalculateNewAverageAcquisitionCost(Asset existingAsset, Transaction transaction)
    {
        var totalCost = (existingAsset.Quantity * existingAsset.AvgAcquisitionCost) +
                        (Math.Abs(transaction.Amount) + transaction.BrokerageFee);
        var totalQuantity = existingAsset.Quantity + transaction.Quantity;
        return totalCost / totalQuantity;
    }


    public void AddSell(Transaction transaction)
    {
        AddAccountIfNotSeen(transaction.AccountName);

        var existingAsset = GetExistingAssetOrDefault(transaction);
        if (_verbose) ExtendedConsole.Write($"Sell {transaction.Quantity.ToString("##")} {transaction.AssetNameOrDescription:yellow} for {transaction.Amount.ToString("C"):green}");

        UpdateBalance(transaction);
        if (existingAsset == null) return;

        existingAsset.Quantity += transaction.Quantity;

        if (existingAsset.Quantity == 0)
        {
            _accountHoldings[transaction.AccountName].Remove(existingAsset);
        }
    }
    

    private void LogBalance(Transaction transaction)
    {
        if (!_verbose) return;
        ExtendedConsole.Write($"\n{$"{" > Account balance",18}"} {$"{transaction.AccountName,38}":white}: {$"{_accountBalance[transaction.AccountName],14:C}":green} \n");
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
        _dividends[transaction.AccountName].Add(transaction);
        if (_verbose)
        {
            ExtendedConsole.Write(transaction.Amount > 0
                ? (ConsoleInterpolatedStringHandler)$"Dividend {transaction.Amount.ToString("C"):green}"
                : (ConsoleInterpolatedStringHandler)$"Dividend tax {transaction.Amount.ToString("C"):red}");
        }
        UpdateBalance(transaction);
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
