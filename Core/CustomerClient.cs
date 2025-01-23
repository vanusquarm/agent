using Finacle.Core.Domain;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Finacle.Core
{
    public partial class FinacleClient
    {

        // Get customer details
        public async Task<T> GetCustomerAsync<T>(string customerId)
        {
            var response = await _httpClient.GetAsync($"/api/v1/Customers/{customerId}");

            response.EnsureSuccessStatusCode(); // Throws exception if not 2xx response

            var content = await response.Content.ReadAsStringAsync();
            var customer = JsonSerializer.Deserialize<T>(content);

            return customer;
        }

        // Get accounts of a customer
        public async Task<List<Account>> GetCustomerAccountsAsync(string customerId)
        {
            var response = await _httpClient.GetAsync($"/api/v1/Customers/{customerId}/accounts");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var accounts = JsonSerializer.Deserialize<GetCustomerAcctsResponse>(content);

            return accounts.BasicAccountDetails;
        }

        // Get account balances of a customer
        public async Task<List<CustomerAccountBalance>> GetCustomerAccountBalancesAsync(string customerId)
        {
            var response = await _httpClient.GetAsync($"/api/v1/Customers/{customerId}/account-balances");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var accountsBalances = JsonSerializer.Deserialize<GetAcctBalancesResponse>(content);

            return accountsBalances.CustomerAccountBalances;
        }

        // Get transactions for a specific account of a customer
        public async Task<List<Transaction>> GetAccountTransactionsAsync(string customerId, string accountId)
        {
            var response = await _httpClient.GetAsync($"/api/v1/Customers/{customerId}/accounts/{accountId}/transactions");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var transactions = JsonSerializer.Deserialize<GetAcctTransactionsResponse>(content);

            return transactions.TransactionDetailsList;
        }
    }
}
