using Core.Common.Enums;
using Core.Common.Interfaces.Application;
using Core.Models.Data;
using Core.Models.Securities.Base;

namespace Application.Features;

public class TransactionAnalysisService : ITransactionAnalysisService
{
    public TransactionAnalysisService()
    {
        
    }

    public void ParseTransactions(IList<Transaction> transactions)
    {
        var ordered = transactions.Reverse();
        //var ordered = transactions.OrderBy(t => t.Date);
        //TODO känt fel, Inbokning av teckningsrätter. Avanza exporterar dessa under transaktionstypen Other (Övrigt). Detta skapar dubletter
        // Det finns ett mönster för dessa, nytt ISIN, saknar Price, saknar Amount. Endast Quantity och ISIN. Men, andra övriga transaktioner.
        // Men ignorerar vi det första värdepappret med nytt ISIN och avsaknad av andra värden kommer nästa övrig transaktion uppfylla samma krav.
        // Krävs att endast den första ignoreras, alt. städas/raderas bort från exceldatat. Det finns ingen annan information under Description som hade kunnat användas.
        var portfolio = new InvestmentPortfolio(false);
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
                    if (ContainsISIN(transaction))
                    {
                        // Kan vara överföring mellan egna konton
                        // särskild utdelningsförförande
                        // ombokning
                         
                        switch (transaction.Quantity)
                        {
                            case > 0:
                                portfolio.AddBuy(transaction);
                                break;
                            case < 0:
                                portfolio.AddSell(transaction, TransactionType.AssetTransfer);
                                break;
                            default:
                                portfolio.AddDepositOrWithdrawal(transaction);
                                break;
                        }
                    }

                    // Avgift, avkastningsskatt, riskpremie, överföring ränta kapitalmedelskonto,
                    // moms, tjänster (ex. K4-/K11-underlag), Nollställning av outnyttjad kredit eller blank beskrivning
                    portfolio.AddDepositOrWithdrawal(transaction);
                    break;
                case TransactionType.ShareLoanDisbursement:
                    portfolio.AddDepositOrWithdrawal(transaction);
                    break;
                case TransactionType.Other:
                    if (ContainsISIN(transaction))
                    {
                        // Kan vara täckningsrätter (TR), betald tecknad aktie (BTA),
                        // aktiesplit, specialutdelning, tilldelning eller inlösen (IL) 
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
                    }

                    // Avgift, avkastningsskatt, riskpremie, överföring ränta kapitalmedelskonto,
                    // moms, tjänster (ex. K4-/K11-underlag), Nollställning av outnyttjad kredit eller blank beskrivning
                    portfolio.AddDepositOrWithdrawal(transaction);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }

    private bool ContainsISIN(Transaction transaction)
    {
        return !string.IsNullOrEmpty(transaction.ISIN);
    }
}
