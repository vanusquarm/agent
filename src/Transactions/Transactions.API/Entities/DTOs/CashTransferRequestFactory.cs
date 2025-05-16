using System.Globalization;

namespace Transactions.API.Entities.DTOs;

public class CashTransferRequestFactory
{
    public static CashTransferRequest Create(string debitAccount, string creditAccount, decimal amount, string partTransactionRemarks, string referenceNumber, string transType, string? currency, List<DenominationDetailsCurrencyType>? denomDetails)
    {
        string transactionDate = DateTime.Now.ToString("yyyy-MM-dd");

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
                    TransactionCurrency = currency ?? "GHS",
                    ValuedDate = transactionDate,
                    PrintAdvice = "O",
                    TransactionParticulars = referenceNumber
                }
            },
            DenominationDetails = new List<DenominationDetail>
            {
                new DenominationDetail
                {
                    DenominationDetailsCurrencyType = denomDetails ?? SplitAmount(amount, currency),
                    RoundOffIndicator = "C",
                    RoundOffAmount = new RoundOffAmount
                    {
                        Value = amount - Math.Truncate(amount),
                        Currency = "GHS"
                    },
                    DenominationRemarks = "ok"
                }
            },
            UserTransactionCode = "102",
            TransactionRemarks = partTransactionRemarks,
            EntererId = debitAccount
        };
    }

    public static List<DenominationDetailsCurrencyType> SplitAmount(decimal amount, string? currency)
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
                    DenominationCurrency = currency ?? "GHS",
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
