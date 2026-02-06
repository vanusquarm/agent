using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Finacle.Core.Domain;

namespace Finacle.Core
{
    public partial class FinacleClient
    {
        // GET /api/v1/Branches
        public async Task<BranchDetailsResponseDto> ListBranchesAsync(string requestId = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/v1/Branches");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BranchDetailsResponseDto>(content);
        }

        // GET /api/v1/Branch/branches
        public async Task<BranchDetails> GetBranchDetailsAsync(string branchCode = null, string bankCode = null, string requestId = null)
        {
            var query = $"?bankCode={bankCode}";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Branches/{branchCode}");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BranchDetails>(content);
        }

        // GET /api/v1/Branch/customers/{customerId}/accounts
        public async Task<BranchCustomerAccounts> GetCustomerAccountsWithBranchAsync(string customerId, bool includeClosedAccounts = false, int setNumber = 1, int maximumRecords = 10, string requestId = null)
        {
            var query = $"?includeClosedAccounts={(includeClosedAccounts ? "Y" : "N")}&setNumber={setNumber}&maximumRecords={maximumRecords}";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Branch/customers/{customerId}/accounts{query}");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BranchCustomerAccounts>(content);
        }
    }
}



