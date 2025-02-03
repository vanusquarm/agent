using Microsoft.AspNetCore.Mvc;
using Transactions.API.Entities.DTOs;
using IntraBankTransferRequest = Finacle.Core.Domain.IntraBankTransferRequest;

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
    [Post("/v2/transactions/INTRA-BANK-FUND-TRANSFER")]
    Task<IntraBankTransferResponse> PerformIntraBankTransfer([FromBody] IntraBankTransferRequest intraBankTransferRequest);
    [Post("/v2/eodStatusDtls/geteodStatusDtls")]
    Task<EodStatusResponse> GetEodStatusDetails(EodStatusRequest request);
}