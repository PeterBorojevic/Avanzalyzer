using Core.Common.Enums;
using Core.Models.Dtos.Csv;
using Core.Models.Securities.Base;

namespace Core.Models.Securities;

public class Fund : Asset
{
    public Fund(Position position) : base(position, AssetType.Fund)
    {
    }
}
