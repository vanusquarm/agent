using Customers.API.Entities.DTOs;
using Refit;

namespace Customers.API.Services
{
    public interface ICustomerClient
    {
        // Corporate Customer
        [Post("/corporate-customers")] 
        Task<CreateCCResponse> CreateCorporateCustomer(CreateCCRequest createCCRequest);

        [Get("/corporate-customers/{customerId}")] //specify the finacle resource endpoint
        Task<GetCCResponse> GetCorporateCustomer(string customerId);

        // Retail Customer
        [Post("/retail-customers")] 
        Task<CreateRCResponse> CreateRetailCustomer(CreateRCRequest createRCRequest);

        [Get("/retail-customers/{customerId}")]
        Task<GetRCResponse> GetRetailCustomer(string customerIdentifier);

    }
}
