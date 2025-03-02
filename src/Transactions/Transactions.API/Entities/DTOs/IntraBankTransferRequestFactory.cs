using System.Globalization;

namespace Transactions.API.Entities.DTOs;

public class IntraBankTransferRequestFactory
{
    public static IntraBankTransferRequest Create(string debitAccount, string creditAccount, decimal amount, string partTransactionRemarks, string referenceNumber, string channel, string? docNum, string? chargeEventId)
    {
        DateTime transactionDate = DateTime.ParseExact(
            DateTime.Now.ToString("yyyy-MM-dd"),
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture
        );
        return new IntraBankTransferRequest
        {
            TransactionType = IntraBankTransferConstants.TransactionType,
            TransactionSubType = IntraBankTransferConstants.TransactionSubType,
            ChannelIdentification = (channel.Length >= 3) ? channel.Substring(0, 3).ToUpper() : IntraBankTransferConstants.ChannelIdentification,
            EntererId = MapService(channel),
            TransferTransactionDetail = new List<IntraBankTransferTransactionDetail>
                    {
                        new IntraBankTransferTransactionDetail
                        {
                            SerialNumber = IntraBankTransferConstants.SerialNumberDebit,
                            PartTransactionType = IntraBankTransferConstants.PartTransactionTypeDebit,
                            AccountIdentification = debitAccount,
                            TransactionAmount = amount,
                            TransactionCurrency = IntraBankTransferConstants.TransactionCurrency,
                            TransactionValueDate = transactionDate,
                            TransactionParticulars = partTransactionRemarks.Split(50, 1),
                            Rate = IntraBankTransferConstants.Rate,
                            PartTransactionRemarks = partTransactionRemarks.Split(50, 2),
                            ReferenceNumber = referenceNumber.Split(20, 1),
                            PartTransactionRemarks2 = partTransactionRemarks.Split(50, 3),
                            LienDetailsList = new List<LienDetails>(),
                            ChargeEventId = chargeEventId,
                            MiscellaneousDetailsType = new TransferMiscellaneousDetailsType
                            {
                                DocumentNumber = docNum,
                                //Freetxt2 = "000013241218110101000037132501",
                                Freetxt3 = partTransactionRemarks.Split(50, 4),
                                Freetxt4 = partTransactionRemarks.Split(50, 5)
                            }
                        },
                        new IntraBankTransferTransactionDetail
                        {
                            SerialNumber = IntraBankTransferConstants.SerialNumberCredit,
                            PartTransactionType = IntraBankTransferConstants.PartTransactionTypeCredit,
                            AccountIdentification = creditAccount,
                            TransactionAmount = amount,
                            TransactionCurrency = IntraBankTransferConstants.TransactionCurrency,
                            TransactionValueDate = transactionDate,
                            TransactionParticulars = partTransactionRemarks.Split(50, 1),
                            Rate = IntraBankTransferConstants.Rate,
                            PartTransactionRemarks = partTransactionRemarks.Split(50, 2),
                            ReferenceNumber = referenceNumber.Split(20, 1),
                            PartTransactionRemarks2 = partTransactionRemarks.Split(50, 3),
                            LienDetailsList = new List<LienDetails>(),
                            MiscellaneousDetailsType = new TransferMiscellaneousDetailsType
                            {
                                DocumentNumber = docNum,
                                //Freetxt2 = "000013241218110101000037132501",
                                Freetxt3 = partTransactionRemarks.Split(50, 4),
                                Freetxt4 = partTransactionRemarks.Split(50, 5)
                            }
                        }
                    }
        };
    }

    private static (string?, string?, string?, string?, string?) SplitTransactionRemarks(string partTransactionRemarks)
    {
        // Ensure the string is not null or empty
        if (string.IsNullOrEmpty(partTransactionRemarks))
            return (null, null, null, null, null);

        // Split into chunks of up to 50 characters each
        string part1 = partTransactionRemarks.Length > 0 ? partTransactionRemarks.Substring(0, Math.Min(50, partTransactionRemarks.Length)) : "";
        string part2 = partTransactionRemarks.Length > 50 ? partTransactionRemarks.Substring(50, Math.Min(50, partTransactionRemarks.Length - 50)) : "";
        string part3 = partTransactionRemarks.Length > 100 ? partTransactionRemarks.Substring(100, Math.Min(50, partTransactionRemarks.Length - 100)) : "";
        string part4 = partTransactionRemarks.Length > 150 ? partTransactionRemarks.Substring(150, Math.Min(50, partTransactionRemarks.Length - 150)) : "";
        string part5 = partTransactionRemarks.Length > 200 ? partTransactionRemarks.Substring(200, Math.Min(50, partTransactionRemarks.Length - 200)) : "";

        return (part1, part2, part3, part4, part5);
    }

    public static string MapService(string key)
    {
        // List of services and their corresponding channel IDs
        Dictionary<string, string> serviceMap = new Dictionary<string, string>
        {
            { "GAPS", "GTBGH1" },
            { "737", "GTBGH2" },
            { "GTWORLD", "GTBGH3" },
            { "MYGHPAY", "GTBGH4" },
            { "GTCOLLECT", "GTBGH5" },
            { "IPAY", "GTBGH6" },
            { "EWALLET", "GTBGH7" },
            { "AIRTELTIGO", "GTBGH8" },
            { "VODAFONE", "GTBGH9" },
            { "EXPRESSPAY", "GTBGH10" },
            { "APIHUB", "GTBGH11" },
            { "APISWITCH", "GTBGH12" }
        };
        return serviceMap.TryGetValue(key.ToUpper(), out string value) ? value : "GTBGH100";
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
