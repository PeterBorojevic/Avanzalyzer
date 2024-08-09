using Application.Features;
using Core.Common.Interfaces.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceInstaller
{
    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        services.AddTransient<IFinancialAnalyzerService, FinancialAnalyzerService>();
        services.AddTransient<IPortfolioAnalyzerService, PortfolioAnalyzerService>();

        return services;
    }
}
