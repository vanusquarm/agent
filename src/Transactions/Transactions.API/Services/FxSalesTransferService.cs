using System.Data;
using Transactions.API.Entities;
using static Transactions.API.Entities.ResponseManager;

namespace Transactions.API.Services
{
    public class FxSalesTransferService
    {
        private readonly ILogger<FxSalesTransferService> _logger;
        //private readonly FxBehaviorProcessor _fxBehaviorProcessor;
        //private readonly AccountUtil _accountUtil;
        //private readonly FxSalesRepo _fxSalesRepo;
        //private readonly TransferProcessBehavior _transferProcessBehavior;
        private readonly ITransactionClient _fxSaleRateRepo;
        //private readonly CurrencyConversionRepo _currencyConversion;
        //private readonly TransferFundRepo _transferFundRepo;
        //static readonly IHttpContextAccessor _httpContextAccessor = new HttpContextAccessor();

        public FxSalesTransferService(ILogger<FxSalesTransferService> logger, 
            //FxBehaviorProcessor fxBehaviorProcessor, AccountUtil accountUtil,
        //FxSalesRepo fxSalesRepo,
        //TransferProcessBehavior transferProcessBehavior,
            ITransactionClient fxSaleRateRepo
        //    CurrencyConversionRepo currencyConversion, TransferFundRepo transferFundRepo
            )
        {
            _logger = logger;
            //_fxBehaviorProcessor = fxBehaviorProcessor;
            //_accountUtil = accountUtil;
            //_fxSalesRepo = fxSalesRepo;
            //_transferProcessBehavior = transferProcessBehavior;
            _fxSaleRateRepo = fxSaleRateRepo;
            //_currencyConversion = currencyConversion;
            //_transferFundRepo = transferFundRepo;
        }

        //public async Task<ResponseWithStatusCode> ActionFxTransfer(FxSalesRequest req)
        //{
        //    // Get header information
        //    var getheaders = GenericRequestMapper.getHeaderInformation();
        //    var header = await GenericRequestMapper.generateHeaderRequest(getheaders);
        //    var appsettings = ConfigSettings.AppSettings;

        //    var finacleTransferRequest = new CreateTransferRequest();

        //    // Check if it is above the cut off time
        //    int cutOffTime = await _fxBehaviorProcessor.GetFxSalesCutOffTime();
        //    if (cutOffTime == 0)
        //        return ResponseManager.ReturnFailed("This service is currently unavailable now");

        //    _logger.LogInformation("RequestId: {RequestId}. About to get account details for debitAccount: {DebitAcoount} and creditAccount: {CreditAccount} with Amount {Amount}", header.RequestId, req.DebitAccount, req.CreditAccount, req.Amount);
        //    var accountDetails = (AccountDetailsResponse)_httpContextAccessor.HttpContext.Items["AccountDetails"];
        //    if (accountDetails == null)
        //    {
        //        return ResponseManager.ReturnFailed("Account details not found in the context.");
        //    }
        //    _logger.LogInformation("RequestId: {RequestId} Back from getting account details with response: {@DebitDetails} and {@CreditDetails}", header.RequestId, accountDetails.DebitAccountDetails, accountDetails.CreditAccountDetails);

        //    // Get the TransactionParticulars
        //    string transactionParticulars = _transferProcessBehavior.GetTransactionParticulars(header.ChannelId, req?.Remark, header.AuthMode);

        //    // Get the Explanation code           
        //    int transactionCode = _transferProcessBehavior.GetTransactionCode(header.ChannelId, req.Purpose);

        //    // Generate Fx Remarks
        //    req.Remark = $"{req.Remark?.Trim()} FXSALE from {accountDetails?.DebitAccountDetails?.accountCurrency} to {accountDetails?.CreditAccountDetails?.accountCurrency}";

        //    // Perform FXSales Transfer
        //    _logger.LogInformation("RequestId: {RequestId} About to Map request to finacle.", header.RequestId);

