using Core.Common.Converters;
using CsvHelper.Configuration.Attributes;

namespace Core.Models;

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
}