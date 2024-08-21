using Core.Common.Enums;
using Core.Common.Interfaces.Application;
using Core.Interfaces.Repositories;
using Core.Models.Data;

namespace Application.Features;

public class TransactionAnalysisService : ITransactionAnalysisService
{
    private readonly IAvanzaRepository _avanzaRepository;
    public TransactionAnalysisService(IAvanzaRepository avanzaRepository)
    {
        _avanzaRepository = avanzaRepository;
    }

    // What can this service do?

    
}
