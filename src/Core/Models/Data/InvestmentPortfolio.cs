using Core.Common.ConsoleTables;
using Core.Common.Enums;
using Core.Models.Securities;
using Core.Models.Securities.Base;
using System.Linq;
using System.Text;

namespace Core.Models.Data;
/* TODO
 * Wouldn't it be nice with some more refined, refactored, clean properties?
 *
 * This class is also getting big, a lot of responsibilities.
 * Can functions be extracted, letting the InvestmentPortfolio only hold data f
 */

public class InvestmentPortfolio
{
    private readonly Dictionary<string, List<Asset>> _accountHoldings = new();
    private readonly AccountBalance _accountBalance = new();
    private readonly Dictionary<string, Dividends> _accountDividends = new(); // TODO what if we want to fetch a specific assets dividend?
    private readonly bool _verbose;
    public HashSet<string> TradedAssets { get; } = new();
    public Dictionary<string, string> AssetNameToISIN { get; } = new();
    /// <summary>
    /// Maps an assets ISIN to realised profits.
    /// </summary>
    public Dictionary<string, decimal> AssetRealisedProfitOrLosses { get; } = new(); //TODO map ISIN to realised profits

    public InvestmentPortfolio(bool verbose = false)
    {
        _verbose = verbose;
    }

    public Asset? GetAsset(string isin)
    {
        return _accountHoldings
            .Select(accountHolding => accountHolding.Value.FirstOrDefault(asset => asset.ISIN == isin))
            .FirstOrDefault(holdings => holdings is not null);
    }

    public Dividends GetAccountDividends(string accountName) => _accountDividends.TryGetValue(accountName, out var dividends) ? dividends : new Dividends();
    public decimal GetAssetDividends(Asset asset) => _accountDividends.TryGetValue(asset.AccountNumber, out var dividends) ? dividends[asset.ISIN] : 0;

    /// <summary>
    /// Due to asset transfers, where an asset is removed from one account and then added to another. The asset name becomes "ÖVERFÖRING MELLAN EGNA KONTON".
    /// But this asset has been seen before, when bought. So the true name exists. This function corrects asset names of such assets using ISIN to match.
    /// </summary>
    public void CorrectAssetNames()
    {
        var isinToName =
            AssetNameToISIN.ToDictionary(keySelector: pair => pair.Value, elementSelector: pair => pair.Key);
        foreach (var accountHolding in _accountHoldings)
        {
            foreach (var asset in accountHolding.Value)
            {
                var isin = asset.ISIN;
                if (!isinToName.TryGetValue(isin, out var trueName)) continue;
                if (trueName != asset.Name) asset.Name = trueName;
            }
        }
    }

    public void UpdateBalance(Transaction transaction)
    {
        _accountBalance[transaction.AccountName] += transaction.Amount;
        LogBalance(transaction);
    }

    public void AddTradedAsset(Transaction transaction)
    {
        TradedAssets.Add(transaction.AssetNameOrDescription);
        if (!AssetNameToISIN.ContainsKey(transaction.AssetNameOrDescription))
        {
            AssetNameToISIN.Add(transaction.AssetNameOrDescription, transaction.ISIN);
        }
    }

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
        var result = transaction.Result ?? CalculateResult(existingAsset, transaction);
        if (AssetRealisedProfitOrLosses.ContainsKey(existingAsset.ISIN)) AssetRealisedProfitOrLosses[existingAsset.ISIN] += result;
        else AssetRealisedProfitOrLosses.Add(existingAsset.ISIN, result);
        

        existingAsset.Quantity += transaction.Quantity;

        if (existingAsset.Quantity == 0)
        {
            _accountHoldings[transaction.AccountName].Remove(existingAsset);
        }
    }

    private static decimal CalculateResult(Asset existingAsset, Transaction transaction)
    {
        var averageAcquisitionCost = Math.Abs(existingAsset.AvgAcquisitionCost);

        var sellPrice = transaction.Amount; // Realised gain, positive if sold
        var assetsSold = Math.Abs(transaction.Quantity); // negative if sold
        var result = sellPrice / assetsSold - averageAcquisitionCost;

        return result;
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
        _accountDividends[transaction.AccountName].Add(transaction);
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
        if (!_accountDividends.ContainsKey(accountName)) _accountDividends[accountName] = new Dividends();
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
