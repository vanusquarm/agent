using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Finacle.Core.Common;
using Finacle.Core.Domain;

namespace Finacle.Core
{
    public partial class FinacleClient
    {
        public async Task<IntraBankTransferResponse> TransferFunds(AccountTransferRequest transferRequest)
        {
            try
            {
                
                if (transferRequest.Amount <= 0)
                {
                    throw new ArgumentException("Transaction amount must be greater than zero.", nameof(transferRequest.Amount));
                }

                if (string.IsNullOrWhiteSpace(transferRequest.SourceAccountId) ||
                    string.IsNullOrWhiteSpace(transferRequest.DestinationAccountId))
                {
                    throw new ArgumentException("Invalid SourceAccount || DestinationAccount", nameof(transferRequest));
                }
 
                var jsonContent = JsonSerializer.Serialize(transferRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                HttpResponseMessage response = await _httpClient.PostAsync("/api/v1/Transactions/intra-bank-transfer", content);
                response.EnsureSuccessStatusCode();
                
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IntraBankTransferResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw;
            }
        }

        public async Task<EodStatusResponse> FetchEodStatus(EodStatusRequest request)
        {
            try
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync("/api/v1/Transactions/eod-status", content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var eodStatusResponse = JsonSerializer.Deserialize<EodStatusResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return eodStatusResponse;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
                throw;
            }
        }

        public async Task<TransactionResponse> GetTransactionsAsync(
            string fromDate, string toDate, string accountId, string remarks = null, string refNo = null,
            string tranInd = null, string revInd = null, string tranId = null, string tranType = null,
            string userTransactionCode = null, string currency = null, string tellerId = null,
            string tranRefNumber = null, string tranRmks = null)
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
                var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync().Result;
                var requestUrl = $"/api/v1/Transactions?{queryString}";

                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var transactions = JsonSerializer.Deserialize<TransactionResponse>(content);
                return transactions;
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

    }
}