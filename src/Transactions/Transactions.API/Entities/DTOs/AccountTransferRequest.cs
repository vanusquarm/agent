namespace Transactions.API.Entities.DTOs;

public class AccountTransferRequest
{
    public string SourceAccountId { get; set; }
    public string DestinationAccountId { get; set; }
    public decimal Amount { get; set; }
    public string Remarks { get; set; }
    public string ChannelReference { get; set; }
    public string ChannelId { get; set; }
    public string? Currency { get; set; }
    public string? DocNum { get; set; }
    public string? UserPartTransactionCode { get; set; }
    public string? ChargeEventId { get; set; }
    public string? BatchId { get; set; }
    public void Trim()
    {
        SourceAccountId = SourceAccountId?.Trim();
        DestinationAccountId = DestinationAccountId?.Trim();
        if (Remarks != null)
        {
            Remarks = Remarks.Trim()
                .Replace("\\", "-")
                .Replace("&", "-")
                .Replace("“", "-")
                .Replace("|", "-");
        }
        ChannelReference = ChannelReference?.Trim();
        ChannelId = ChannelId?.Trim();
        DocNum = DocNum?.Trim();
        UserPartTransactionCode = UserPartTransactionCode?.Trim();
        ChargeEventId = ChargeEventId?.Trim();
        BatchId = BatchId?.Trim();
    }

    public static AccountTransferRequest Map(ChequeDepositRequest chequeDepositRequest)
    {
        return new AccountTransferRequest
        {
            SourceAccountId = chequeDepositRequest.ChequeAccountNumber,
            DestinationAccountId = chequeDepositRequest.BeneficiaryAccountNumber,
            Amount = chequeDepositRequest.Amount,
            Remarks = $"Cheque Deposit: {chequeDepositRequest.ChequeNumber}",
            ChannelReference = chequeDepositRequest.ChequeAccountName,
            ChannelId = "GTCOLLECTION"
        };
    }
}

public class ChequeDepositRequest
{
    public string? ChequeAccountName { get; set; } = string.Empty;
    public required string ChequeAccountNumber { get; set; }
    public required decimal Amount { get; set; }
    public required DateTime ValueDate { get; set; }
    public required string BankName { get; set; }
    public string? BranchCode { get; set; }
    public required string ChequeNumber { get; set; }
    public required string BeneficiaryAccountNumber { get; set; }

    public static ChequeDetails Map(ChequeDepositRequest chequeDepositRequest) 
    { 
        return new ChequeDetails
        {
            InstrumentDate = chequeDepositRequest.ValueDate.ToString("yyyy-MM-dd"),
            InstrumentAlpha = chequeDepositRequest.BankName.ToUpper().Equals("GTB") ? "!" : chequeDepositRequest.BankName.ToUpper().Substring(0, 4),
            InstrumentNumber = chequeDepositRequest.ChequeNumber,
            InstrumentType = "CHQ" // Assuming the type is always a cheque
        };
    }
}

public class BatchTransferRequest
{
    public string SourceAccountId { get; set; }
    public List<CreditTarget> CreditTargets { get; set; } = new();
    public decimal Amount { get; set; }
    public string Remarks { get; set; }
    public string ChannelReference { get; set; }
    public string ChannelId { get; set; }
    public string? Currency { get; set; }
    public string? ExplanationCode { get; set; }
    public string? ChargeEventId { get; set; }
    public string? BatchId { get; set; }
}

public class CreditTarget
{
    public string AccountId { get; set; }
    public decimal Amount { get; set; }
    public string Reference { get; set; }
}