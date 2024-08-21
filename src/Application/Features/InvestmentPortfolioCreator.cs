using Core.Common.Enums;
using Core.Common.Interfaces.Application;
using Core.Interfaces.Repositories;
using Core.Models.Data;

namespace Application.Features;

public class InvestmentPortfolioCreator : IInvestmentPortfolioCreator
{
    private readonly IAvanzaRepository _avanzaRepository;

    public InvestmentPortfolioCreator(IAvanzaRepository avanzaRepository)
    {
        _avanzaRepository = avanzaRepository;
    }

    public InvestmentPortfolio Create(IList<Transaction>? transactions = null, bool verbose = false)
    {
        transactions ??= _avanzaRepository.LoadTransactions();
        var ordered = transactions.Reverse();
        var portfolio = new InvestmentPortfolio(verbose);
        var seenISINs = new HashSet<string>();
        foreach (var transaction in ordered)
        {
            switch (transaction.TransactionType)
            {
                case TransactionType.Undefined:
                    break;
                case TransactionType.Options:
                    break;
                case TransactionType.Deposit:
                    portfolio.AddDepositOrWithdrawal(transaction);
                    break;
                case TransactionType.Withdraw:
                    portfolio.AddDepositOrWithdrawal(transaction);
                    break;
                case TransactionType.Buy:
                    portfolio.AddTradedAsset(transaction);
                    portfolio.AddBuy(transaction);
                    break;
                case TransactionType.Sell:
                    portfolio.AddSell(transaction);
                    break;
                case TransactionType.Dividend:
                    portfolio.AddDividend(transaction);
                    break;
                case TransactionType.Interest:
                    portfolio.AddDepositOrWithdrawal(transaction);
                    break;
                case TransactionType.ForeignTax:
                    portfolio.AddDividend(transaction);
                    break;
                case TransactionType.ProvisionalTax:
                    portfolio.AddDepositOrWithdrawal(transaction);
                    break;
                case TransactionType.DividendProvisionalTax:
                    portfolio.AddDividend(transaction);
                    break;
                case TransactionType.AssetTransfer:
                    if (transaction.ContainsISIN())
                    {
                        // Kan vara överföring mellan egna konton
                        // särskild utdelningsförförande
                        // ombokning
                        switch (transaction.Quantity)
                        {
                            case > 0:
                                portfolio.AddAsset(transaction);
                                break;
                            case < 0:
                                portfolio.RemoveAsset(transaction);
                                break;
                            default:
                                portfolio.AddDepositOrWithdrawal(transaction);
                                break;
                        }
                        break;
                    }

                    // Avgift, avkastningsskatt, riskpremie, överföring ränta kapitalmedelskonto,
                    // moms, tjänster (ex. K4-/K11-underlag), Nollställning av outnyttjad kredit eller blank beskrivning
                    portfolio.AddDepositOrWithdrawal(transaction);
                    break;
                case TransactionType.ShareLoanDisbursement:
                    portfolio.AddDepositOrWithdrawal(transaction);
                    break;
                case TransactionType.Other:
                    if (transaction.ContainsISIN())
                    {
                        // För betald tecknad aktie (BTA) skapas ibland en transaktionsdublett av avanza, så kallad inbokning. Denna kan ignoreras och följer mönstret:
                        // 1. Vi har ett ISIN men det är nytt
                        // 2. Transaktionen innehåller endast Quantity
                        // 3. Beskrivningen innehåller "BTA".
                        if (transaction.IsBTA() && transaction.ContainsOnlyQuantity() && !seenISINs.Contains(transaction.ISIN)) break;

                        // Kan vara täckningsrätter (TR), aktiesplit, specialutdelning, tilldelning eller inlösen (IL) 
                        switch (transaction.Quantity)
                        {
                            case > 0:
                                portfolio.AddBuy(transaction);
                                break;
                            case < 0:
                                portfolio.AddSell(transaction);
                                break;
                            default:
                                portfolio.AddDepositOrWithdrawal(transaction);
                                break;
                        }
                        break;
                    }

                    // Avgift, avkastningsskatt, riskpremie, överföring ränta kapitalmedelskonto,
                    // Åtebetalning utländsk källskatt, moms,
                    // tjänster (ex. K4-/K11-underlag), Nollställning av outnyttjad kredit eller blank beskrivning
                    portfolio.AddDepositOrWithdrawal(transaction);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (transaction.ContainsISIN()) seenISINs.Add(transaction.ISIN);
        }
        portfolio.CorrectAssetNames();

        return portfolio;
    }
}
