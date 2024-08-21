using Core.Common.Converters;
using Core.Models.Securities;
using Core.Models.Securities.Base;
using CsvHelper.Configuration.Attributes;

namespace Core.Models.Dtos.Csv;
/* TODO
 * This class is used by the infrastructure layer as well by application. Could we separate infrastructure models and internal ones?
 */
public class Position
{
    /// <summary>
    /// Kontonummer
    /// </summary>
    [Name("Kontonummer")]
    public string AccountNumber { get; set; }
    /// <summary>
    /// Name
    /// </summary>
    [Name("Namn")]
    public string Name { get; set; }
    /// <summary>
    /// Volym
    /// </summary>
    [Name("Volym")]
    [TypeConverter(typeof(CustomDecimalConverter))]
    public decimal Quantity { get; set; }
    /// <summary>
    /// Marknadsvärde
    /// </summary>
    [Name("Marknadsvärde")]
    [TypeConverter(typeof(CustomDecimalConverter))]
    public decimal MarketValue { get; set; }
    /// <summary>
    /// GAV
    /// </summary>
    [Name("Genomsnittligt anskaffningsvärde")]
    [TypeConverter(typeof(CustomDecimalConverter))]
    public decimal AvgAcquisitionCost { get; set; }

    [Name("ISIN")]
    public string ISIN { get; set; }

    /// <summary>
    /// Valuta (SEK, EUR, USD, NOK 
    /// </summary>
    [Name("Valuta")]
    public string Currency { get; set; }

    /// <summary>
    /// Typ (FUND, STOCK, EXCHANGE_TRADED_FUND, CERTIFICATE)
    /// </summary>
    [Name("Typ")]
    public string Type { get; set; }

    // Calculated properties

    [Ignore] public decimal AcquisitionCost => AvgAcquisitionCost * Quantity;
    [Ignore] public decimal ProfitOrLoss => MarketValue - AcquisitionCost;
    [Ignore] public decimal PercentageChange => decimal.Divide(MarketValue, AcquisitionCost) - 1;
    [Ignore] public decimal AssetValue => decimal.Divide(MarketValue, Quantity);

    public Asset ToAsset() => Type switch
    {
        "STOCK" => new Stock(this),
        "FUND" => new Fund(this),
        "EXCHANGE_TRADED_FUND" => new ExchangeTradedFund(this),
        "CERTIFICATE" => new Certificate(this),
        _ => throw new ArgumentOutOfRangeException()
    };
}
