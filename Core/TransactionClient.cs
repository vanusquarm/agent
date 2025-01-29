using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Finacle.Core.Domain;

namespace Finacle.Core
{
    public partial class FinacleClient
    {
        public async Task<CreateIntraBankTransferResponse> CreateIntraBankTransferTransaction(CreateIntraBankTransferRequest request)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
                HttpResponseMessage response = await _httpClient.PostAsync("/api/v2/transactions/INTRA-BANK-FUND-TRANSFER", content);
                response.EnsureSuccessStatusCode();
            
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<CreateIntraBankTransferResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
    }
}