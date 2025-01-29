using Finacle.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Finacle.Core
{
    public partial class FinacleClient
    {

        // Get account details
        public async Task<Account> GetAccountAsync(string accountId)
        {
            var response = await _httpClient.GetAsync($"/api/v1/Accounts/{accountId}");

            var content = await response.Content.ReadAsStringAsync();
            var accountDetails = JsonSerializer.Deserialize<Account>(content);

            return accountDetails;
        }

        // Get account balances
        public async Task<List<CustomerAccountBalance>> GetAccountBalancesAsync(string accountId, string accountType)
        {
            try
            {
                // Ensure accountType is valid (non-null and non-empty). If invalid, set a default or handle appropriately
                if (string.IsNullOrEmpty(accountType))
                {
                    accountType = null;
                }

                var response = await _httpClient.GetAsync($"/api/v1/Accounts/{accountId}/balances");

                var content = await response.Content.ReadAsStringAsync();
                var accountsBalances = JsonSerializer.Deserialize<GetAcctBalancesResponse>(content);

                return accountsBalances.CustomerAccountBalances;
            }
            catch (Exception ex) { 
                return new List<CustomerAccountBalance>();
            }
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
