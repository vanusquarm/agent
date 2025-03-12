using Microsoft.AspNetCore.Mvc;
using Transactions.API.Entities.DTOs;

namespace Transactions.API.Services;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transactions.API.Entities;

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
        "requesttype: SRV"
    )]
    [Post("/v2/transactions/INTRA-BANK-FUND-TRANSFER")]
    Task<object> PerformIntraBankTransfer(
        [Header("requestreference")] string requestReference,
        [Body] IntraBankTransferRequest intraBankTransferRequest);


    [Post("/v2/createCustomCharge/createCustomChargeDtls")]
    Task<object> PerformCharge(
        [Body] ChargeDetailsDto intraBankTransferRequest);



    [Headers(
        "delivery_channel: API",
        "duptranchkreqd: Y",
        "requesttype: SRV"
    )]
    [Post("/v2/savings-accounts:fundsTransferReversal")]
    Task<object> Reverse([Header("requestreference")] string requestReference,[Body] object reversalTransferRequest);

    [Post("/v2/bankBranchBodDateDtls/getbankBranchBodDateDtls")]
    Task<BankDateDto> GetTransactionBankDate([Body] Dictionary<string, string> request);


    [Post("/v2/FXRateDtls/getFXRateDtlssl")]
    Task<FxSaleRateResponse> GetFxRates(string rateCodeString, string requestId);

    [Post("/v2/FXRateDtls/getFXRateDtlssl")]
    Task<FxSaleRateResponse> GetFxRates(RatesRequestModel rateCodeString);

    [Post("/v2/eventIdchargeDtls/geteventIdchargeDtls")]
    Task<object> ChargeInquiry(ChargeInquiryRequest request);

    //"GetFxSaleRates": "v2/FXRateDtls/getFXRateDtlss",
    //"CurrencyConversion": "v2/currencies/CONVERT",
    //"TransactionStatusQuery": "v2/transactiondetails/gettransactiondetails",
    //"GetOfficeAccountDetails": "v2/officeAcctDtls/getOfficeAcctDtls",
    //"StatementChargeEndpoint": "v2/createStatementCharge/createStatementChargeDtls",
    //"GetAccountType": "v2/getAccntType/getAcType",
    //"VirtualAccountInquiry": "v1/virtual-accounts/{VirtualAccountIdentification}",
    //"VirtualAccountTransferEndpoint": "v1/virtual-accounts-transactions"
}