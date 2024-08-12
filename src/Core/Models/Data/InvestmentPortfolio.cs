using Core.Models.Securities.Base;
using System.Transactions;
using Core.Common.Enums;
using Core.Models.Securities;
using System.Data.Common;

namespace Core.Models.Data;

public class InvestmentPortfolio
{
    private Dictionary<string, List<Asset>> _accountHoldings = new();
    private Dictionary<string, decimal> _accountBalance = new();
    private Dictionary<string, decimal> _dividends = new();
    
    public void AddBuy(Transaction transaction)
    {
        AddAccountIfNotSeen(transaction.AccountName);
        var existingAsset = GetExistingAssetOrDefault(transaction);

        if (existingAsset != null)
        {
            var assetValueAtPurchase = Math.Abs(transaction.Amount - transaction.BrokerageFee) / transaction.Quantity;
            existingAsset.AvgAcquisitionCost = CalculateNewAverageAcquisitionCost(existingAsset, transaction);
            existingAsset.Quantity += transaction.Quantity;
            existingAsset.MarketValue = assetValueAtPurchase * existingAsset.Quantity;
        }
        else
        {
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
            _accountBalance[transaction.AccountName] -= Math.Abs(transaction.Amount);
        }
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


        if (existingAsset == null) return;

        existingAsset.Quantity += transaction.Quantity;

        if (existingAsset.Quantity == 0)
        {
            _accountHoldings[transaction.AccountName].Remove(existingAsset);
        }

        if (transaction.Amount is decimal.Zero || transactionType is TransactionType.AssetTransfer)
        {
            var valueOfTrade = transaction.Quantity * transaction.Price;
            _accountBalance[transaction.AccountName] += valueOfTrade;
        }
        else
        {
            _accountBalance[transaction.AccountName] += transaction.Amount;
        }

    }

    public void AddDepositOrWithdrawal(Transaction transaction)
    {
        AddAccountIfNotSeen(transaction.AccountName);
        _accountBalance[transaction.AccountName] += transaction.Amount;
    }

    public void DeductAsset()
    {

    }

    public void AddAsset()
    {

    }
    private void AddAccountIfNotSeen(string accountName)
    {
        if (!_accountBalance.ContainsKey(accountName)) _accountBalance[accountName] = 0;
        if (!_accountHoldings.ContainsKey(accountName)) _accountHoldings[accountName] = new List<Asset>();
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
                _accountBalance[accountName] = decimal.Zero;
            }
        }
    }
}