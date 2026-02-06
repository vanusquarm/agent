namespace Finacle.Core.Domain
{
    public class CreateIntraBankTransferRequest
    {
        public string TransactionType { get; set; }
        public string TransactionSubType { get; set; }
        public string ChannelIdentification { get; set; }
        public TransferTransactionDetailDto[] TransferTransactionDetail { get; set; }
        public string EntererId { get; set; }
    }

    public class TransferTransactionDetailDto
    {
        public string SerialNumber { get; set; }
        public string PartTransactionType { get; set; }
        public string AccountIdentification { get; set; }
        public decimal TransactionAmount { get; set; }
        public string TransactionCurrency { get; set; }
        public string TransactionValueDate { get; set; }
        public string TransactionParticulars { get; set; }
        public decimal Rate { get; set; }
        public string PartTransactionRemarks { get; set; }
        public string ReferenceNumber { get; set; }
        public MiscellaneousDetailsDto MiscellaneousDetailsType { get; set; }
        public string PartTransactionRemarks2 { get; set; }
        public object[] LienDetailsList { get; set; }
    }

    public class MiscellaneousDetailsDto
    {
        public string DocumentNumber { get; set; }
        public string Freetxt2 { get; set; }
        public string Freetxt3 { get; set; }
        public string Freetxt4 { get; set; }
    }

    public class TransactionResponseDto
    {
        public string TransactionDate { get; set; }
        public string TransactionReference { get; set; }
        public string TranStatus { get; set; }
    }

}