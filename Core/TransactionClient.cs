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
        public async Task<IntraBankTransferResponse> TransferFundsAsync(IntraBankTransferRequest transferRequest, string requestId)
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

            requestMessage.Headers.Add("RequestId", requestId);

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) return null;

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

            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) return null;

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<EodStatusResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<TransactionResponse> GetTransactionsAsync(
            string fromDate, string toDate, string accountId, string remarks = null, string refNo = null,
            string tranInd = null, string revInd = null, string tranId = null, string tranType = null,
            string userTransactionCode = null, string currency = null, string tellerId = null,
            string tranRefNumber = null, string tranRmks = null, string requestId = null)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "FromDate", fromDate },
                { "ToDate", toDate },
                { "AccountId", accountId }
            };

            if (!string.IsNullOrEmpty(remarks)) queryParams.Add("Remarks", remarks);
            if (!string.IsNullOrEmpty(refNo)) queryParams.Add("RefNo", refNo);
            if (!string.IsNullOrEmpty(tranInd)) queryParams.Add("TranInd", tranInd);
            if (!string.IsNullOrEmpty(revInd)) queryParams.Add("RevInd", revInd);
            if (!string.IsNullOrEmpty(tranId)) queryParams.Add("TranId", tranId);
            if (!string.IsNullOrEmpty(tranType)) queryParams.Add("TranType", tranType);
            if (!string.IsNullOrEmpty(userTransactionCode)) queryParams.Add("UserTransactionCode", userTransactionCode);
            if (!string.IsNullOrEmpty(currency)) queryParams.Add("Currency", currency);
            if (!string.IsNullOrEmpty(tellerId)) queryParams.Add("TellerId", tellerId);
            if (!string.IsNullOrEmpty(tranRefNumber)) queryParams.Add("TranRefNumber", tranRefNumber);
            if (!string.IsNullOrEmpty(tranRmks)) queryParams.Add("TranRmks", tranRmks);

            try
            {
                var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync().GetAwaiter().GetResult();
                var requestUrl = $"/api/v1/Transactions?{queryString}";

                var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

                var response = await _httpClient.SendAsync(requestMessage);
                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TransactionResponse>(content);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"HTTP Request error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<ChargeResponse> ChargeTransactionAsync(ChargeRequest request, string requestId)
        {
            if (string.IsNullOrEmpty(requestId))
                throw new ArgumentNullException(nameof(requestId));
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/Transactions/charges")
            {
                Content = new StringContent(JsonSerializer.Serialize(ChargeRequestv2.Map(request)), Encoding.UTF8, "application/json")
            };

            requestMessage.Headers.Add("RequestId", requestId);

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) return null;

            var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<ChargeResponse>(responseStream);
        }

        public async Task<CashTransferResponse> DepositCashAsync(CashTransferRequest cashTransferRequest, string requestId)
        {
            if (string.IsNullOrEmpty(requestId))
                throw new ArgumentNullException(nameof(requestId));

            var request = new Dictionary<string, string>
            {
                { "tellerID", cashTransferRequest.TellerId },
                { "debitAccount", cashTransferRequest.CustomerAccountId },
                { "amount", cashTransferRequest.Amount.ToString() },
                { "remarks", cashTransferRequest.Remarks },
                { "channelReference", cashTransferRequest.ChannelReference }
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/Transactions/cash-deposit")
            {
                Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
            };

            requestMessage.Headers.Add("RequestId", requestId);

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) return null;

            var responseContent = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<CashTransferResponse>(responseContent);
        }

        public async Task<CashTransferResponse> WithdrawCashAsync(CashTransferRequest cashTransferRequest, string requestId)
        {
            if (string.IsNullOrEmpty(requestId))
                throw new ArgumentNullException(nameof(requestId));

            var request = new Dictionary<string, string>
            {
                { "tellerID", cashTransferRequest.TellerId },
                { "creditAccount", cashTransferRequest.CustomerAccountId },
                { "amount", cashTransferRequest.Amount.ToString() },
                { "remarks", cashTransferRequest.Remarks },
                { "channelReference", cashTransferRequest.ChannelReference }
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/Transactions/cash-withdrawal")
            {
                Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
            };

            requestMessage.Headers.Add("RequestId", requestId);

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CashTransferResponse>(responseContent);
        }
    }
}
