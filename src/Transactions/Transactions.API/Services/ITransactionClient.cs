using Microsoft.AspNetCore.Mvc;
using Transactions.API.Entities.DTOs;

namespace Transactions.API.Services;
using Refit;
using System.Threading.Tasks;

public interface ITransactionClient
{
    [Post("/v2/transactiondetails/gettransactiondetails")]
    Task<object> GetTransactionDetails(object request);

    [Post("/v2/eodStatusDtls/geteodStatusDtls")]
    Task<EodStatusResponse> GetEodStatusDetails(EodStatusRequest request);

    [Headers(
        "context_sol_id: 201",
        "delivery_channel: COR",
        "context_user_id: GTBGH2"
    )]
    [Post("/v2/transactions/cash-deposit")]
    Task<CashDepositResponse> PerformCashDeposit(CashDepositRequest cashDepositRequest);

    [Headers(
        "context_sol_id: 201",
        "delivery_channel: COR",
        "context_user_id: GTBGH2"
    )]
    [Post("/v2/transactions/cash-withdrawal")]
    Task<CashWithdrawalResponse> PerformCashWithdrawal(CashWithdrawalRequest cashWithdrawalRequest);

    [Headers(
        "delivery_channel: API",
        "duptranchkreqd: Y",
        "requesttype: SRV",
        "requestreference: 00080280778578878882479904"
    )]
    [Post("/v2/transactions/INTRA-BANK-FUND-TRANSFER")]
    Task<IntraBankTransferResponse> PerformIntraBankTransfer([Body] IntraBankTransferRequest intraBankTransferRequest);
}