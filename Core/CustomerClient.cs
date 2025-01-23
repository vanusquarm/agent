using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Finacle.Core
{
    public class CustomerClient:FinacleClient
    {
        public CustomerClient(string baseUrl) : base(baseUrl)
        {
        }

        // Get customer details
        public async Task<string> GetCustomerAsync(string customerId)
        {
            var response = await _httpClient.GetAsync($"/api/v1/Customers/{customerId}");

            response.EnsureSuccessStatusCode(); // Throws exception if not 2xx response

            return await response.Content.ReadAsStringAsync();
        }

        // Get accounts of a customer
        public async Task<string> GetCustomerAccountsAsync(string customerId)
        {
            var response = await _httpClient.GetAsync($"/api/v1/Customers/{customerId}/accounts");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        // Get account balances of a customer
        public async Task<string> GetCustomerAccountBalancesAsync(string customerId)
        {
            var response = await _httpClient.GetAsync($"/api/v1/Customers/{customerId}/account-balances");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        // Get transactions for a specific account of a customer
        public async Task<string> GetAccountTransactionsAsync(string customerId, string accountId)
        {
            var response = await _httpClient.GetAsync($"/api/v1/Customers/{customerId}/accounts/{accountId}/transactions");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
