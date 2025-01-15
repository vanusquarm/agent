using Customers.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Customers.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    //[Authorize]
    public class CustomersController(ICustomerClient customerClient) : ControllerBase
    {

        [HttpGet("{customerIdentifier}")]
        public async Task<IActionResult> GetCustomer(string customerIdentifier)
        {
            if (customerIdentifier.StartsWith("C"))
            {
                var customer = await customerClient.GetCorporateCustomer(customerIdentifier);
                return Ok(customer);
            }
            else if (customerIdentifier.StartsWith("R"))
            {
                var customer = await customerClient.GetRetailCustomer(customerIdentifier);
                return Ok(customer);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
