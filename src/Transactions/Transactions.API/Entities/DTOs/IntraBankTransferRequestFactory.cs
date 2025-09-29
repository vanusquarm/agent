using Finacle.Infrastructure.Utils;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Transactions.API.Entities.DTOs;

public class IntraBankTransferRequestFactory
{
    public static IntraBankTransferRequest Create(string requestReference, string debitAccount, string creditAccount, decimal amount, string partTransactionRemarks, string referenceNumber, string channel, 
        string? currency, string? docNum, string? chargeEventId, string? batchId, string? explanationCode, ChequeDetails? chequeDetails = null)
    {
        DateTime transactionDate = DateTime.ParseExact(
            DateTime.Now.ToString("yyyy-MM-dd"),
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture
        );

        var (remarksPart1, remarksPart2, remarksPart3) = SplitTransactionRemarks(partTransactionRemarks);

        return new IntraBankTransferRequest
        {
            TransactionType = IntraBankTransferConstants.TransactionType,
            TransactionSubType = IntraBankTransferConstants.TransactionSubType,
            ChannelIdentification = (channel.Length >= 3) ? channel.Substring(0, 3).ToUpper() : IntraBankTransferConstants.ChannelIdentification,
            EntererId = null,
            UserTransactionCode = explanationCode ?? IntraBankTransferConstants.UserTransactionCode,
            TransferTransactionDetail = new List<IntraBankTransferTransactionDetail>
            {
                new IntraBankTransferTransactionDetail
                {
                    SerialNumber = IntraBankTransferConstants.SerialNumberDebit,
                    PartTransactionType = IntraBankTransferConstants.PartTransactionTypeDebit,
                    AccountIdentification = debitAccount,
                    TransactionAmount = amount,
                    TransactionCurrency = currency ?? IntraBankTransferConstants.TransactionCurrency,
                    TransactionValueDate = transactionDate,
                    TransactionParticulars = remarksPart1,
                    Rate = IntraBankTransferConstants.Rate,
                    PartTransactionRemarks = remarksPart2,
                    //ReferenceNumber = referenceNumber.Split(20, 1),
                    ReferenceNumber = referenceNumber,
                    PartTransactionRemarks2 = remarksPart3,
                    LienDetailsList = new List<LienDetails>(),
                    ChargeEventId = chargeEventId,
                    InstrumentDetails = chequeDetails != null ? new InstrumentDetails
                    {
                        InstrumentDate = chequeDetails.InstrumentDate,
                        InstrumentAlpha = chequeDetails.InstrumentAlpha,
                        InstrumentNumber = chequeDetails.InstrumentNumber,
                        InstrumentType = chequeDetails.InstrumentType
                    }: null,
                    MiscellaneousDetailsType = new TransferMiscellaneousDetailsType
                    {
                        DocumentNumber = docNum ?? "DocNum",
                        Freetxt2 = requestReference,
                        Freetxt3 = requestReference,
                        Freetxt4 = batchId
                    },
                    UserPartTransactionCode = explanationCode ?? IntraBankTransferConstants.UserTransactionCode,
                },
                new IntraBankTransferTransactionDetail
                {
                    SerialNumber = IntraBankTransferConstants.SerialNumberCredit,
                    PartTransactionType = IntraBankTransferConstants.PartTransactionTypeCredit,
                    AccountIdentification = creditAccount,
                    TransactionAmount = amount,
                    TransactionCurrency = currency ?? IntraBankTransferConstants.TransactionCurrency,
                    TransactionValueDate = transactionDate,
                    TransactionParticulars = remarksPart1,
                    Rate = IntraBankTransferConstants.Rate,
                    PartTransactionRemarks = remarksPart2,
                    ReferenceNumber = referenceNumber.Split(20, 1),
                    PartTransactionRemarks2 = remarksPart3,
                    LienDetailsList = new List<LienDetails>(),
                    UserPartTransactionCode = explanationCode ?? IntraBankTransferConstants.UserTransactionCode,
                }
            }
        };
    }

