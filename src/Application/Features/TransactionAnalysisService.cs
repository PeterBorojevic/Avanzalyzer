using Core.Common.Enums;
using Core.Common.Interfaces.Application;
using Core.Models.Data;

namespace Application.Features;

public class TransactionAnalysisService : ITransactionAnalysisService
{
    public TransactionAnalysisService()
    {
        
    }

    public InvestmentPortfolio ParseTransactions(IList<Transaction> transactions, bool verbose = false)
    {
        var ordered = transactions.Reverse();
        //var ordered = transactions.OrderBy(t => t.Date);
        //TODO känt fel, Inbokning av teckningsrätter. Avanza exporterar dessa under transaktionstypen Other (Övrigt). Detta skapar dubletter
        // Det finns ett mönster för dessa, nytt ISIN, saknar Price, saknar Amount. Endast Quantity och ISIN. Men, andra övriga transaktioner.
        // Men ignorerar vi det första värdepappret med nytt ISIN och avsaknad av andra värden kommer nästa övrig transaktion uppfylla samma krav.
        
        // Vi bör ignorera dessa unika transaktioner om
        // 1. Vi har ett ISIN men det är nytt
        // 2. Transaktionen innehåller endast Quantity
        // 3. Beskrivningen innehåller "BTA".
        var portfolio = new InvestmentPortfolio(verbose);
        var sum = transactions.Sum(t => t.Amount);
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
                                continue;
                            case < 0:
                                portfolio.AddSell(transaction, TransactionType.AssetTransfer);
                                continue;
                            default:
                                portfolio.AddDepositOrWithdrawal(transaction);
                                continue;
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
                                continue;
                            case < 0:
                                portfolio.AddSell(transaction);
                                continue;
                            default:
                                portfolio.AddDepositOrWithdrawal(transaction);
                                continue;
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

        return portfolio;
    }

    private bool ContainsISIN(Transaction transaction)
    {
        return !string.IsNullOrEmpty(transaction.ISIN);
    }
}
