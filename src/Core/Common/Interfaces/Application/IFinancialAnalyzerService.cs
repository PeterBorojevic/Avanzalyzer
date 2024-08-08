using Core.Common.Enums;
using Core.Models;
using Core.Models.Data;
using Core.Models.Settings;

namespace Core.Common.Interfaces.Application;

public interface IFinancialAnalyzerService
{
    IPrintable Get(AnalysisCalculationType type, Portfolio? portfolio = null, List<Transaction>? transactions = null);
    IPrintable Get(AnalysisCalculationType type, Action<FinancialAnalysisOptions> options);
}
