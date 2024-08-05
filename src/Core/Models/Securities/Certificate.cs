using Core.Common.Enums;
using Core.Models.Dtos.Csv;
using Core.Models.Securities.Base;

namespace Core.Models.Securities;

public class Certificate : Asset
{
    public Certificate(Position position) : base(position, AssetType.Certificate)
    {
    }
}
