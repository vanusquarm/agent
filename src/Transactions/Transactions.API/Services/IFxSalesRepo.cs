using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Transactions.API.Entities;

namespace Transactions.API.Services
{
    public class FxSaleRateRepo
    {
    //    private readonly ILogger<FxSaleRateRepo> _logger;
    //    private readonly IClientService _finacleClientService;
    //    private readonly IGetBearerRepo _bearerToken;
    //    private readonly JsonSerializerSettings jsonSerializeSettings = new JsonSerializerSettings
    //    {
    //        ContractResolver = new CamelCasePropertyNamesContractResolver(),
    //        NullValueHandling = NullValueHandling.Ignore
    //    };
    //    public FxSaleRateRepo(IClientService finacleClientService, ILogger<FxSaleRateRepo> logger, IGetBearerRepo bearerToken)
    //    {
    //        _finacleClientService = finacleClientService;
    //        _logger = logger;
    //        _bearerToken = bearerToken;
    //    }


    //    public async Task<FxSaleRateResponse> GetFxRates(string rateCodes, string reqId)
    //    {
    //        _logger.LogInformation("RequestId: {RequestId} About to get bearer Token", reqId);
    //        var bearerToken = await _bearerToken.GetBearerToken(reqId);
    //        _logger.LogInformation("RequestId: {RequestId} Back from getting bearer Token", reqId);
    //        var payload = JsonConvert.SerializeObject(new { RateCode = rateCodes });

    //        var req = new FinacleClientRequest
    //        {
    //            BearerToken = bearerToken,
    //            serializedRequestBody = payload,
    //            endpoint = ConfigSettings.FinacleAPIDetails.GetFxSaleRates,
    //        };

    //        var logRequest = GenericRequestMapper.generateLogWritingRequest(ConfigSettings.FinacleAPIDetails.GetFxSaleRates, payload);
    //        _logger.LogInformation("RequestId: {RequestId}. About to post transaction with request: {@Request}", reqId, logRequest);
    //        var fxRateResponse = await _finacleClientService.CreateApiRequest(HttpVerb.POST, req);
    //        var jsonResponse = await fxRateResponse.Content.ReadAsStringAsync();
    //        _logger.LogInformation("RequestId: {RequestId}. Back from posting transaction with response: {Response}", reqId, jsonResponse);

    //        if (!fxRateResponse.IsSuccessStatusCode)
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

    //        var postFxResponseDto = JsonConvert.DeserializeObject<FxSaleRateResponse>(jsonResponse, jsonSerializeSettings);

    //        if (postFxResponseDto.errorFlg == FinacleConstant.ERRORFLAG_NO)
    //        {
    //            return JsonConvert.DeserializeObject<FxSaleRateResponse>(jsonResponse, jsonSerializeSettings);
    //        }

    //        // Throw custom exception with finacle error response
    //        var exception = new FinacleApiCallNotSuccessfulException();
    //        var customFinacleErrorResponse = new FinacleErrorResponse() { ErrorDetails = [new FinacleError() { ErrorMessage = postFxResponseDto.errorMsg ?? "Invalid Response From DataSource" }] };
    //        exception.Data.Add(nameof(FinacleErrorResponse), customFinacleErrorResponse);
    //        throw exception;
    //    }
    }

}