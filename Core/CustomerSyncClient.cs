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
            if (customerid.StartsWith("C"))
            {
                return true;
            }
            return false;
        }

        // Get customer details
        public T GetCustomer<T>(string customerId)
        {
            var response = _httpClient.GetAsync($"/api/v1/Customers/{customerId}").Result;

            response.EnsureSuccessStatusCode(); // Throws exception if response is not 2xx

            var content = response.Content.ReadAsStringAsync().Result;
            var customer = JsonSerializer.Deserialize<T>(content);

            return customer;
        }

        // Get customer relationship manager
        public RelationshipManagerDetail GetCustomerRM(string customerId)
        {
            // if accountId is provided, get the customer reference
            if (!(customerId.StartsWith("C") || customerId.StartsWith("R")))
            {
                customerId = GetAccountCustomers(customerId).customerDetail.CustomerReference;
            }
            var response = _httpClient.GetAsync($"/api/v1/Customers/{customerId}/RelationshipmanagerInfos").Result;

            response.EnsureSuccessStatusCode(); // Throws exception if response is not 2xx

            var content = response.Content.ReadAsStringAsync().Result;
            var rms = JsonSerializer.Deserialize<List<RelationshipManagerDetail>>(content);

            var primaryRM = rms.Find(e => e.PrimaryRelationshipManager == "Y");

            return primaryRM;
        }

        // Get customer employment detail
        public EmploymentDetail GetCustomerEmploymentDetail(string customerId)
        {
            var response = _httpClient.GetAsync($"/api/v1/Customers/{customerId}/employment").Result;

            response.EnsureSuccessStatusCode(); // Throws exception if response is not 2xx

            var content = response.Content.ReadAsStringAsync().Result;
            var employmentDetailResponse = JsonSerializer.Deserialize<List<EmploymentDetail>>(content);

            var employmentDetail = employmentDetailResponse.FirstOrDefault();

            return employmentDetail;
        }

        public bool IsCustomer(string phoneNumber)
        {
            var response = _httpClient.GetAsync($"/api/v1/Customers/{phoneNumber}").Result;

            response.EnsureSuccessStatusCode(); // Throws exception if response is not 2xx

            var content = response.Content.ReadAsStringAsync().Result;
            var customer = JsonSerializer.Deserialize<GetRetailCustomerResponse>(content);

            return customer.CustomerIdentifier != null;
        }

        public bool IsCustomerEmail(string customerId, string email)
        {
            var response = _httpClient.GetAsync($"/api/v1/Customers/{customerId}/emails").Result;

            response.EnsureSuccessStatusCode(); // Throws exception if response is not 2xx

            var content = response.Content.ReadAsStringAsync().Result;
            var emails = JsonSerializer.Deserialize<List<EmailDetail>>(content);
            

            return emails.Any(e => e.EmailAddress.Equals(email));
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

        public List<Account> ListDormantAccounts(string customerId)
        {
            var accounts = GetCustomerAccounts(customerId);
            var localAccounts = accounts.Where(a => a.AccountState == "D").ToList();

            return localAccounts;
        }

        public List<Account> GetCustomerLocalAccounts(string customerId)
        {
            var accounts = GetCustomerAccounts(customerId);
            var localAccounts = accounts.Where(a => a.AccountCurrency == "GHS").ToList();

            return localAccounts;
        }

        public List<Account> GetCustomerForeignAccounts(string customerId)
        {

            var accounts = GetCustomerAccounts(customerId);
            var foreignAccounts = accounts.Where(a => a.AccountCurrency != "GHS").ToList();

            return foreignAccounts;
        }

        // Get primary account of a customer
        public string GetCustomerAccount(string customerId)
        {
            var response = _httpClient.GetAsync($"/api/v1/Customers/{customerId}/accounts").Result;

            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().Result;
            var accounts = JsonSerializer.Deserialize<GetCustomerAcctsResponse>(content);

            return accounts.BasicAccountDetails[0].AccountIdentification;
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

        // Make loan inquiry of a customer
        public LoanInquiryResponse GetCustomerLoan(string customerId, string accountId)
        {
            var response = _httpClient.GetAsync($"/api/v1/Customers/{customerId}/loan").Result;

            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().Result;
            var loan = JsonSerializer.Deserialize<LoanInquiryResponse>(content);

            return loan;
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

        public CreateRetailCustomerResponseDto2 CreateRetailCustomer2(CreateRetailCustomerRequestDto2 request)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(request);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = _httpClient.PostAsync("/retail-customers/createCustomer/retail", httpContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    return JsonSerializer.Deserialize<CreateRetailCustomerResponseDto2>(content, new JsonSerializerOptions
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
