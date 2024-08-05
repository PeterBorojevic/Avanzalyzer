﻿namespace Core.Common.Enums;

public enum TransactionType
{
    Undefined,
    Options,
    Forex,
    /// <summary>
    /// Insättning/Uttag
    /// </summary>
    DepositWithdraw,
    /// <summary>
    /// Köp/Sälj
    /// </summary>
    BuySell,
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
    Other
}
