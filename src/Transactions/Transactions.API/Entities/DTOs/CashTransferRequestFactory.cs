using System.Globalization;

namespace Transactions.API.Entities.DTOs;

public class CashTransferRequestFactory
{
    public static CashTransferRequest Create(string debitAccount, string creditAccount, int amount, string partTransactionRemarks, string referenceNumber, string transType, string? currency, List<DenominationDetailsCurrencyType>? denomDetails)
    {
        DateTime transactionDate = DateTime.ParseExact("2025-04-27", "yyyy-MM-dd", CultureInfo.InvariantCulture);
        //DateTime transactionDate = DateTime.ParseExact(
        //    DateTime.Now.ToString("yyyy-MM-dd"),
        //    "yyyy-MM-dd",
        //    CultureInfo.InvariantCulture
        //);

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
                    ValuedDate = "2025-04-27",
                    PrintAdvice = "O",
                    TransactionParticulars = referenceNumber
                }
            },
            DenominationDetails = new List<DenominationDetail>
            {
                new DenominationDetail
                {
                    DenominationDetailsCurrencyType = denomDetails ?? SplitAmount(amount),
                    RoundOffIndicator = "C",
                    RoundOffAmount = new RoundOffAmount
                    {
                        Value = 0,
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

    public static List<DenominationDetailsCurrencyType> SplitAmount(int amount)
    {
        int[] denominations = { 100, 50, 20, 10, 5 };
        var result = new List<DenominationDetailsCurrencyType>();

        foreach (var note in denominations)
        {
            int count = amount / note;
            if (count > 0)
            {
                result.Add(new DenominationDetailsCurrencyType
                {
                    DenominationCurrency = "GHS", // statically set
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