    public static IntraBankTransferRequest CreateBatch(string requestReference, string debitAccount, List<CreditTarget> creditAccounts, string partTransactionRemarks, string referenceNumber, string channel, 
        string? currency, string? chargeEventId, string? batchId, string? explanationCode)
    {
        DateTime transactionDate = DateTime.ParseExact(
            DateTime.Now.ToString("yyyy-MM-dd"),
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture
        );
        currency = currency ?? Utils.ParseAccountNumber(debitAccount).Currency;
        var (remarksPart1, remarksPart2, remarksPart3) = SplitTransactionRemarks(partTransactionRemarks);

        var totalDebitAmount = creditAccounts.Sum(c => c.Amount);

        return new IntraBankTransferRequest
        {
            TransactionType = IntraBankTransferConstants.TransactionType,
            TransactionSubType = IntraBankTransferConstants.TransactionSubType,
            ChannelIdentification = (channel.Length >= 3) ? channel.Substring(0, 3).ToUpper() : IntraBankTransferConstants.ChannelIdentification,
            TransferTransactionDetail = new List<IntraBankTransferTransactionDetail>
            {
                // Debit leg
                new IntraBankTransferTransactionDetail
                {
                    SerialNumber = IntraBankTransferConstants.SerialNumberDebit,
                    PartTransactionType = IntraBankTransferConstants.PartTransactionTypeDebit,
                    AccountIdentification = debitAccount,
                    TransactionAmount = totalDebitAmount,
                    TransactionCurrency = currency,
                    TransactionValueDate = transactionDate,
                    TransactionParticulars = remarksPart1,
                    Rate = IntraBankTransferConstants.Rate,
                    PartTransactionRemarks = remarksPart2,
                    ReferenceNumber = referenceNumber.Split(20, 1),
                    PartTransactionRemarks2 = remarksPart3,
                    LienDetailsList = new List<LienDetails>(),
                    ChargeEventId = chargeEventId,
                    MiscellaneousDetailsType = new TransferMiscellaneousDetailsType
                    {
                        DocumentNumber = "Batch",
                        Freetxt2 = requestReference,
                        Freetxt3 = requestReference,
                        Freetxt4 = batchId
                    },
                    UserPartTransactionCode = explanationCode ?? IntraBankTransferConstants.UserTransactionCode,
                },
            } // Credit legs
            .Concat(creditAccounts.Select((creditAccount, index) => new IntraBankTransferTransactionDetail
            {
                SerialNumber = (index + IntraBankTransferConstants.SerialNumberCredit).ToString(),
                PartTransactionType = IntraBankTransferConstants.PartTransactionTypeCredit,
                AccountIdentification = creditAccount?.AccountId.Trim(),
                TransactionAmount = creditAccount.Amount,
                TransactionCurrency = currency,
                TransactionValueDate = transactionDate,
                TransactionParticulars = remarksPart1,
                Rate = IntraBankTransferConstants.Rate,
                PartTransactionRemarks = remarksPart2,
                ReferenceNumber = creditAccount?.Reference.Split(20, 1),
                PartTransactionRemarks2 = remarksPart3,
                LienDetailsList = new List<LienDetails>()
            })).ToList()
        };
    }

    private static (string, string, string) SplitTransactionRemarks(string partTransactionRemarks)
    {
        // Ensure the string is not null or empty
        if (string.IsNullOrEmpty(partTransactionRemarks))
            return ("", "", "");

        // Split into chunks: 80, 50, 50
        string part1 = partTransactionRemarks.Length > 0 ? partTransactionRemarks.Substring(0, Math.Min(80, partTransactionRemarks.Length)) : "";
        string part2 = partTransactionRemarks.Length > 80 ? partTransactionRemarks.Substring(80, Math.Min(50, partTransactionRemarks.Length - 80)) : "";
        string part3 = partTransactionRemarks.Length > 130 ? partTransactionRemarks.Substring(130, Math.Min(50, partTransactionRemarks.Length - 130)) : "";

        return (part1, part2, part3);
    }

}

public static class StringExtensions
{
    public static IEnumerable<string> Split(this string input, int chunkSize)
    {
        if (string.IsNullOrEmpty(input) || chunkSize <= 0)
            return Enumerable.Empty<string>();

        return input.Chunk(chunkSize).Select(chunk => new string(chunk));
    }
    public static string Split(this string input, int chunkSize, int chunkIndex)
    {
        return input.Split(chunkSize).Skip(chunkIndex - 1).FirstOrDefault() ?? string.Empty;
    }
}
