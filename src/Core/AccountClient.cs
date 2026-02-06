using Finacle.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Finacle.Core
{
    public partial class FinacleClient
    {
        // Get account details
        public async Task<Account> GetAccountAsync(string accountId, string requestId = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Accounts/{accountId}");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Account>(content);
        }

        // Get account status
        public async Task<string> GetAccountStatusAsync(string accountId, string requestId = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Accounts/{accountId}");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            var accountDetails = JsonSerializer.Deserialize<Account>(content);

            return accountDetails.AccountStatus;
        }

        // Get account balances
        public async Task<AccountBalance> GetAccountBalanceAsync(string accountId, string date = null, string requestId = null)
        {
            try
            {
                if (string.IsNullOrEmpty(date))
                {
                    date = DateTime.UtcNow.ToString("yyyy-MM-dd");
                }

                var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Accounts/{accountId}/balance");
                requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

                var response = await _httpClient.SendAsync(requestMessage);
                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                var accountsBalance = JsonSerializer.Deserialize<AccountBalance>(content);

                return accountsBalance;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<List<Balance>> GetAccountBalancesAsync(string accountId, string balanceType, string requestId = null)
        {
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Accounts/{accountId}/balances");
                requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

                var response = await _httpClient.SendAsync(requestMessage);
                if (!response.IsSuccessStatusCode) return new List<Balance>();

                var content = await response.Content.ReadAsStringAsync();
                var accountsBalances = JsonSerializer.Deserialize<AccountBalanceDto>(content);

                if (!string.IsNullOrEmpty(balanceType))
                {
                    return accountsBalances.Balances.Where(balance => balance.BalanceType == balanceType).ToList();
                }
                return accountsBalances.Balances;
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        // Get account transactions
        public async Task<List<Transaction>> GetAccountTransactionsAsync(string accountId, string requestId = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Accounts/{accountId}/transactions");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Transaction>>(content);
        }

        // Add account restriction
        public async Task<FreezeAccountResponse> FreezeAccountAsync(string accountId, FreezeAccountRequest request, string requestId = null)
        {
            var url = $"api/v1/Accounts/{accountId}/freeze";

            var jsonContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = jsonContent
            };
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<FreezeAccountResponse>(content);
        }

        // Remove account restriction
        public async Task<UnFreezeAccountResponse> UnfreezeAccountAsync(string accountId, UnfreezeAccountRequest request, string requestId = null)
        {
            var url = $"api/v1/Accounts/{accountId}/unfreeze";

            var jsonContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = jsonContent
            };
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UnFreezeAccountResponse>(content);
        }

        // Check if account is frozen
        public async Task<bool> IsAccountFrozenAsync(string accountId, string requestId = null)
        {
            var account = await GetAccountAsync(accountId, requestId);
            if (account == null) return false;
            return account.AccountStatus == "Frozen";
        }

        // Check if account exists
        public async Task<bool> IsAccountExistAsync(string accountId, string requestId = null)
        {
            var account = await GetAccountAsync(accountId, requestId);
            if (account == null) return false;
            return account.AccountIdentification == accountId;
        }

        // Get customer details by accountId
        public async Task<(AccountDetail, CustomerDetail)> GetAccountCustomersAsync(string accountId, string requestId = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"api/v1/Accounts/{accountId}/customers");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) return (null, null);

            var content = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonSerializer.Deserialize<AccountCustomersResponse>(content);

            if (deserializedResponse?.AccountDetails == null || deserializedResponse.AccountDetails.Count == 0)
                throw new InvalidOperationException("AccountDetails is missing or empty in the response.");
            if (deserializedResponse.CustomerDetails == null || deserializedResponse.CustomerDetails.Count == 0)
                throw new InvalidOperationException("CustomerDetails is missing or empty in the response.");

            return (deserializedResponse.AccountDetails[0], deserializedResponse.CustomerDetails[0]);
        }

        public async Task<GetRetailCustomerResponse> GetCustomerByAccountIdAsync(string accountId, string requestId = null)
        {
            var (_, customer) = await GetAccountCustomersAsync(accountId, requestId);
            var customerDetails = await GetCustomerAsync<GetRetailCustomerResponse>(customer.CustomerReference, requestId);
            return customerDetails;
        }

        public async Task<string> GetCustPreferredNameByAccountIdAsync(string accountId, string requestId = null)
        {
            var (_, customer) = await GetAccountCustomersAsync(accountId, requestId);
            return customer.CustomerName;
        }

        // Get account statements
        public async Task<AcctStatementResponse> GetAccountStatementsAsync(string accountId, string fromDate, string toDate, int setNumber = 1, int maximumRecords = 10, string requestId = null)
        {
            var query = $"?AccountId={accountId}&FromDate={fromDate}&ToDate={toDate}&SetNumber={setNumber}&MaximumRecords={maximumRecords}";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Accounts/{accountId}/statements{query}");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AcctStatementResponse>(content);
        }

        // Create Account
        public async Task<string> CreateCurrentAccountAsync(string customerReference, string productCode, string glSubHeadCode, string accountCurrency, string branchReference, string accountOpeningDate, string requestId = null)
        {
            var url = "/v1/Accounts/current-account"; // Replace with actual base URL

            var json = $"{{\"customerReference\": \"{customerReference}\",\"productCode\": \"{productCode}\",\"glSubHeadCode\": \"{glSubHeadCode}\",\"accountCurrency\": \"{accountCurrency}\",\"branchReference\": \"{branchReference}\",\"accountOpeningDate\": \"{accountOpeningDate}\"}}";

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadAsStringAsync();
        }

        // Create Account
        public async Task<string> CreateSavingsAccountAsync(CreateSavingsAcctRequest createSavingsAcctRequest, string requestId = null)
        {
            var url = "/v1/Accounts/savings-account"; // Replace with actual base URL

            var json = JsonSerializer.Serialize(createSavingsAcctRequest);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<LienResponse> BlockFundsAsync(string accountId, LienRequest request, string requestId = null)
        {
            var json = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/Accounts/{accountId}/liens")
            {
                Content = json
            };
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());
            var response = await _httpClient.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<LienResponse>(content);
        }

        public async Task<bool> HasSufficientFundsAsync(string AccountNumber, double amount)
        {
            var balance = await GetAccountBalancesAsync(AccountNumber, "EFFAVL");
            var currentBalance = balance.FirstOrDefault()?.Amount?.Value.ToString();

            // Check if the balance is sufficient
            double closingBalance = 0;
            double.TryParse(currentBalance, out closingBalance);

            return closingBalance >= amount;
        }
    }
}


