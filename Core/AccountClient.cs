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

        // Get account details
        public async Task<Account> GetAccountAsync(string accountId)
        {
            var response = await _httpClient.GetAsync($"/api/v1/Accounts/{accountId}");

            var content = await response.Content.ReadAsStringAsync();
            var accountDetails = JsonSerializer.Deserialize<Account>(content);

            return accountDetails;
        }

        // Get account status
        public async Task<string> GetAccountStatusAsync(string accountId)
        {
            var response = await _httpClient.GetAsync($"/api/v1/Accounts/{accountId}");

            var content = await response.Content.ReadAsStringAsync();
            var accountDetails = JsonSerializer.Deserialize<Account>(content);

            return accountDetails.AccountStatus;
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
        public async Task<List<Transaction>> GetAccountTransactionsAsync(string accountId)
        {
            var response = await _httpClient.GetAsync($"/api/v1/Accounts/{accountId}/transactions");

            var content = await response.Content.ReadAsStringAsync();
            var transactions = JsonSerializer.Deserialize<List<Transaction>>(content);

            return transactions;
        }

        // Add account restriction
        public async Task<FreezeAccountResponse> FreezeAccountAsync(string accountId, FreezeAccountRequest request)
        {
            var url = $"api/v1/Accounts/{accountId}/freeze";

            var jsonContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, jsonContent);

            var content = await response.Content.ReadAsStringAsync();
            var freezeStatus = JsonSerializer.Deserialize<FreezeAccountResponse>(content);

            return freezeStatus;
        }

        // Remove account restriction
        public async Task<UnFreezeAccountResponse> UnfreezeAccountAsync(string accountId, UnfreezeAccountRequest request)
        {
            var url = $"api/v1/Accounts/{accountId}/unfreeze";

            var jsonContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, jsonContent);

            var content = await response.Content.ReadAsStringAsync();
            var unfreezeStatus = JsonSerializer.Deserialize<UnFreezeAccountResponse>(content);

            return unfreezeStatus;
        }

        // Check if account is frozen
        public async Task<bool> IsAccountFrozenAsync(string accountId)
        {
            var account = await GetAccountAsync(accountId);

            return account.AccountStatus == "Frozen";
        }

        // Check if account exists
        public async Task<bool> IsAccountExistAsync(string accountId)
        {
            var account = await GetAccountAsync(accountId);

            return account.AccountIdentification == accountId;
        }

        // Get customer details by accountId
        public async Task<(AccountDetail, CustomerDetail)> GetAccountCustomersAsync(string accountId)
        {
            var response = await _httpClient.GetAsync($"api/v1/Accounts/{accountId}/customers"); // Send the GET request

            var content = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonSerializer.Deserialize<AccountCustomersResponse>(content);

            return (deserializedResponse.AccountDetails[0], deserializedResponse.CustomerDetails[0]);
        }

        public async Task<CustomerDetail> GetCustomerByAccountIdAsync(string accountId)
        {
            var (_, customer) = await GetAccountCustomersAsync(accountId);
            return customer;
        }

        public async Task<string> GetCustPreferredNameByAccountIdAsync(string accountId)
        {
            var (_, customer) = await GetAccountCustomersAsync(accountId);
            return customer.CustomerName;
        }

        // Get account statements

        public async Task<HttpResponseMessage> GetAccountStatementsAsync(string accountId, string fromDate, string toDate, int setNumber = 1, int maximumRecords = 10)
        {
            var query = $"?AccountId={accountId}&FromDate={fromDate}&ToDate={toDate}&SetNumber={setNumber}&MaximumRecords={maximumRecords}";
            var response = await _httpClient.GetAsync($"/api/v1/Accounts/{accountId}/statements{query}");
            response.EnsureSuccessStatusCode();
            return response;
        }

        // Create Account
        public async Task<string> CreateCurrentAccountAsync(string customerReference, string productCode, string glSubHeadCode, string accountCurrency, string branchReference, string accountOpeningDate)
        {
            var url = "/v1/Accounts/current-account"; // Replace with actual base URL

            // Create the request body as JSON
            var json = $"{{\n  \"customerReference\": \"{customerReference}\",\n  \"productCode\": \"{productCode}\",\n  \"glSubHeadCode\": \"{glSubHeadCode}\",\n  \"accountCurrency\": \"{accountCurrency}\",\n  \"branchReference\": \"{branchReference}\",\n  \"accountOpeningDate\": \"{accountOpeningDate}\"\n}}";

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Make the POST request
            var response = await _httpClient.PostAsync(url, content);

            // Ensure the request was successful
            response.EnsureSuccessStatusCode();

            // Return the response content as a string
            return await response.Content.ReadAsStringAsync();
        }
    }
}
