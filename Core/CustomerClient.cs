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

        public async Task<string> GetCustomerDataAsync(string customerId, string route)
        {
            // Construct the URL
            var url = $"/api/v1/Customers/{customerId}/{route}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();

                return content;
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the API request
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task<string> GetCustomerPreferredNameAsync(string customerId)
        {
            var customerName = (await GetCustomerAsync<GetRetailCustomerResponse>(customerId)).CustomerName ??
                (await GetCustomerAsync<GetCorpCustomerResponse>(customerId)).CorporateName;

            return customerName;
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
        
        public async Task<CreateRetailCustomerResponse> CreateRetailCustomerAsync<T>(RetailCustomerRequest request)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(request);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/retail-customers", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<CreateRetailCustomerResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Error creating customer. Status: {response.StatusCode}, Response: {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"HTTP Request error: {ex.Message}");
            }
            catch (JsonException ex)
            {
                throw new Exception($"JSON Serialization/Deserialization error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
