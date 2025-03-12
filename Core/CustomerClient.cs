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
        public async Task<T> GetCustomerAsync<T>(string customerId, string requestId = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Customers/{customerId}");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode(); // Throws exception if not 2xx response

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content);
        }

        public bool IsCustomerStaff(string customerId, string requestId = null)
        {
            var customer = GetCustomer<GetRetailCustomerResponse>(customerId, requestId);
            return customer.RelationshipManagerDetails[0].RelationshipManagerId == "GH001800";
        }

        public async Task<BankDateDto> GetCustomerBankDateAsync(string branchCode, string requestId = null)
        {
            var jsonContent = JsonSerializer.Serialize(branchCode);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/transactions/bankdate")
            {
                Content = httpContent
            };
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode(); // Throws exception if not 2xx response

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BankDateDto>(content);
        }

        public async Task<string> GetCustomerDataAsync(string customerId, string route, string requestId = null)
        {
            // Construct the URL
            var url = $"/api/v1/Customers/{customerId}/{route}";

            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

                var response = await _httpClient.SendAsync(requestMessage);
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

        public async Task<string> GetCustomerPreferredNameAsync(string customerId, string requestId = null)
        {
            var customerName = (await GetCustomerAsync<GetRetailCustomerResponse>(customerId, requestId)).CustomerName ??
                (await GetCustomerAsync<GetCorpCustomerResponse>(customerId, requestId)).CorporateName;

            return customerName;
        }

        // Get accounts of a customer
        public async Task<List<Account>> GetCustomerAccountsAsync(string customerId, string requestId = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Customers/{customerId}/accounts");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var accounts = JsonSerializer.Deserialize<GetCustomerAcctsResponse>(content);

            return accounts.BasicAccountDetails;
        }

        // Get account balances of a customer
        public async Task<List<CustomerAccountBalance>> GetCustomerAccountBalancesAsync(string customerId, string requestId = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Customers/{customerId}/account-balances");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var accountsBalances = JsonSerializer.Deserialize<GetAcctBalancesResponse>(content);

            return accountsBalances.CustomerAccountBalances;
        }

        // Get transactions for a specific account of a customer
        public async Task<List<Transaction>> GetAccountTransactionsAsync(string customerId, string accountId, string requestId = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Customers/{customerId}/accounts/{accountId}/transactions");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var transactions = JsonSerializer.Deserialize<GetAcctTransactionsResponse>(content);

            return transactions.TransactionDetailsList;
        }

        public async Task<CreateRetailCustomerResponse> CreateRetailCustomerAsync(RetailCustomerRequest request, string requestId = null)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(request);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/retail-customers")
                {
                    Content = httpContent
                };
                requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

                var response = await _httpClient.SendAsync(requestMessage);

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



