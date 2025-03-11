using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Transactions.API.Entities;

namespace Transactions.API.Services
{
    public class TransferFundRepo
    {
    //    private readonly ILogger<TransferFundRepo> _logger;
    //    private readonly ITransactionClient _finacleClientService;
    //    private readonly JsonSerializerSettings jsonSerializeSettings = new JsonSerializerSettings
    //    {
    //        ContractResolver = new CamelCasePropertyNamesContractResolver(),
    //        NullValueHandling = NullValueHandling.Ignore
    //    };
    //    private readonly IInquiryRepo _inquiryRepo;
    //    private readonly IFraudSnipperRepo _fraudSnipperRepo;
    //    public TransferFundRepo(ILogger<TransferFundRepo> logger, ITransactionClient finacleClientService, IInquiryRepo inquiryRepo, IFraudSnipperRepo fraudSnipperRepo)
    //    {
    //        _logger = logger;
    //        _finacleClientService = finacleClientService;
    //        _bearerToken = bearerToken;
    //        _inquiryRepo = inquiryRepo;
    //        _fraudSnipperRepo = fraudSnipperRepo;
    //    }

    //    public async Task<CreateTransferResponse> CreateFundsTransfer(CreateTransferRequest req, FinacleHeaderRequest reqHeader)
    //    {
    //        var finacleAppSettings = ConfigSettings.FinacleAPIDetails;
    //        var response = new CreateTransferResponse();
    //        var _postFundsTransfer = finacleAppSettings.FundsTransfer;

    //        _logger.LogInformation("RequestId: {RequestId} About to get bearer Token", reqHeader.RequestId);
    //        var bearerToken = await _bearerToken.GetBearerToken(reqHeader.RequestId);
    //        _logger.LogInformation("RequestId: {RequestId}. Back from getting bearer Token", reqHeader.RequestId);

    //        var serializedRequestBody = JsonConvert.SerializeObject(req, jsonSerializeSettings);
    //        var request = new FinacleClientRequest
    //        {
    //            endpoint = _postFundsTransfer,
    //            serializedRequestBody = serializedRequestBody,
    //            delivery_channel = finacleAppSettings.DeliveryChannel,
    //            duptranchkreqd = finacleAppSettings.duptranchkreqdHeader,
    //            requesttype = finacleAppSettings.requesttypeHeader,
    //            requestreference = reqHeader.RequestId,
    //            BearerToken = bearerToken
    //        };

    //        if (!string.IsNullOrEmpty(reqHeader.SolId))
    //        {
    //            request.context_sol_id = reqHeader.SolId;
    //        }

    //        var logRequest = GenericRequestMapper.generateLogWritingRequest(_postFundsTransfer, serializedRequestBody, finacleAppSettings.DeliveryChannel, request.duptranchkreqd, request.requesttype, request.requestreference, request.context_sol_id);
    //        _logger.LogInformation("RequestId: {RequestId}. About to post transaction with request: {@Request}", reqHeader.RequestId, logRequest);
    //        var transferResponse = await _finacleClientService.CreateApiRequest(HttpVerb.POST, request);
    //        var jsonResponse = await transferResponse.Content.ReadAsStringAsync();
    //        _logger.LogInformation("RequestId: {RequestId}. Back from posting transaction with response: {Response}", reqHeader.RequestId, jsonResponse);

