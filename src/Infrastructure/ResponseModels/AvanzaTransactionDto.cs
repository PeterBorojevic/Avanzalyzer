using Core.Models.Data;
using Core.Common.Enums;
using CsvHelper.Configuration.Attributes;
using Core.Common.Converters;

namespace Infrastructure.ResponseModels;

public class AvanzaTransactionDto
{
    [Name("Datum")]
    public DateTime Date { get; set; }

    [Name("Konto")]
    public string AccountName { get; set; }

    [Name("Typ av transaktion")]
    public string TransactionType { get; set; }

    [Name("Värdepapper/beskrivning")]
    public string AssetNameOrDescription { get; set; }

    [Name("Antal")]
    [TypeConverter(typeof(CustomDecimalConverter))]
    public decimal Quantity { get; set; }

    [Name("Kurs")]
    [TypeConverter(typeof(CustomDecimalConverter))]
    public decimal Price { get; set; }
    
    /// <summary>
    /// Total amount for the transaction,
    /// For eg. asset buys/sells, equal to <seealso cref="Quantity"/> * <seealso cref="Price"/>
    /// </summary>
    [Name("Belopp")]
    [TypeConverter(typeof(CustomDecimalConverter))]
    public decimal Amount { get; set; }

    [Name("Courtage")]
    [TypeConverter(typeof(CustomDecimalConverter))]
    public decimal BrokerageFee { get; set; }

    [Name("Valuta")]
    public string Currency { get; set; }

    [Name("ISIN")]
    public string ISIN { get; set; }

    [NullValues("-")]
    [Name("Resultat")]
    public decimal? Result { get; set; }

    public Transaction ToInternal() => new Transaction
    {
        Date = Date,
        AccountName = AccountName,
        TransactionType = TransactionType switch
        {
            "Insättning" => Core.Common.Enums.TransactionType.Deposit, 
            "Uttag" => Core.Common.Enums.TransactionType.Withdraw,
            "Köp" => Core.Common.Enums.TransactionType.Buy,
            "Sälj" => Core.Common.Enums.TransactionType.Sell,
            "Utdelning" => Core.Common.Enums.TransactionType.Dividend,
            "Ränta" => Core.Common.Enums.TransactionType.Interest,
            "Utländsk källskatt" => Core.Common.Enums.TransactionType.ForeignTax,
            "Preliminärskatt" => Core.Common.Enums.TransactionType.ProvisionalTax,
            "Prelskatt utdelning" => Core.Common.Enums.TransactionType.DividendProvisionalTax,
            "Värdepappersöverföring" => Core.Common.Enums.TransactionType.AssetTransfer,
            "Utbetalning aktielån" => Core.Common.Enums.TransactionType.ShareLoanDisbursement,
            "Övrigt" => Core.Common.Enums.TransactionType.Other,
            _ => Core.Common.Enums.TransactionType.Undefined,

        },
        AssetNameOrDescription = AssetNameOrDescription,
        Quantity = Quantity,
        Price = Price,
        Amount = Amount,
        BrokerageFee = BrokerageFee,
        Currency = Currency,
        ISIN = ISIN,
        Result = Result
    };
}
