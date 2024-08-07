using Core.Common.Enums;

namespace Core.Common.Interfaces.Application;

public interface IFinancialAnalyzerService
{
    IPrintable Get(AnalysisCalculationType type);
    IPrintable GetDividends(GroupingType groupBy = GroupingType.ByYear);
    IPrintable GetAccountTotals();
    IPrintable GetDepositsAndWithdrawals();

}
