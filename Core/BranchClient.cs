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

        // GET /api/v1/Branch/branches-details
        public async Task<BranchDetailsResponseDto> ListBranchesAsync()
        {
            var response = await _httpClient.GetAsync("/api/v1/Branch/branches-details");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var branches = JsonSerializer.Deserialize<BranchDetailsResponseDto>(content);

            return branches;
        }

        // GET /api/v1/Branch/branches
        public async Task<BranchDetails> GetBranchDetailsAsync(string branchCode = null, string bankCode = null)
        {
            var query = $"?branchCode={branchCode}&bankCode={bankCode}";
            var response = await _httpClient.GetAsync($"/api/v1/Branch/branches{query}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var branchDetails = JsonSerializer.Deserialize<BranchDetails>(content);

            return branchDetails;
        }

        // GET /api/v1/Branch/customers/{customerId}/accounts
        public async Task<BranchCustomerAccounts> GetCustomerAccountsWithBranchAsync(string customerId, bool includeClosedAccounts = false, int setNumber = 1, int maximumRecords = 10)
        {
            var query = $"?includeClosedAccounts={(includeClosedAccounts ? "Y" : "N")}&setNumber={setNumber}&maximumRecords={maximumRecords}";
            var response = await _httpClient.GetAsync($"/api/v1/Branch/customers/{customerId}/accounts{query}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var branchDetails = JsonSerializer.Deserialize<BranchCustomerAccounts>(content);

            return branchDetails;
        }

    }
}


