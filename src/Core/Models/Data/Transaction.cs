using System.Text.Json;
using Core.Common.Enums;

namespace Core.Models.Data;

public class Transaction
{
    public DateTime Date { get; set; }
    
    public string AccountName { get; set; }
    
    public TransactionType TransactionType { get; set; }
    
    public string AssetNameOrDescription { get; set; }
    
    public decimal Quantity { get; set; }
    
    /// <summary>
    /// The price for the asset, in the currency determined by <see cref="Currency"/>
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Total amount for the transaction,
    /// For eg. asset buys/sells, equal to <seealso cref="Quantity"/> * <seealso cref="Price"/> + <see cref="BrokerageFee"/>
    /// </summary>
    public decimal Amount { get; set; }
    
    public decimal BrokerageFee { get; set; }
    
    /// <summary>
    /// Denotes the currency in which the <see cref="Price"/> is in. 
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// International Securities Identification Number
    /// </summary>
    public string ISIN { get; set; }
    
    public decimal? Result { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }

    public bool ContainsISIN()
    {
        return !string.IsNullOrEmpty(ISIN);
    }

    public bool IsBTA()
    {
        return AssetNameOrDescription.Contains("BTA");
    }

    public bool ContainsOnlyQuantity()
    {
        return Price is decimal.Zero 
               && Amount is decimal.Zero 
               && BrokerageFee is decimal.Zero;
    }
}