    //        if (!transferResponse.IsSuccessStatusCode)
    //        {
    //            // Throw custom exception with finacle error response
    //            var ex = new FinacleApiCallNotSuccessfulException();
    //            try
    //            {
    //                var finacleErrorResponse = JsonConvert.DeserializeObject<FinacleErrorResponse>(jsonResponse, jsonSerializeSettings);
    //                ex.Data.Add(nameof(CustomProblemDetails.ResponseCode), ResponseManager.FailedValidation.Item1);
    //                ex.Data.Add(nameof(FinacleErrorResponse), finacleErrorResponse);
    //            }
    //            catch (Exception)
    //            {
    //                var transactionInquiryRequest = new CreateTransactionDetailsRequest
    //                {
    //                    accountId = req.transferTransactionDetail.First().accountIdentification,
    //                    fromDate = req.transferTransactionDetail.First().transactionValueDate,
    //                    toDate = req.transferTransactionDetail.First().transactionValueDate,
    //                    tranRefNumber = request.requestreference
    //                };
    //                var transactionInquiryResponse = await _inquiryRepo.GetTransactionStatus(transactionInquiryRequest, reqHeader);
    //                if (string.IsNullOrEmpty(transactionInquiryResponse.errorMsg))
    //                {
    //                    response.TransactionDate = transactionInquiryResponse.tranDetails.FirstOrDefault().transactionDate;
    //                    response.TransactionReference = transactionInquiryResponse.tranDetails.FirstOrDefault().transactionIdentification;
    //                    return response;
    //                }
    //                if (reqHeader.IsFraudSnipperValidated) { Task.Run(() => _fraudSnipperRepo.FinalizeFraudSnipper(reqHeader.TransactionId, reqHeader.RequestId)); }
    //                ex.Data.Add(nameof(CustomProblemDetails.ResponseCode), ResponseManager.FinacleFailure.Item1);
    //            }
    //            throw ex;
    //        }
    //        return JsonConvert.DeserializeObject<CreateTransferResponse>(jsonResponse, jsonSerializeSettings);
    //    }

    //    public async Task<CreateTransferResponse> CreateCashWithdrawalTransfer(CreateCashTransferRequest req, FinacleHeaderRequest reqHeader)
    //    {
    //        var finacleAppSettings = ConfigSettings.FinacleAPIDetails;
    //        var _postFundsTransfer = finacleAppSettings.CashWithdrawal;

    //        _logger.LogInformation("RequestId: {RequestId} About to get bearer Token", reqHeader.RequestId);
    //        var bearerToken = await _bearerToken.GetBearerToken(reqHeader.RequestId);
    //        _logger.LogInformation("RequestId: {RequestId} Back from getting bearer Token", reqHeader.RequestId);

    //        var serializedRequestBody = JsonConvert.SerializeObject(req, jsonSerializeSettings);
    //        var request = new FinacleClientRequest
    //        {
    //            endpoint = _postFundsTransfer,
    //            serializedRequestBody = serializedRequestBody,
    //            //idempotencykey = reqHeader.RequestId,
    //            context_user_id = reqHeader.SupervisorsId,
    //            duptranchkreqd = finacleAppSettings.duptranchkreqdHeader,
    //            requesttype = finacleAppSettings.requesttypeHeader,
    //            requestreference = reqHeader.RequestId,
    //            BearerToken = bearerToken,
    //            context_sol_id = reqHeader.SolId,
    //            delivery_channel = ConfigSettings.FinacleAPIDetails.DeliveryChannel,
    //        };

    //        if (reqHeader.AcceptException) { request.delivery_channel = ConfigSettings.FinacleAPIDetails.DeliveryChannel; }

    //        var logRequest = GenericRequestMapper.generateLogWritingRequest(_postFundsTransfer, serializedRequestBody, finacleAppSettings.DeliveryChannel, request.duptranchkreqd, request.requesttype, request.requestreference, request.context_sol_id, request.context_user_id);
    //        _logger.LogInformation("RequestId: {RequestId} About to post Cash withdrawal transaction with request: {@Request}", reqHeader.RequestId, logRequest);
    //        var trasnferResponse = await _finacleClientService.CreateApiRequest(HttpVerb.POST, request);
    //        var jsonResponse = await trasnferResponse.Content.ReadAsStringAsync();
    //        _logger.LogInformation("RequestId: {RequestId} Back from posting cash withdrawal transaction with response: {Response}", reqHeader.RequestId, jsonResponse);

