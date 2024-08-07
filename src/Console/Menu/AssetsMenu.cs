using Console.Interfaces;
using Core.Common.Interfaces.Application;

namespace Console.Menu;

public class AssetsMenu : IAssetMenu
{
    private readonly IFinancialAnalyzerService _analyzer;

    public AssetsMenu(IFinancialAnalyzerService analyzer)
    {
        _analyzer = analyzer;
    }


    public UserAction Next()
    {
        throw new NotImplementedException();
    }

    public UserAction Show()
    {
        throw new NotImplementedException();
    }
}
