using System.Text;
using Core.Common.Enums;
using Core.Models.Securities.Base;

namespace Core.Models.Securities;

public class Investment : Asset
{
    public Investment(AssetType assetType = AssetType.UnknownAsset) : base(assetType)
    {
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"{Name,-20}").Append($", Q={Quantity,-3}, ");
        sb.Append($"Value= {MarketValue,-16:C}, ").Append($"GAV= {AvgAcquisitionCost,-16:C}");
        return sb.ToString();
    }
}
