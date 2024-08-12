using Core.Common.Enums;
using Core.Models.Securities.Base;

namespace Core.Models.Securities;

public class Investment : Asset
{
    public Investment(AssetType assetType = AssetType.UnknownAsset) : base(assetType)
    {
    }
}
