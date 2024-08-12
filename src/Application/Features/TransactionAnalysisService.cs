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
        var portfolio = new InvestmentPortfolio();
        foreach (var transaction in ordered)
        {
            switch (transaction.TransactionType)
            {
                case TransactionType.Undefined:
                    break;
                case TransactionType.Options:
                    break;
                case TransactionType.Forex:
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
                    break;
                case TransactionType.Interest:
                    portfolio.AddDepositOrWithdrawal(transaction);
                    break;
                case TransactionType.ForeignTax:
                    break;
                case TransactionType.ProvisionalTax:
                    break;
                case TransactionType.DividendProvisionalTax:
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
                case TransactionType.ShareLoanDisbursement:
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

    }

    private bool ContainsISIN(Transaction transaction)
    {
        return !string.IsNullOrEmpty(transaction.ISIN);
    }
}
