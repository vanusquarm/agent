using Finacle.Infrastructure.Logging;
using Finacle.Infrastructure.Persistence.Repositories;
using Finacle.Infrastructure.Utils;
using MassTransit.Middleware;
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
                var _maxRetries = 3;
                _logger.LogError("Error performing intra-bank transfer with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
                // 3️⃣ On 500, query transaction status
                var status = await CheckTransactionStatusAsync(requestId, request);

                while (status == null)
                {
                    int retryCount = 0;
                    status = await CheckTransactionStatusAsync(requestId, request);

                    retryCount++;

                    if (retryCount > _maxRetries)
                    {
                        _logger.LogError("Max retries reached.");
                        return new IntraBankTransferResponse
                        {
                            TransactionDate = "",
                            TransactionReference = "",
                            TranStatus = "Failed",
                            Message = ex.Content
                        };
                    }

                    var delaySeconds = Math.Pow(4, retryCount);
                    var jitter = new Random().Next(0, 1000);
                    var delay = TimeSpan.FromSeconds(delaySeconds) + TimeSpan.FromMilliseconds(jitter);

                    _logger.LogWarning("Retry {RetryCount} after {Delay}", retryCount, delay);

                    await Task.Delay(delay);
                }
                return status;
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

        private async Task<IntraBankTransferResponse?> CheckTransactionStatusAsync(
            string requestId,
            IntraBankTransferRequest request)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");

            _logger.LogInformation(
                "Checking transaction status. RequestId: {RequestId}, AccountId: {AccountId}, Date: {Date}, SearchTerm: {SearchTerm}",
                requestId,
                request.TransferTransactionDetail[0].AccountIdentification,
                date,
                request.TransferTransactionDetail[0].TransactionParticulars
            );

            var statusResponse = await GetTransactionStatus(
                requestId,
                request.TransferTransactionDetail[0].AccountIdentification,
                date,
                request.TransferTransactionDetail[0].TransactionParticulars
            );

            if (statusResponse?.TranDetails != null)
            {
                var transactionDetails = statusResponse.TranDetails
                    .FirstOrDefault(x =>
                        x.TransactionAmount ==
                        request.TransferTransactionDetail[1].TransactionAmount.ToString());

                if (transactionDetails != null)
                {
                    return new IntraBankTransferResponse
                    {
                        TransactionDate = DateTime.Parse(transactionDetails.TransactionDate).ToString("yyyy-MM-dd"),
                        TransactionReference = transactionDetails.TransactionIdentification,
                        TranStatus = "Success"
                    };
                }
            }

            var dbresponse = await CheckTransactionStatusAsync(request.TransferTransactionDetail[0].ReferenceNumber);

            if (dbresponse == null) return null;

            return new IntraBankTransferResponse
            {
                TransactionDate = dbresponse.TransactionDate,
                TransactionReference = dbresponse.TransactionReference,
                TranStatus = dbresponse.TranStatus
            };
        }

        public async Task<Finacle.Core.Domain.IntraBankTransferResponse?> CheckTransactionStatusAsync(string channelReference)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");

            _logger.LogInformation("Checking transaction status. RequestId: {RequestId}", channelReference);

                var statusResponse = await _repository.GetTransactionAsync(channelReference);

            return statusResponse;
        }
    }
}
