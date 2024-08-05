using Core.Models.Data;
using Core.Common.Enums;
using CsvHelper.Configuration.Attributes;

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
    [NullValues("-")]
    public decimal? Quantity { get; set; }

    [Name("Kurs")]
    [NullValues("-")]
    public decimal? Price { get; set; }
    
    /// <summary>
    /// Total amount for the transaction,
    /// For eg. asset buys/sells, equal to <seealso cref="Quantity"/> * <seealso cref="Price"/>
    /// </summary>
    [Name("Belopp")]
    [NullValues("-")]
    public decimal? Amount { get; set; }

    [Name("Courtage")]
    [NullValues("-")]
    public decimal? BrokerageFee { get; set; }

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
            "Insättning" => Core.Common.Enums.TransactionType.DepositWithdraw, 
            "Uttag" => Core.Common.Enums.TransactionType.DepositWithdraw,
            "Köp" => Core.Common.Enums.TransactionType.BuySell,
            "Sälj" => Core.Common.Enums.TransactionType.BuySell,
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
