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
    Task<GetTransactionsResponse> GetTransactionDetails(object request);

    [Post("/v2/eodStatusDtls/geteodStatusDtls")]
    Task<EodStatusResponse> GetEodStatusDetails(EodStatusRequest request);

    [Headers("delivery_channel: COR")]
    [Post("/v2/transactions/cash-deposit")]
    Task<CashTransferResponse> PerformCashDeposit(
        CashTransferRequest cashDepositRequest,
        [Header("context_user_id")] string tellerId,
        [Header("context_sol_id")] string solId);

    [Headers("delivery_channel: COR")]
    [Post("/v2/transactions/cash-withdrawal")]
    Task<CashTransferResponse> PerformCashWithdrawal(
        CashTransferRequest cashWithdrawalRequest,
        [Header("context_user_id")] string tellerId,
        [Header("context_sol_id")] string solId);

    [Headers(
        "delivery_channel: API",
        "duptranchkreqd: Y",
        "requesttype: SRV"
    )]
    [Post("/v2/transactions/INTRA-BANK-FUND-TRANSFER")]
    Task<IntraBankTransferResponse> PerformIntraBankTransfer(
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
    [Post("/v2/current-accounts:fundsTransferReversal")]
    Task<ReversalTransferResponse> Reverse([Header("requestreference")] string requestReference,[Body] ReversalTransferRequest reversalTransferRequest);

    [Post("/v2/bankBranchBodDateDtls/getbankBranchBodDateDtls")]
    Task<BankDateDto> GetTransactionBankDate([Body] Dictionary<string, string> request);


    [Post("/v2/FXRateDtls/getFXRateDtlssl")]
    Task<FxSaleRateResponse> GetFxRates(string rateCodeString, string requestId);

    [Post("/v2/FXRateDtls/getFXRateDtlssl")]
    Task<FxSaleRateResponse> GetFxRates(RatesRequestModel rateCodeString);

    [Post("/v2/eventIdchargeDtls/geteventIdchargeDtls")]
    Task<object> ChargeInquiry(ChargeInquiryRequest request);

    //Do currency conversion
    [Post("/v2/currencies/CONVERT")]
    Task<object> ConvertCurrency(object request);

    [Post("/v2/createStatementCharge/createStatementChargeDtls")]
    Task<object> PerformStatementCharge(StatementChargeRquest chargeRequest);


    //"GetFxSaleRates": "v2/FXRateDtls/getFXRateDtlss",
    //"CurrencyConversion": "v2/currencies/CONVERT",
    //"TransactionStatusQuery": "v2/transactiondetails/gettransactiondetails",
    //"GetOfficeAccountDetails": "v2/officeAcctDtls/getOfficeAcctDtls",
    //"StatementChargeEndpoint": "v2/createStatementCharge/createStatementChargeDtls",
    //"GetAccountType": "v2/getAccntType/getAcType",
    //"VirtualAccountInquiry": "v1/virtual-accounts/{VirtualAccountIdentification}",
    //"VirtualAccountTransferEndpoint": "v1/virtual-accounts-transactions"
}