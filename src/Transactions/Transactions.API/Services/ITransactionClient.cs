using Microsoft.AspNetCore.Mvc;
using Transactions.API.Entities.DTOs;

namespace Transactions.API.Services;

using Microsoft.AspNetCore.Mvc;
using Refit;

public interface ITransactionClient
{

        [Post("/v2/transactions/cash-deposit")]
        Task<CashDepositResponse> PerformCashDeposit(CashDepositRequest cashDepositRequest);
        [Post("/v2/transactions/cash-withdrawal")]
        Task<CashWithdrawalResponse> PerformCashWithdrawal(CashWithdrawalRequest cashWithdrawalRequest);
        [Post("/v2/transactions/{customerReference}/tranDate/{transactionDate}")]
        Task<TransactionDetailsResponse> GetTransactionDetails(string customerReference, DateTime transactionDate, [Query] string accountIdentification);
        [Post("/v2/transactions/INTRA-BANK-FUND-TRANSFER")]
        Task<IntraBankTransferResponse> PerformIntraBankTransfer([FromBody] IntraBankTransferRequest intraBankTransferRequest);
        [Post("/v2/transactions")]
        Task<TransactionQueryResponse> GetTransactions([Query] string accountIdentification, [Query] string transactionFromDate, [Query] string transactionToDate, [Query] decimal amountRangeFrom, [Query] decimal amountRangeTo);

}