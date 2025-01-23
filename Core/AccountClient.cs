using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Finacle.Core
{
    internal class AccountClient: FinacleClient
    {
        public AccountClient(string baseUrl) : base(baseUrl)
        {
        }

        // Get account details
        public async Task<string> GetAccountAsync(string accountId)
        {
            var response = await _httpClient.GetAsync($"/api/v1/Accounts/{accountId}");

            response.EnsureSuccessStatusCode(); // Throws exception if not 2xx response

            return await response.Content.ReadAsStringAsync();
        }

        // Get account balances
        public async Task<string> GetAccountBalancesAsync(string accountId)
        {
            var response = await _httpClient.GetAsync($"/api/v1/Accounts/{accountId}/balances");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        // Get account transactions
        public async Task<string> GetAccountTransactionsAsync(string accountId)
        {
            var response = await _httpClient.GetAsync($"/api/v1/Accounts/{accountId}/transactions");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
