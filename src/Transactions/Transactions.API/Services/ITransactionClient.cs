using Microsoft.AspNetCore.Mvc;
using Transactions.API.Entities.DTOs;

namespace Transactions.API.Services;
using Refit;

public interface ITransactionClient
{
    [Post("/v2/transactiondetails/gettransactiondetails")]
    Task<object> GetTransactionDetails(object request);
    [Post("/v2/transactions/cash-deposit")]
    Task<CashDepositResponse> PerformCashDeposit(CashDepositRequest cashDepositRequest);
    [Post("/v2/transactions/cash-withdrawal")]
    Task<CashWithdrawalResponse> PerformCashWithdrawal(CashWithdrawalRequest cashWithdrawalRequest);
    //[Post("/v2/transactions/{transactionReference}/tranDate/{transactionDate}")]
    //Task<TransactionDetailsResponse> GetTransactionDetails(string customerReference, DateTime transactionDate, [Query] string accountIdentification);
    [Post("/v2/transactions/INTRA-BANK-FUND-TRANSFER")]
    Task<IntraBankTransferResponse> PerformIntraBankTransfer([FromBody] IntraBankTransferRequest intraBankTransferRequest);

}