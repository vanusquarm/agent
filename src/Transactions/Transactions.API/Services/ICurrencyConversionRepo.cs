using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Transactions.API.Entities;

namespace Transactions.API.Services
{
    public class CurrencyConversionRepo 
    {
        //private readonly ILogger<CurrencyConversionRepo> _logger;
        //private readonly IClientService _finacleClientService;
        //private readonly JsonSerializerSettings jsonSerializeSettings = new JsonSerializerSettings
        //{
        //    ContractResolver = new CamelCasePropertyNamesContractResolver(),
        //    NullValueHandling = NullValueHandling.Ignore
        //};

        //public CurrencyConversionRepo(ILogger<CurrencyConversionRepo> logger, IClientService finacleClientService)
        //{
        //    _logger = logger;
        //    _finacleClientService = finacleClientService;
        //}

        //public async Task<CurrencyConversionResVM> ConvertCurrency(CurrencyConversionReqDTO request, string requestId)
        //{
        //    var _convertCurrencyEndpoint = ConfigSettings.FinacleAPIDetails.CurrencyConversion;



        //    _logger.LogInformation("RequestId: {RequestId}. Back from getting bearer Token", requestId);

        //    var serializedRequestBody = JsonConvert.SerializeObject(request, jsonSerializeSettings);
        //    var finacleRequest = new FinacleClientRequest
        //    {
        //        endpoint = _convertCurrencyEndpoint,
        //        serializedRequestBody = serializedRequestBody,
        //        BearerToken = bearerToken
        //    };

        //    var logRequest = GenericRequestMapper.generateLogWritingRequest(_convertCurrencyEndpoint, serializedRequestBody);
        //    //Preparing to call currency conversion endpoint from Finacle
        //    _logger.LogInformation("RequestId: {RequestId}. About to Convert currency with request: {@Request}", requestId, logRequest);
        //    var response = await _finacleClientService.CreateApiRequest(HttpVerb.POST, finacleRequest);
        //    string jsonResponse = await response.Content.ReadAsStringAsync();
        //    _logger.LogInformation("RequestId: {RequestId}. Back from Converting currency with response: {@Response}", requestId, jsonResponse);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        // Throw custom exception with finacle error response
        //        var ex = new FinacleApiCallNotSuccessfulException();
        //        try
        //        {
        //            var finacleErrorResponse = JsonConvert.DeserializeObject<FinacleErrorResponse>(jsonResponse, jsonSerializeSettings);
        //            ex.Data.Add(nameof(CustomProblemDetails.ResponseCode), ResponseManager.FailedValidation.Item1);
        //            ex.Data.Add(nameof(FinacleErrorResponse), finacleErrorResponse);
        //        }
        //        catch (Exception)
        //        {
        //            ex.Data.Add(nameof(CustomProblemDetails.ResponseCode), ResponseManager.FinacleFailure.Item1);
        //        }
        //        throw ex;
        //    }
        //    var currencyConversionResponse = JsonConvert.DeserializeObject<CurrencyConversionResVM>(jsonResponse, jsonSerializeSettings);
        //    var outObject = new OutputAmountResVM();
        //    if (currencyConversionResponse.outputAmount is string)
        //    {
        //        outObject.currency = null;
        //        outObject.value = Convert.ToDecimal(currencyConversionResponse.outputAmount);
        //        currencyConversionResponse.outputAmount = outObject;
        //    }

        //    if (currencyConversionResponse.outputAmount is object)
        //    {
        //        var currencyConversionResponse1 = JsonConvert.DeserializeObject<CurrencyConversionResVMDto>(jsonResponse, jsonSerializeSettings);
        //        outObject.currency = currencyConversionResponse1.outputAmount.ccy;
        //        outObject.value = currencyConversionResponse1.outputAmount.value;
        //        currencyConversionResponse.outputAmount = outObject;
        //    }
        //    return currencyConversionResponse;
        //}
    }


}