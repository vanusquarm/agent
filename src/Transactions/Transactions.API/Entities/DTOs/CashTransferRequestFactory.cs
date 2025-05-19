using System.Globalization;
using Finacle.Infrastructure.Utils;

namespace Transactions.API.Entities.DTOs;

public class CashTransferRequestFactory
{
    public static CashTransferRequest Create(string debitAccount, string creditAccount, decimal amount, string partTransactionRemarks, string referenceNumber, string transType, string? currency, List<DenominationDetailsCurrencyType>? denomDetails)
    {
        string transactionDate = DateTime.Now.ToString("yyyy-MM-dd");
        string inferredCurrency = Utils.ParseAccountNumber(creditAccount).Currency;

        return new CashTransferRequest
        {
            TransactionType = "C",
            TransactionSubType = transType == "D" ? "PI" : "NR",
            TransactionSummary = new List<TransactionSummary>
            {
                new TransactionSummary
                {
                    SerialNumber = "001",
                    PartTransactionType = transType,
                    AccountIdentification = creditAccount,
                    TransactionAmount = amount,
                    TransactionCurrency = currency ?? inferredCurrency,
                    ValuedDate = transactionDate,
                    PrintAdvice = "O",
                    TransactionParticulars = referenceNumber
                }
            },
            DenominationDetails = new List<DenominationDetail>
            {
                new DenominationDetail
                {
                    DenominationDetailsCurrencyType = denomDetails ?? SplitAmount(amount, currency ?? inferredCurrency),
                    RoundOffIndicator = "C",
                    RoundOffAmount = new RoundOffAmount
                    {
                        Value = amount - Math.Truncate(amount),
                        Currency = inferredCurrency
                    },
                    DenominationRemarks = "ok"
                }
            },
            UserTransactionCode = "102",
            TransactionRemarks = partTransactionRemarks,
            EntererId = debitAccount
        };
    }

    private static List<DenominationDetailsCurrencyType> SplitAmount(decimal amount, string currency)
    {
        int[] denominations = { 100, 50, 20, 10, 5, 2, 1 };
        var result = new List<DenominationDetailsCurrencyType>();

        foreach (var note in denominations.OrderByDescending(d => d))
        {
            int count = (int)(amount / note);
            if (count > 0)
            {
                result.Add(new DenominationDetailsCurrencyType
                {
                    DenominationCurrency = currency,
                    Denomination = note,
                    DenominationType = "N",
                    StrapOrBundleSize = 100,
                    NumberOfNotesOrCoins = count,
                    NumberOfStrapsOrBundles = 0,
                    NumberOfNotesOrCoinsPaybackOrGetback = 0,
                    NumberOfStrapsOrBundlesPaybackOrGetback = 0
                });

                amount %= note;
            }
        }

        return result;
    }
}
