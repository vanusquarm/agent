using Transactions.API.Entities.DTOs;

namespace Transactions.API.Services;
using Refit;

public interface ITransactionClient
{

        [Post("/api/v2/transactions/cash-deposit")]
        Task<CashDepositResponse> PerformCashDeposit(CashDepositRequest cashDepositRequest);
        [Post("/api/v2/transactions/cash-withdrawal")]
        Task<CashWithdrawalResponse> PerformCashWithdrawal(CashWithdrawalRequest cashWithdrawalRequest);
        [Post("/api/v2/transactions/{customerReference}/tranDate/{transactionDate}")]
        Task<TransactionDetailsResponse> GetTransactionDetails(string customerReference, DateTime transactionDate, [Query] string accountIdentification);
        [Post("/api/v2/transactions/INTRA-BANK-FUND-TRANSFER")]
        Task<IntraBankTransferResponse> PerformIntraBankTransfer(IntraBankTransferRequest intraBankTransferRequest);
        [Post("/api/v2/transactions")]
        Task<TransactionQueryResponse> GetTransactions([Query] string accountIdentification, [Query] string transactionFromDate, [Query] string transactionToDate, [Query] decimal amountRangeFrom, [Query] decimal amountRangeTo);

}