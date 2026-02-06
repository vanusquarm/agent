using Finacle.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Finacle.Core
{
    public partial class FinacleClient
    {
        // Get account details
        public Account GetAccount(string accountId, string requestId = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Accounts/{accountId}");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) return null;

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<Account>(content);
        }

        // Get account status
        public string GetAccountStatus(string accountId, string requestId = null)
        {
            var account = GetAccount(accountId, requestId);
            if (account == null) return null;
            return account.AccountStatus;
        }

        // Get account status
        public int IsAccountActive(string accountId, string requestId = null)
        {
            var status = GetAccountStatus(accountId, requestId);
            if (status == null) return 0;
            return status == "O" ? 1 : 0;
        }

        // Get account balances
        public Balance GetAccountBalances(string accountId, string accountType, string requestId = null)
        {
            try
            {
                if (string.IsNullOrEmpty(accountType))
                {
                    accountType = null;
                }
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Accounts/{accountId}/balances?accountType={accountType}");
                requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

                var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode) return null;

                var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var Balance = JsonSerializer.Deserialize<Balance>(content);
                return Balance;
            }
            catch (Exception)
            {
                return new Balance();
            }
        }

        public List<CustomerAccountBalance> GetAccountBalances(string accountId, string requestId = null)
        {
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Accounts/{accountId}/balances");
                requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

                var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode) return new List<CustomerAccountBalance>();
                var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                var accountsBalances = JsonSerializer.Deserialize<GetAcctBalancesResponse>(content);
                return accountsBalances.CustomerAccountBalances;
            }
            catch (Exception)
            {
                return new List<CustomerAccountBalance>();
            }
        }

        public Amount GetAccountBalance(string accountId, string requestId = null)
        {
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Accounts/{accountId}/balances?accountType=EFFAVL");
                requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

                var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode) return null;

                var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var accountBalance = JsonSerializer.Deserialize<Balance>(content);
                return accountBalance.Amount;
            }
            catch (Exception)
            {
                return new Amount();
            }
        }

        // Get account transactions
        public List<Transaction> GetAccountTransactions(string accountId, string requestId = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Accounts/{accountId}/transactions");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) return new List<Transaction>();

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<List<Transaction>>(content);
        }

        // Add account restriction
        public FreezeAccountResponse FreezeAccount(string accountId, FreezeAccountRequest request, string requestId = null)
        {
            var url = $"api/v1/Accounts/{accountId}/freeze";
            var jsonContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = jsonContent
            };
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) return null;
            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<FreezeAccountResponse>(content);
        }

        // Remove account restriction
        public UnFreezeAccountResponse UnfreezeAccount(string accountId, UnfreezeAccountRequest request, string requestId = null)
        {
            var url = $"api/v1/Accounts/{accountId}/unfreeze";
            var jsonContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = jsonContent
            };
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) return null;
            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<UnFreezeAccountResponse>(content);
        }

        // Check if account is frozen
        public bool IsAccountFrozen(string accountId, string requestId = null)
        {
            var account = GetAccount(accountId, requestId);
            if (account == null) return false;
            return account.AccountStatus == "Frozen";
        }

        public bool IsNotRestricted(string accountId, string requestId = null)
        {
            var account = GetAccount(accountId, requestId);
            if (account == null) return false;
            return account.AccountStatus != "Frozen";
        }

        // Check if account exists
        public bool IsAccountExist(string accountId, string requestId = null)
        {
            var account = GetAccount(accountId, requestId);
            if (account == null) return false;
            return account.AccountIdentification == accountId;
        }

        // Get customer details by accountId
        public (AccountDetail accountDetail, CustomerDetail customerDetail) GetAccountCustomers(string accountId, string requestId = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"api/v1/Accounts/{accountId}/customers");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) return (null, null);
            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var deserializedResponse = JsonSerializer.Deserialize<AccountCustomersResponse>(content);
            return (deserializedResponse.AccountDetails[0], deserializedResponse.CustomerDetails[0]);
        }

        public bool IsStaff(string accountId, string requestId = null)
        {
            if (accountId.StartsWith("R")) // Check if argument is Retail CIF
            {
                var customer = GetCustomer<GetRetailCustomerResponse>(accountId, requestId);
                return (customer?.Segment == "EMPB" ||
                    customer?.Segment == "EMR" ||
                    customer?.Segment == "EHNI");

            }
            return IsStaffAccount(accountId, requestId);
        }

        public bool IsStaffAccount(string accountId, string requestId = null)
        {
            var (_, customerDetail) = GetAccountCustomers(accountId, requestId);
            if(accountId?.StartsWith("0") == null)
            {
                var account = GetAccount(accountId, requestId);
                return account?.ProductCode == "OFLIB";
            }
            return (customerDetail?.OcuupationCode == "2104");
        }

        public CustomerDetail GetCustomerByAccountId(string accountId, string requestId = null)
        {
            var (_, customer) = GetAccountCustomers(accountId, requestId);
            return customer;
        }

        public string GetCustPreferredNameByAccountId(string accountId, string requestId = null)
        {
            var (_, customer) = GetAccountCustomers(accountId, requestId);
            return customer.CustomerName;
        }

        // Get account statements
        public AcctStatementResponse GetAccountStatements(string accountId, string fromDate, string toDate, int setNumber = 1, int maximumRecords = 10, string requestId = null)
        {
            var query = $"?AccountId={accountId}&FromDate={fromDate}&ToDate={toDate}&SetNumber={setNumber}&MaximumRecords={maximumRecords}";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Accounts/{accountId}/statements{query}");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<AcctStatementResponse>(content);
        }

        // Create Account
        public string CreateCurrentAccount(string customerReference, string productCode, string glSubHeadCode, string accountCurrency, string branchReference, string accountOpeningDate, string requestId = null)
        {
            var url = "/v1/Accounts/current-account";
            var json = $"{{\"customerReference\": \"{customerReference}\",\"productCode\": \"{productCode}\",\"glSubHeadCode\": \"{glSubHeadCode}\",\"accountCurrency\": \"{accountCurrency}\",\"branchReference\": \"{branchReference}\",\"accountOpeningDate\": \"{accountOpeningDate}\"}}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }

        public LienResponse BlockFunds(string accountId, LienRequest request, string requestId = null)
        {
            var url = $"/api/v1/Accounts/{accountId}/liens";
            var jsonContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = jsonContent
            };
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();

            // Ensure the request was successful
            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<LienResponse>(content);
        }

        public bool IsAccountTheSame(string DebitAccount, string CreditAccount)
        {
            return DebitAccount == CreditAccount;
        }

        public bool IsAccountCurrencyTheSame(string DebitAccountCurrency, string CreditAccountCurrency, string TransactionCurrency, string RequestId)
        {
            bool iscurrencySame = DebitAccountCurrency == CreditAccountCurrency;

            if (!string.IsNullOrEmpty(TransactionCurrency) && iscurrencySame)
            {
                iscurrencySame = DebitAccountCurrency == TransactionCurrency && CreditAccountCurrency == TransactionCurrency;
            }
            return iscurrencySame;
        }

        public bool IsAccountBalanceLessThanAmountToTransfer(double DebitAccountBalance, double amount, LiensList[] DebitlienDetailsList = null)
        {
            double lienAmount = 0;
            if (DebitlienDetailsList != null)
            {
                lienAmount = DebitlienDetailsList.Sum(lien => Convert.ToDouble(lien.lienReleaseAmount));
            }
            double totalBalance = lienAmount + DebitAccountBalance;
            return totalBalance < amount;
        }
    }
}