        //    if (!string.IsNullOrEmpty(req.MidCurrency))
        //    {
        //        finacleTransferRequest = FxRequestMapper.generateThirdCurrencyFxSalesFinacleTransferRequest(req, transactionCode, header.RequestId);
        //    }

        //    var channelToExemptFromRateCodeCheck = ConfigSettings.FxSalesConfig.ChannelToExemptFromRateCodeCheck;
        //    if (!channelToExemptFromRateCodeCheck.Split(',').Contains(header.ChannelId.ToUpper()))
        //    {
        //        // Validate FxSales
        //        var validateResponse = _transferProcessBehavior.ValidateFxSales(accountDetails?.DebitAccountDetails?.accountCurrency, accountDetails?.CreditAccountDetails?.accountCurrency);
        //        if (!string.IsNullOrEmpty(validateResponse))
        //        {
        //            return ResponseManager.ReturnFailed(validateResponse);
        //        }

        //        // Adjust FxSale Rate Code
        //        var rateCodeResponse = _transferProcessBehavior.MapRateCode(accountDetails?.DebitAccountDetails?.accountCurrency, accountDetails?.CreditAccountDetails?.accountCurrency, req.RateCode);
        //        req.RateCode = rateCodeResponse;
        //    }

        //    finacleTransferRequest = FxRequestMapper.generateFxSalesFinacleTransferRequest(req, transactionCode, header.RequestId);
        //    _logger.LogInformation("RequestId: {RequestId} About to Call Finacle endpoint to make transfer with request: {Request}", header.RequestId, finacleTransferRequest);
        //    var finacleHeader = new FinacleHeaderRequest() { RequestId = header.RequestId, Channel = header.ChannelId, DeliveryChannel = ConfigSettings.FinacleAPIDetails.DeliveryChannel };

        //    var createTransferResponse = await _transferFundRepo.CreateFundsTransfer(finacleTransferRequest, finacleHeader);
        //    _logger.LogInformation("RequestId: {RequestId} Back from Calling Finacle endpoint to make transfer with response: {@Response}", header.RequestId, createTransferResponse);
        //    _ = await _fxSalesRepo.PatchFxSales(createTransferResponse, header.RequestId, ResponseManager.Success.Item1);
        //    return ResponseManager.ReturnSuccess(createTransferResponse);
        //}

        public async Task<ResponseWithStatusCode> GetFxRates(RatesRequestModel model)
        {
            // Get header information
            //var getheaders = GenericRequestMapper.getHeaderInformation();
            //var header = await GenericRequestMapper.generateHeaderRequest(getheaders);

            var rateCodeString = string.Join(",", model.RateCodes.Select(x => $"'{x}'"));
            var fxRateResponse = await _fxSaleRateRepo.GetFxRates(rateCodeString, "header.RequestId");
            return ResponseManager.ReturnSuccess(fxRateResponse);
        }

        //public async Task<ResponseWithStatusCode> ConvertCurrency(CurrencyConversionReqVM request)
        //{
        //    // Get header information
        //    var getheaders = GenericRequestMapper.getHeaderInformation();
        //    var header = await GenericRequestMapper.generateHeaderRequest(getheaders);
        //    _logger.LogInformation("RequestId: {RequestId}. Extracted header: {@Header}", header.RequestId, header);

        //    // Convert Currency
        //    var channelToExemptFromRateCodeCheck = ConfigSettings.FxSalesConfig.ChannelToExemptFromRateCodeCheck;
        //    if (!channelToExemptFromRateCodeCheck.Split(',').Contains(header.ChannelId.ToUpper()))
        //    {
        //        request.rateCode = _transferProcessBehavior.MapRateCode(request.fromCurrencyCode, request.toCurrencyCode, request.rateCode);
        //    }
        //    var currencyConvertRequest = FxRequestMapper.generateCurrencyConversionRequestToFinacle(request);
        //    var rateConversionResponse = await _currencyConversion.ConvertCurrency(currencyConvertRequest, header.RequestId);
        //    return ResponseManager.ReturnSuccess(rateConversionResponse);
        //}
    }
}
