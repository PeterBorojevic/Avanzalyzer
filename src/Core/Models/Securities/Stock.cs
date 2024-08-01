using Core.Common.Enums;
using Core.Models.Dtos;
using Core.Models.Securities.Base;

namespace Core.Models.Securities;

public class Stock : Asset
{
    public Stock(Position position) : base(position, AssetType.Stock)
    {
    }
}
