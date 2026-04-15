using Finacle.Infrastructure.Logging;
using Finacle.Infrastructure.Persistence.Repositories;
using Finacle.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Transactions.API.Entities.DTOs;
using Transactions.API.Validators;

namespace Transactions.API.Services
{
    public class TransferOrchestrator
    {
        private readonly ILogger<TransferOrchestrator> _logger;
        private readonly ITransferRepository _repository;
        private readonly ITransactionClient _coreClient;

        public TransferOrchestrator(
            ILogger<TransferOrchestrator> logger,
            ITransferRepository repository,
            ITransactionClient coreClient)
        {
            _logger = logger;
            _repository = repository;
            _coreClient = coreClient;
        }

        public async Task<BankDateDto> GetTransactionBankDate(Dictionary<string, string> request)
        {
            return await _coreClient.GetTransactionBankDate(request);
        }

        public async Task<object> GetTransactionDetails(object request) { 
          return await _coreClient.GetTransactionDetails(request);
        }

        public async Task<IntraBankTransferResponse> PerformIntraBankTransfer(string requestId, IntraBankTransferRequest request)
        {
            // 1️⃣ Insert if not exists
            //var transfer = await _repository.CreateIfNotExistsAsync(requestId, request);

            //if (transfer.Status == "Success")
            //    return JsonSerializer.Deserialize<IntraBankTransferResponse>(transfer.ResponsePayload);

            try
            {
                // 2️⃣ Call Core API
                //var intraBankRequest = IntraBankTransferRequestFactory.Create(requestId, request.SourceAccountId, request.DestinationAccountId, request.Amount,
                //    request.Remarks, request.ChannelReference, "", request.Currency, request.DocNum, request.ChargeEventId, request.BatchId, explanationCode: request.UserPartTransactionCode);

                var response = await _coreClient.PerformIntraBankTransfer(requestId, request);
                _logger.LogInformation("Back from Performing IntraBankTransfer with response: {@Response}", response);

                //await _repository.UpdateStatusAsync(
                //    requestId,
                //    "Success",
                //    response.TranStatus ?? "00",
                //    JsonSerializer.Serialize(response));

                return response;
            }
            catch (Refit.ApiException ex) when ((int)ex.StatusCode == 500)
            {
                _logger.LogError("Error performing intra-bank transfer with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
                // 3️⃣ On 500, query transaction status
                string date = DateTime.Now.ToString("yyyy-MM-dd");

                _logger.LogInformation("About to get transaction status for requestId: {RequestId}. StatusCode: {StatusCode}, Content: {Content}", requestId, ex.StatusCode, ex.Content);
                var statusResponse = await GetTransactionStatus(requestId, request.TransferTransactionDetail[1].AccountIdentification, date, request.TransferTransactionDetail[1].TransactionParticulars);
                _logger.LogInformation("Back from GetTransactionStatus with response: {@StatusResponse}", statusResponse);

                if (statusResponse != null && statusResponse?.TranDetails != null)
                {
                    var transactionDetails = statusResponse?.TranDetails?.FirstOrDefault();
                    //await _repository.UpdateStatusAsync(
                    //    requestId,
                    //    "Success",
                    //    "200",
                    //    JsonSerializer.Serialize(statusResponse));

                    var response = new IntraBankTransferResponse
                    {
                        TransactionDate = transactionDetails.TransactionDate,
                        TransactionReference = transactionDetails.TransactionIdentification,
                        TranStatus = ""
                    };

                    return response;
                }
                else
                {
                    //await _repository.UpdateStatusAsync(
                    //    requestId,
                    //    "Failed",
                    //    ex.StatusCode.ToString(),
                    //    ex.Content);

                    var response = new IntraBankTransferResponse
                    {
                        TransactionDate = "",
                        TransactionReference = "",
                        TranStatus = "Failed",
                        Message = ex.Content
                    };

                    return response;
                }
            }
            catch (Refit.ApiException ex)
            {
                // Other exceptions
                //await _repository.UpdateStatusAsync(
                //    requestId,
                //    "Failed",
                //    "500",
                //    ex.Message);

                _logger.LogError(ex, "Error performing intra-bank transfer for requestId: {RequestId}", requestId);
                var response = new IntraBankTransferResponse
                {
                    TransactionDate = "",
                    TransactionReference = "",
                    TranStatus = "Failed",
                    Message = ex.Content
                };

                return response;
            }
        }

        public Task<object> ChargeInquiry(ChargeInquiryRequest request){
            return _coreClient.ChargeInquiry(request);
        }

        public Task<object> PerformCharge(ChargeDetailsDto intraBankTransferRequest)
        {
            return _coreClient.PerformCharge(intraBankTransferRequest);
        }

        public Task<object> PerformStatementCharge(StatementChargeRquest chargeRequest)
        {
            return _coreClient.PerformStatementCharge(chargeRequest);
        }

        private async Task<GetTransactionsResponse> GetTransactionStatus(string requestId, string accountId, string date, string searchTerm)
        {
            try
            {
                searchTerm = searchTerm.Length > 50 ? searchTerm.Substring(0,50) : searchTerm;
                var request = new Dictionary<string, string>
                {
                    { "fromDate", $"{ date }" },
                    { "toDate", $"{ date }" },
                    { "accountId", $"{ accountId }" },
                    { "tranRmks", $"{ searchTerm }" }
                };
                var statusResponse = await _coreClient.GetTransactionDetails(request);
                string response = JsonSerializer.Serialize(statusResponse);

                if (TryDeserialize(response , out GetTransactionsResponse result))
                {
                    return result;
                }
                return default;
            }
            catch (Exception ex)
            {
                // Log exception or handle as needed
                throw new Exception($"Error fetching transaction status: {ex.Message}", ex);
            }
        }

        private bool TryDeserialize<T>(string json, out T result)
        {
            try
            {
                result = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result != null;
            }
            catch (JsonException ex)
            {
                result = default;
                return false;
            }
        }
    }
}
