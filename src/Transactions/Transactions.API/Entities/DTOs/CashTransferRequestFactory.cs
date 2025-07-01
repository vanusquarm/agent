using System.Globalization;
using Finacle.Infrastructure.Utils;

namespace Transactions.API.Entities.DTOs;

public class CashTransferRequestFactory
{
    public static CashTransferRequest Create(string debitAccount, string creditAccount, decimal amount, string partTransactionRemarks, string referenceNumber, string transType, string? currency, List<DenominationDetailsCurrencyType>? denomDetails)
    {
        string transactionDate = DateTime.Now.ToString("yyyy-MM-dd");
        currency = currency ?? Utils.ParseAccountNumber(creditAccount).Currency;

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
                    TransactionCurrency = currency,
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
                        Currency = currency
                    },
                    DenominationRemarks = "ok"
                }
            },
            UserTransactionCode = "102",
            TransactionRemarks = partTransactionRemarks.Split(50, 1),
            EntererId = debitAccount
        };
    }

    public static CashTransferRequest Create(
        string debitAccount, string creditAccount, decimal amount, string partTransactionRemarks,
        string referenceNumber, string transType, string? currency, Dictionary<string, string>? denomDetails)
    {
        var denomList = denomDetails != null
            ? ConvertDictionaryToDenominationDetails(denomDetails, currency ?? Utils.ParseAccountNumber(creditAccount).Currency)
            : null;

        return Create(debitAccount, creditAccount, amount, partTransactionRemarks, referenceNumber, transType, currency, denomList);
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

    private static List<DenominationDetailsCurrencyType> ConvertDictionaryToDenominationDetails(
    Dictionary<string, string> denomDetails, string currency)
    {
        var result = new List<DenominationDetailsCurrencyType>();
        foreach (var kvp in denomDetails)
        {
            if (int.TryParse(kvp.Key, out int denomination) && int.TryParse(kvp.Value, out int count))
            {
                result.Add(new DenominationDetailsCurrencyType
                {
                    DenominationCurrency = currency,
                    Denomination = denomination,
                    DenominationType = "N",
                    StrapOrBundleSize = 100,
                    NumberOfNotesOrCoins = count,
                    NumberOfStrapsOrBundles = 0,
                    NumberOfNotesOrCoinsPaybackOrGetback = 0,
                    NumberOfStrapsOrBundlesPaybackOrGetback = 0
                });
            }
        }
        return result;
    }

}
