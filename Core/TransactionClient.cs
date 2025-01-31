using System;
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
                var request = IntraBankTransferRequestFactory.Create(transferRequest.SourceAccountId, transferRequest.DestinationAccountId, transferRequest.Amount, transferRequest.Remarks, transferRequest.ChannelReference, transferRequest.ChannelId);
                var jsonContent = JsonSerializer.Serialize(request);
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
    }
}