using Customers.API.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace Customers.API.Services
{
    public interface ICustomerClient
    {
        // Corporate Customer
        [Post("/corporate-customers")] 
        Task<CreateCorpCustomerResponse> CreateCorporateCustomer(CreateCorpCustomerRequest createCCRequest);

        [Get("/corporate-customers/{customerId}")] 
        Task<GetCorpCustomerResponse> GetCorporateCustomer(string customerId);

        [Get("/corporate-customers/{customerId}/{route}")]
        Task<dynamic> GetCorporateCustomer(string customerId, string route);

        // Retail Customer
        [Post("/retail-customers")] 
        Task<CreateRetailCustomerResponse> CreateRetailCustomer(CreateRetailCustomerRequest createRCRequest);

        [Get("/retail-customers/{customerId}")]
        Task<GetRetailCustomerResponse> GetRetailCustomer(string customerId);

        [Get("/retail-customers/{customerId}/{route}")]
        Task<GetCustomerAcctsResponse> GetRetailCustomer(string customerId, string route);

        // All Customers
        [Post("/v2/CustomerDtls/getCustomerDtls")]
        Task<CustomerDetailsResponse> GetCustomerDetailsByPhoneAsync([Body] Dictionary<string, string> request);


        [Get("/v2/customers/{customerId}/accounts?includeClosedAccounts={includeClosedAccounts}")]
        Task<object> GetCustomerAccounts(string customerId, string includeClosedAccounts);

        [Get("/customers/{customerId}/accounts?includeClosedAccounts={includeClosedAccounts}")]
        Task<object> GetCustomerAccountsStatements(string customerId, string includeClosedAccounts);

        [Get("/v2/customers/{customerId}/account-balances?includeClosedAccounts=Y")]
        Task<object> GetCustomerAccountsBalances(string customerId);

        [Get("/accounts/{accountId}/transactions")]
        Task<object> GetCustomerAccountTransactions(string accountId);
        [Post("/v2/StandingInstructionInquiryDtls/getStandingInstructionInquiryDtls")]
        Task<GetStandingIstructionsResponse> GetCustomerStandingInstructions(GetStandingInstructionsRequest request);
        [Post("/v2/standing-instructions/Fixed")]
        Task<PostCustomerStandingInstructionsResponse> PostCustomerStandingInstructions(PostCustomerStandingInstructionsRequest request);
    }
}
