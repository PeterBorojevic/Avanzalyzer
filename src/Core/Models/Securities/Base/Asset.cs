using Core.Common.Converters;
using Core.Common.Enums;
using Core.Models.Dtos.Csv;
using CsvHelper.Configuration.Attributes;

namespace Core.Models.Securities.Base;

public abstract class Asset
{
    protected Asset(Position position, AssetType assetType)
    {
        AccountNumber = position.AccountNumber;
        Name = position.Name;
        Quantity = position.Quantity;
        MarketValue = position.MarketValue;
        AvgAcquisitionCost = position.AvgAcquisitionCost;
        ISIN = position.ISIN;
        Currency = position.Currency;
        Type = assetType;
    }

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
    /// Valuta (SEK, EUR, USD, NOK, etc)
    /// </summary>
    [Name("Valuta")]
    public string Currency { get; set; }

    /// <summary>
    /// Typ av värdepapper
    /// </summary>
    [Name("Typ")]
    public AssetType Type { get; set; }

    // Calculated properties

    public decimal AcquisitionCost => AvgAcquisitionCost * Quantity;
    public decimal ProfitOrLoss => MarketValue - AcquisitionCost;
    public decimal PercentageChange => decimal.Divide(MarketValue, AcquisitionCost) - 1;
    public decimal AssetValue => decimal.Divide(MarketValue, Quantity);
}
