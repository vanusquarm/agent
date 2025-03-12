using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Finacle.Core.Domain;

namespace Finacle.Core
{
    public partial class FinacleClient
    {
        public async Task<IntraBankTransferResponse> TransferFundsAsync(IntraBankTransferRequest transferRequest, string requestId = null)
        {
            if (transferRequest == null)
                throw new ArgumentNullException(nameof(transferRequest));
            if (transferRequest.Amount <= 0)
                throw new ArgumentException("Transaction amount must be greater than zero.", nameof(transferRequest.Amount));
            if (string.IsNullOrWhiteSpace(transferRequest.SourceAccountId) || string.IsNullOrWhiteSpace(transferRequest.DestinationAccountId))
                throw new ArgumentException("Invalid SourceAccount || DestinationAccount", nameof(transferRequest));

            var requestUri = "api/v1/Transactions/transfer";
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(JsonSerializer.Serialize(transferRequest), Encoding.UTF8, "application/json")
            };

            requestMessage.Headers.Add("requestId", requestId ?? Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IntraBankTransferResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<EodStatusResponse> FetchEodStatus(EodStatusRequest request, string requestId = null)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/Transactions/eod-status")
            {
                Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
            };

            requestMessage.Headers.Add("requestId", requestId ?? Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<EodStatusResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<TransactionResponse> GetTransactionsAsync(Dictionary<string, string> queryParams, string requestId = null)
        {
            var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync().Result;
            var requestUrl = $"/api/v1/Transactions?{queryString}";

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            requestMessage.Headers.Add("requestId", requestId ?? Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TransactionResponse>(content);
        }

        public async Task<ChargeResponse> ChargeTransactionAsync(ChargeRequest request, string requestId = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/Transactions/charges")
            {
                Content = new StringContent(JsonSerializer.Serialize(ChargeRequestv2.Map(request)), Encoding.UTF8, "application/json")
            };

            requestMessage.Headers.Add("requestId", requestId ?? Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<ChargeResponse>(responseStream);
        }
    }
}
