using Customers.API.Entities.DTOs;
using Refit;

namespace Customers.API.Services
{
    public interface ICorporateCustomerClient
    {
        [Get("/corporate-customers")] //specify the finacle resource endpoint
        Task<CreateCCResponse> CreateCorporateCustomer(CreateCCRequest createCCRequest);
    }
}