    //        if (!trasnferResponse.IsSuccessStatusCode)
    //        {
    //            // Throw custom exception with finacle error response
    //            var ex = new FinacleApiCallNotSuccessfulException();
    //            try
    //            {
    //                var finacleErrorResponse = JsonConvert.DeserializeObject<FinacleErrorResponse>(jsonResponse, jsonSerializeSettings);
    //                ex.Data.Add(nameof(CustomProblemDetails.ResponseCode), ResponseManager.FailedValidation.Item1);
    //                ex.Data.Add(nameof(FinacleErrorResponse), finacleErrorResponse);
    //            }
    //            catch (Exception)
    //            {
    //                ex.Data.Add(nameof(CustomProblemDetails.ResponseCode), ResponseManager.FinacleFailure.Item1);
    //            }
    //            throw ex;
    //        }
    //        return JsonConvert.DeserializeObject<CreateTransferResponse>(jsonResponse, jsonSerializeSettings);
    //    }

    //    public async Task<CreateTransferResponse> CreateCashDepositTransfer(CreateCashTransferRequest req, FinacleHeaderRequest reqHeader)
    //    {
    //        var finacleAppSettings = ConfigSettings.FinacleAPIDetails;
    //        var _postFundsTransfer = finacleAppSettings.CashDeposit;

    //        _logger.LogInformation("RequestId: {RequestId} About to get bearer Token", reqHeader.RequestId);
    //        var bearerToken = await _bearerToken.GetBearerToken(reqHeader.RequestId);
    //        _logger.LogInformation("RequestId: {RequestId} Back from getting bearer Token", reqHeader.RequestId);

    //        var serializedRequestBody = JsonConvert.SerializeObject(req, jsonSerializeSettings);
    //        var request = new FinacleClientRequest
    //        {
    //            endpoint = _postFundsTransfer,
    //            serializedRequestBody = serializedRequestBody,
    //            //idempotencykey = reqHeader.RequestId,
    //            context_user_id = reqHeader.SupervisorsId,
    //            duptranchkreqd = finacleAppSettings.duptranchkreqdHeader,
    //            requesttype = finacleAppSettings.requesttypeHeader,
    //            requestreference = reqHeader.RequestId,
    //            BearerToken = bearerToken,
    //            context_sol_id = reqHeader.SolId,
    //            delivery_channel = ConfigSettings.FinacleAPIDetails.DeliveryChannel,
    //        };

    //        if (reqHeader.AcceptException) { request.delivery_channel = finacleAppSettings.DeliveryChannel; }

    //        var logRequest = GenericRequestMapper.generateLogWritingRequest(_postFundsTransfer, serializedRequestBody, finacleAppSettings.DeliveryChannel, request.duptranchkreqd, request.requesttype, request.requestreference, request.context_sol_id, request.context_user_id);
    //        _logger.LogInformation("RequestId: {RequestId} About to post Cash deposit transaction with request: {@Request}", reqHeader.RequestId, logRequest);
    //        var trasnferResponse = await _finacleClientService.CreateApiRequest(HttpVerb.POST, request);
    //        var jsonResponse = await trasnferResponse.Content.ReadAsStringAsync();
    //        _logger.LogInformation("RequestId: {RequestId} Back from posting cash deposit transaction with response: {Response}", reqHeader.RequestId, jsonResponse);

    //        if (!trasnferResponse.IsSuccessStatusCode)
    //        {
    //            // Throw custom exception with finacle error response
    //            var ex = new FinacleApiCallNotSuccessfulException();
    //            try
    //            {
    //                var finacleErrorResponse = JsonConvert.DeserializeObject<FinacleErrorResponse>(jsonResponse, jsonSerializeSettings);
    //                ex.Data.Add(nameof(CustomProblemDetails.ResponseCode), ResponseManager.FailedValidation.Item1);
    //                ex.Data.Add(nameof(FinacleErrorResponse), finacleErrorResponse);
    //            }
    //            catch (Exception)
    //            {
    //                ex.Data.Add(nameof(CustomProblemDetails.ResponseCode), ResponseManager.FinacleFailure.Item1);
    //            }
    //            throw ex;
    //        }
    //        return JsonConvert.DeserializeObject<CreateTransferResponse>(jsonResponse, jsonSerializeSettings);
    //    }
    }

}