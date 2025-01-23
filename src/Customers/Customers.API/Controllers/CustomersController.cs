using Customers.API.Entities;
using Customers.API.Entities.DTOs;
using Customers.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Customers.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    //[Authorize]
    public class CustomersController(ICustomerClient customerClient) : ControllerBase
    {

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomer(string customerId, string? filter = null)
        {
            if (customerId.StartsWith("C"))
            {
                var customer = await customerClient.GetCorporateCustomer(customerId);
                var filteredProperty = filter == null? customer: customer.GetType().GetProperty(filter)?.GetValue(customer);
                return Ok(filteredProperty);
            }
            else if (customerId.StartsWith("R"))
            {
                var customer = await customerClient.GetRetailCustomer(customerId);
                var filteredProperty = filter == null ? customer : customer.GetType().GetProperty(filter)?.GetValue(customer);
                return Ok(filteredProperty);
            }
            else
            {
                return NotFound();
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateCustomer(object customerDto, string customerType)
        {
            if (customerType.ToUpper() == CustomerType.Corporate)
            {
                var customer = await customerClient.CreateCorporateCustomer((CreateCorpCustomerRequest) customerDto);
                return Ok(customer);
            }
            else if (customerType.ToUpper() == CustomerType.Retail)
            {
                var customer = await customerClient.CreateRetailCustomer((CreateRetailCustomerRequest) customerDto);
                return Ok(customer);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{customerId}/accounts")]
        public async Task<IActionResult> GetCustomerAccounts(string customerId)
        {
            var accounts = await customerClient.GetCustomerAccounts(customerId);
            return Ok(accounts);
        }

        [HttpGet("{customerId}/account-balances")]
        public async Task<IActionResult> GetCustomerAccountsBalances(string customerId)
        {
            var accountsBalances = await customerClient.GetCustomerAccountsBalances(customerId);
            return Ok(accountsBalances);
        }

        [HttpGet("{customerId}/accounts/{accountId}/transactions")]
        public async Task<IActionResult> GetCustomerAccountTransactions(string customerId, string accountId)
        {
            var customerExits = await GetCustomer(customerId);
            if (customerExits == null)
            {
                return NotFound();
            }
            var accountsBalances = await customerClient.GetCustomerAccountTransactions(accountId);
            return Ok(accountsBalances);
        }
    }
}
