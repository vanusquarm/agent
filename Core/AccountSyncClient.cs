using Finacle.Core.Domain;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Finacle.Core
{
    public partial class FinacleClient
    {
        // Get account details
        public Account GetAccount(string accountId)
        {
            var response = _httpClient.GetAsync($"/api/v1/Accounts/{accountId}").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<Account>(content);
        }

        // Get account status
        public string GetAccountStatus(string accountId)
        {
            return GetAccount(accountId).AccountStatus;
        }

        // Get account status
        public int isAccountActive(string accountId)
        {
            return GetAccountStatus(accountId) == "O" ? 1 : 0;
        }

        // Get account balances
        public Balance GetAccountBalances(string accountId, string accountType)
        {
            try
            {
                if (string.IsNullOrEmpty(accountType))
                {
                    accountType = null;
                }
                var response = _httpClient.GetAsync($"/api/v1/Accounts/{accountId}/balances?accountType={accountType}").Result;
                var content = response.Content.ReadAsStringAsync().Result;
                var Balance = JsonSerializer.Deserialize<Balance>(content);
                return Balance;
            }
            catch (Exception)
            {
                return new Balance();
            }
        }

        public List<CustomerAccountBalance> GetAccountBalances(string accountId)
        {
            try
            {
                var response = _httpClient.GetAsync($"/api/v1/Accounts/{accountId}/balances").Result;
                var content = response.Content.ReadAsStringAsync().Result;
                var accountsBalances = JsonSerializer.Deserialize<GetAcctBalancesResponse>(content);
                return accountsBalances.CustomerAccountBalances;
            }
            catch (Exception)
            {
                return new List<CustomerAccountBalance>();
            }
        }

        public Amount GetAccountBalance(string accountId)
        {
            try
            {
                var response = _httpClient.GetAsync($"/api/v1/Accounts/{accountId}/balances?accountType=EFFAVL").Result;
                var content = response.Content.ReadAsStringAsync().Result;
                var accountBalance = JsonSerializer.Deserialize<Balance>(content);
                return accountBalance.Amount;
            }
            catch (Exception)
            {
                return new Amount();
            }
        }

        // Get account transactions
        public List<Transaction> GetAccountTransactions(string accountId)
        {
            var response = _httpClient.GetAsync($"/api/v1/Accounts/{accountId}/transactions").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<List<Transaction>>(content);
        }

        // Add account restriction
        public FreezeAccountResponse FreezeAccount(string accountId, FreezeAccountRequest request)
        {
            var url = $"api/v1/Accounts/{accountId}/freeze";
            var jsonContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync(url, jsonContent).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<FreezeAccountResponse>(content);
        }

        // Remove account restriction
        public UnFreezeAccountResponse UnfreezeAccount(string accountId, UnfreezeAccountRequest request)
        {
            var url = $"api/v1/Accounts/{accountId}/unfreeze";
            var jsonContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync(url, jsonContent).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<UnFreezeAccountResponse>(content);
        }

        // Check if account is frozen
        public bool IsAccountFrozen(string accountId)
        {
            return GetAccount(accountId).AccountStatus == "Frozen";
        }

        public bool IsNotRestricted(string accountId)
        {
            return GetAccount(accountId).AccountStatus != "Frozen";
        }

        // Check if account exists
        public bool IsAccountExist(string accountId)
        {
            return GetAccount(accountId).AccountIdentification == accountId;
        }

        // Get customer details by accountId
        public (AccountDetail accountDetail, CustomerDetail customerDetail) GetAccountCustomers(string accountId)
        {
            var response = _httpClient.GetAsync($"api/v1/Accounts/{accountId}/customers").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var deserializedResponse = JsonSerializer.Deserialize<AccountCustomersResponse>(content);
            return (deserializedResponse.AccountDetails[0], deserializedResponse.CustomerDetails[0]);
        }

        public bool IsStaff(string accountId)
        {
            var (_, customerDetail) = GetAccountCustomers(accountId);
            var customer = GetCustomer<GetRetailCustomerResponse>(customerDetail.CustomerReference);
            return customer.Segment == "GTSTAFF";
        }



        public CustomerDetail GetCustomerByAccountId(string accountId)
        {
            var (_, customer) = GetAccountCustomers(accountId);
            return customer;
        }

        public string GetCustPreferredNameByAccountId(string accountId)
        {
            var (_, customer) = GetAccountCustomers(accountId);
            return customer.CustomerName;
        }

        // Get account statements
        public AcctStatementResponse GetAccountStatements(string accountId, string fromDate, string toDate, int setNumber = 1, int maximumRecords = 10)
        {
            var query = $"?AccountId={accountId}&FromDate={fromDate}&ToDate={toDate}&SetNumber={setNumber}&MaximumRecords={maximumRecords}";
            var response = _httpClient.GetAsync($"/api/v1/Accounts/{accountId}/statements{query}").Result;
            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<AcctStatementResponse>(content);
        }

        // Create Account
        public string CreateCurrentAccount(string customerReference, string productCode, string glSubHeadCode, string accountCurrency, string branchReference, string accountOpeningDate)
        {
            var url = "/v1/Accounts/current-account";
            var json = $"{{\"customerReference\": \"{customerReference}\",\"productCode\": \"{productCode}\",\"glSubHeadCode\": \"{glSubHeadCode}\",\"accountCurrency\": \"{accountCurrency}\",\"branchReference\": \"{branchReference}\",\"accountOpeningDate\": \"{accountOpeningDate}\"}}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync(url, content).Result;
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
