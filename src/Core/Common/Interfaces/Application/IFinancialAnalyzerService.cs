using Core.Common.Enums;

namespace Core.Common.Interfaces.Application;

public interface IFinancialAnalyzerService
{
    IPrintable GetDividends(GroupingType groupBy = GroupingType.ByYear);
    IPrintable GetAccountTotals();
    IPrintable GetDepositsAndWithdrawals();

}
