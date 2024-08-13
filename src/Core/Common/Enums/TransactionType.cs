namespace Core.Common.Enums;

public enum TransactionType
{
    Undefined,
    Options,
    /// <summary>
    /// Insättning
    /// </summary>
    Deposit,
    /// <summary>
    /// Uttag
    /// </summary>
    Withdraw,
    /// <summary>
    /// Köp
    /// </summary>
    Buy,
    /// <summary>
    /// Sälj
    /// </summary>
    Sell,
    /// <summary>
    /// Utdelning
    /// </summary>
    Dividend,
    /// <summary>
    /// Ränta
    /// </summary>
    Interest,
    /// <summary>
    /// Utländsk källskatt
    /// </summary>
    ForeignTax,
    ProvisionalTax,
    DividendProvisionalTax,
    AssetTransfer,
    ShareLoanDisbursement,
    /// <summary>
    /// Övrigt
    /// </summary>
    Other
}
