namespace Core.Common.Enums;

public enum AssetType
{
    UnknownAsset,
    Stock,
    Fund,
    ExchangeTradedFund,
    Certificate,
    Bond,
    Option,
    FutureForward,
    Warrant,
    Index,
    PremiumBond,
    SubscriptionOption,
    EquityLinkedBond,
    Convertible
}

public static class AssetTypeExtensions
{
    public static string PluralName(this AssetType type)
    {
        return type switch
        {
            AssetType.Stock => "Aktier",
            AssetType.Fund => "Fonder",
            AssetType.ExchangeTradedFund => "ETF:er",
            AssetType.Certificate => "Certifikat",
            AssetType.Bond => "Obligationer",
            AssetType.Option => "Optioner",
            AssetType.FutureForward => "Futures",
            AssetType.Warrant => "Warranter",
            AssetType.Index => "Index",
            AssetType.PremiumBond => "Premiumobligationer",
            AssetType.SubscriptionOption => "Teckningsoptioner",
            AssetType.EquityLinkedBond => "Aktieindexobligation",
            AssetType.Convertible => "Konvertiblar",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
