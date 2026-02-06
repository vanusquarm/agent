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
        // Check if customer is a corporate customer
        public bool IsCorporate(string customerid)
        {
            if (string.IsNullOrEmpty(customerid))
            {
                throw new ArgumentNullException(nameof(customerid));
            }
            return customerid.StartsWith("C");
        }

        // Get customer details
        public bool GetCustomer<T>(string customerId, out T customer, string requestId = null)
        {
            customer = default(T);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Customers/{customerId}");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) return false;

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            customer = JsonSerializer.Deserialize<T>(content);
            return true;
        }

        // Get customer details
        public T GetCustomer<T>(string customerId, string requestId = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Customers/{customerId}");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) return default(T);

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<T>(content);
        }

        // Get customer relationship manager
        public Domain.RelationshipManagerDetail GetCustomerRM(string customerId, string requestId = null)
        {
            // if accountId is provided, get the customer reference
            if (!(customerId.StartsWith("C") || customerId.StartsWith("R")))
            {
                customerId = GetAccountCustomers(customerId, requestId).customerDetail.CustomerReference;
            }
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Customers/{customerId}/RelationshipmanagerInfos");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) return null;

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var rms = JsonSerializer.Deserialize<List<Domain.RelationshipManagerDetail>>(content);

            return rms.Find(e => e.PrimaryRelationshipManager == "Y");
        }

        // Get customer employment detail
        public EmploymentDetail GetCustomerEmploymentDetail(string customerId, string requestId = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Customers/{customerId}/employment");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) return null;

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var employmentDetailResponse = JsonSerializer.Deserialize<List<EmploymentDetail>>(content);

            return employmentDetailResponse.FirstOrDefault();
        }

        public bool IsCustomer(string phoneNumber, string requestId = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Customers/{phoneNumber}");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) return false;

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var customer = JsonSerializer.Deserialize<GetRetailCustomerResponse>(content);

            return customer?.CustomerIdentifier != null;
        }

        public bool IsCustomerEmail(string customerId, string email, string requestId = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Customers/{customerId}/emails");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) return false;

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var emails = JsonSerializer.Deserialize<List<EmailDetail>>(content);

            return emails.Any(e => e.EmailAddress.Equals(email));
        }

        public string GetCustomerData(string customerId, string route, string requestId = null)
        {
            // Construct the URL
            var url = $"/api/v1/Customers/{customerId}/{route}";

            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

                var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode) return null;
                var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                return content;
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the API request
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public string GetCustomerPreferredName(string customerId, string requestId = null)
        {
            GetRetailCustomerResponse retailCustomer;
            if (GetCustomer<GetRetailCustomerResponse>(customerId, out retailCustomer, requestId) && retailCustomer.CustomerName != null)
                return retailCustomer.CustomerName;

            GetCorpCustomerResponse corpCustomer;
            if (GetCustomer<GetCorpCustomerResponse>(customerId, out corpCustomer, requestId))
                return corpCustomer.CorporateName;

            return null;
        }

        // Get accounts of a customer
        public List<Account> GetCustomerAccounts(string customerId, string requestId = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Customers/{customerId}/accounts");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) return new List<Account>();

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var accountsResponse = JsonSerializer.Deserialize<GetCustomerAcctsResponse>(content);

            return accountsResponse.BasicAccountDetails
                .Where(a => a.AccountStatus == "O" && (a.ProductType == "CAA" || a.ProductType == "SBA"))
                .ToList();
        }

        public bool GetCustomerAccounts(string customerId, out List<Account> accounts, string requestId = null)
        {
            accounts = null;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Customers/{customerId}/accounts");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) return false;

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var accountsResponse = JsonSerializer.Deserialize<GetCustomerAcctsResponse>(content);

            accounts = accountsResponse.BasicAccountDetails
                .Where(a => a.AccountStatus == "O" && (a.ProductType == "CAA" || a.ProductType == "SBA"))
                .ToList();
            return true;
        }

        public List<Account> ListDormantAccounts(string customerId, string requestId = null)
        {
            if (GetCustomerAccounts(customerId, out var accounts, requestId))
            {
                return accounts.Where(a => a.AccountState == "D").ToList();
            }
            return new List<Account>();
        }

        public List<Account> GetCustomerLocalAccounts(string customerId, string requestId = null)
        {
            if (GetCustomerAccounts(customerId, out var accounts, requestId))
            {
                return accounts.Where(a => a.AccountCurrency == "GHS").ToList();
            }
            return new List<Account>();
        }

        public List<Account> GetCustomerForeignAccounts(string customerId, string requestId = null)
        {
            if (GetCustomerAccounts(customerId, out var accounts, requestId))
            {
                return accounts.Where(a => a.AccountCurrency != "GHS").ToList();
            }
            return new List<Account>();
        }

        // Get primary account of a customer
        public string GetCustomerAccount(string customerId, string requestId = null)
        {
            if (GetCustomerAccounts(customerId, out var accounts, requestId) && accounts.Any())
            {
                return accounts[0].AccountIdentification;
            }
            return null;
        }

        // Get account balances of a customer
        public List<CustomerAccountBalance> GetCustomerAccountBalances(string customerId, string requestId = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Customers/{customerId}/account-balances");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) return new List<CustomerAccountBalance>();

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var accountsBalances = JsonSerializer.Deserialize<GetAcctBalancesResponse>(content);

            return accountsBalances.CustomerAccountBalances;
        }

        // Get transactions for a specific account of a customer
        public List<Transaction> GetAccountTransactions(string customerId, string accountId, string requestId = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Customers/{customerId}/accounts/{accountId}/transactions");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) return new List<Transaction>();

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var transactions = JsonSerializer.Deserialize<GetAcctTransactionsResponse>(content);

            return transactions.TransactionDetailsList;
        }

        // Make loan inquiry of a customer
        public LoanInquiryResponse GetCustomerLoans(string customerId, string accountId, string requestId = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Customers/{customerId}/loans");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode) return null;

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<LoanInquiryResponse>(content);
        }

        public CreateRetailCustomerResponse CreateRetailCustomer(RetailCustomerRequest request, string requestId = null)
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

                var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return JsonSerializer.Deserialize<CreateRetailCustomerResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    var errorContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
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

        public CreateRetailCustomerResponseDto2 CreateRetailCustomer2(CreateRetailCustomerRequestDto2 request, string requestId = null)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(request);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/retail-customers/createCustomer/retail")
                {
                    Content = httpContent
                };
                requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

                var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return JsonSerializer.Deserialize<CreateRetailCustomerResponseDto2>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    var errorContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
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



