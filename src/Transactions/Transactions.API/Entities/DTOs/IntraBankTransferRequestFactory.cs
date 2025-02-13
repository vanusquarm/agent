using System.Globalization;

namespace Transactions.API.Entities.DTOs;

public class IntraBankTransferRequestFactory
{
        public static IntraBankTransferRequest Create(string debitAccount, string creditAccount, decimal amount, string partTransactionRemarks, string referenceNumber,string channel, string? docNum)
        {
            DateTime transactionDate = DateTime.ParseExact("2024-12-24", "yyyy-MM-dd", CultureInfo.InvariantCulture);
            return new IntraBankTransferRequest
            {
                TransactionType = IntraBankTransferConstants.TransactionType,
                TransactionSubType = IntraBankTransferConstants.TransactionSubType,
                ChannelIdentification = IntraBankTransferConstants.ChannelIdentification,
                EntererId = channel,
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
                        ReferenceNumber = referenceNumber,
                        PartTransactionRemarks2 = partTransactionRemarks.Split(50, 3), 
                        LienDetailsList = new List<LienDetails>(),
                            MiscellaneousDetailsType = new TransferMiscellaneousDetailsType
                            {
                                DocumentNumber = docNum
                                //Freetxt2 = "000013241218110101000037132501",
                                //Freetxt3 = "BPS004375435825138052",
                                //Freetxt4 = ""
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
                        ReferenceNumber = referenceNumber,
                        PartTransactionRemarks2 = partTransactionRemarks.Split(50, 3),
                        LienDetailsList = new List<LienDetails>(),
                            MiscellaneousDetailsType = new TransferMiscellaneousDetailsType
                            {
                                DocumentNumber = docNum
                                //Freetxt2 = "000013241218110101000037132501",
                                //Freetxt3 = "BPS004375435825138052",
                                //Freetxt4 = ""
                            }
                    }
                }
            };
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
