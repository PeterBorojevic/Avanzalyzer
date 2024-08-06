using Core.Common.Enums;
using CsvHelper.Configuration.Attributes;

namespace Core.Models.Data;

public class Transaction
{
    public DateTime Date { get; set; }
    
    public string AccountName { get; set; }
    
    public TransactionType TransactionType { get; set; }
    
    public string AssetNameOrDescription { get; set; }
    
    public decimal Quantity { get; set; }
    
    public decimal Price { get; set; }

    /// <summary>
    /// Total amount for the transaction,
    /// For eg. asset buys/sells, equal to <seealso cref="Quantity"/> * <seealso cref="Price"/>
    /// </summary>
    public decimal Amount { get; set; }
    
    public decimal BrokerageFee { get; set; }
    
    public string Currency { get; set; }
    
    public string ISIN { get; set; }
    
    public decimal? Result { get; set; }
}
