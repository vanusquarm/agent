using Customers.API.Entities.DTOs;
using Refit;

namespace Customers.API.Services
{
    public interface ICustomerClient
    {
        // Corporate Customer
        [Post("/corporate-customers")] 
        Task<CreateCorpCustomerResponse> CreateCorporateCustomer(CreateCorpCustomerRequest createCCRequest);

        [Get("/corporate-customers/{customerId}")] //specify the finacle resource endpoint
        Task<GetCorpCustomerResponse> GetCorporateCustomer(string customerId);

        // Retail Customer
        [Post("/retail-customers")] 
        Task<CreateRetailCustomerResponse> CreateRetailCustomer(CreateRetailCustomerRequest createRCRequest);

        [Get("/retail-customers/{customerId}")]
        Task<GetRetailCustomerResponse> GetRetailCustomer(string customerId);

        // All Customers
        [Get("/customers/{customerId}/accounts")]
        Task<object> GetCustomerAccounts(string customerId);

        [Get("/customers/{customerId}/account-balances")]
        Task<object> GetCustomerAccountsBalances(string customerId);

        [Get("/accounts/{accountId}/transactions")]
        Task<object> GetCustomerAccountTransactions(string accountId);

    }
}
