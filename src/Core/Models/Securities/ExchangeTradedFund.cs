using Core.Common.Enums;
using Core.Models.Dtos.Csv;
using Core.Models.Securities.Base;

namespace Core.Models.Securities;

public class ExchangeTradedFund : Asset
{
    public ExchangeTradedFund(Position position) : base(position, AssetType.Etf)
    {
    }
}
