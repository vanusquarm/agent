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
        public T GetCustomer<T>(string customerId)
        {
            var response = _httpClient.GetAsync($"/api/v1/Customers/{customerId}").Result;

            response.EnsureSuccessStatusCode(); // Throws exception if response is not 2xx

            var content = response.Content.ReadAsStringAsync().Result;
            var customer = JsonSerializer.Deserialize<T>(content);

            return customer;
        }

        public bool IsCustomer(string phoneNumber)
        {
            var response = _httpClient.GetAsync($"/api/v1/Customers/{phoneNumber}").Result;

            response.EnsureSuccessStatusCode(); // Throws exception if response is not 2xx

            var content = response.Content.ReadAsStringAsync().Result;
            var customer = JsonSerializer.Deserialize<GetRetailCustomerResponse>(content);

            return customer.CustomerIdentifier != null;
        }

        public async Task<string> GetCustomerData(string customerId, string route)
        {
            // Construct the URL
            var url = $"/api/v1/Customers/{customerId}/{route}";

            try
            {
                var response = _httpClient.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                var content = response.Content.ReadAsStringAsync().Result;

                return content;
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the API request
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public string GetCustomerPreferredName(string customerId)
        {
            var customerName = (GetCustomer<GetRetailCustomerResponse>(customerId)).CustomerName ??
                (GetCustomer<GetCorpCustomerResponse>(customerId)).CorporateName;

            return customerName;
        }

        // Get accounts of a customer
        public List<Account> GetCustomerAccounts(string customerId)
        {
            var response = _httpClient.GetAsync($"/api/v1/Customers/{customerId}/accounts").Result;

            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().Result;
            var accounts = JsonSerializer.Deserialize<GetCustomerAcctsResponse>(content);

            return accounts.BasicAccountDetails;
        }

        // Get account balances of a customer
        public List<CustomerAccountBalance> GetCustomerAccountBalances(string customerId)
        {
            var response = _httpClient.GetAsync($"/api/v1/Customers/{customerId}/account-balances").Result;

            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().Result;
            var accountsBalances = JsonSerializer.Deserialize<GetAcctBalancesResponse>(content);

            return accountsBalances.CustomerAccountBalances;
        }

        // Get transactions for a specific account of a customer
        public List<Transaction> GetAccountTransactions(string customerId, string accountId)
        {
            var response = _httpClient.GetAsync($"/api/v1/Customers/{customerId}/accounts/{accountId}/transactions").Result;

            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().Result;
            var transactions = JsonSerializer.Deserialize<GetAcctTransactionsResponse>(content);

            return transactions.TransactionDetailsList;
        }
        
        public CreateRetailCustomerResponse CreateRetailCustomer(RetailCustomerRequest request)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(request);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = _httpClient.PostAsync("/retail-customers", httpContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    return JsonSerializer.Deserialize<CreateRetailCustomerResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    var errorContent = response.Content.ReadAsStringAsync().Result;
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
