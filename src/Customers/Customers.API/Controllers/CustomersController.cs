using Customers.API.Entities;
using Customers.API.Entities.DTOs;
using Customers.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.RegularExpressions;

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
            try
            {
                if (customerId.StartsWith("C"))
                {
                    var customer = await customerClient.GetCorporateCustomer(customerId);
                    var filteredProperty = filter == null ? customer : customer.GetType().GetProperty(filter)?.GetValue(customer);
                    return Ok(filteredProperty);
                }
                else if (customerId.StartsWith("R"))
                {
                    var customer = await customerClient.GetRetailCustomer(customerId);
                    var filteredProperty = filter == null ? customer : customer.GetType().GetProperty(filter)?.GetValue(customer);
                    return Ok(filteredProperty);
                }
                else if (IsValidPhoneNumber(customerId))
                {
                    var request = new Dictionary<string, string>
                    {
                        { "PhoneNumber", customerId }
                    };
                    var customer = await customerClient.GetCustomerDetailsByPhoneAsync(request);
                    var retailCustomer = customer?.Transform();
                    return Ok(retailCustomer);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
            
        }

        [HttpGet("{customerId}/{route?}")]
        public async Task<IActionResult> GetCustomerPhones([FromRoute] string customerId, [FromRoute] string? route)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customerId))
                {
                    return BadRequest("Customer ID cannot be null or empty.");
                }

                if (customerId.StartsWith("C"))
                {
                    var customer = await customerClient.GetCorporateCustomer(customerId, route);
                    return Ok(customer);
                }
                else if (customerId.StartsWith("R"))
                {
                    var customer = await customerClient.GetRetailCustomer(customerId, route);
                    return Ok(customer);
                }

                return BadRequest("Invalid Customer ID format. Must start with 'C' or 'R'.");
            }
            catch (Exception ex)
            {
                // Structured logging with exception details
                Console.Error.WriteLine($"Error occurred while fetching customer details: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetCustomerAccounts(string customerId)
        {
            var accounts = await customerClient.GetCustomerAccounts(customerId, "Y");
            return Ok(accounts);
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
        public async Task<IActionResult> GetCustomerAccounts(string customerId, string includeClosedAccounts = "Y")
        {
            var accounts = await customerClient.GetCustomerAccounts(customerId, includeClosedAccounts);
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

        private bool IsValidPhoneNumber(string phone)
        {

            // Remove all non-digit characters (spaces, dashes, parentheses) for general length checking
            string cleanPhoneNumber = Regex.Replace(phone, @"[^\d]", "");

            // Ensure that the phone number has the correct number of digits
            return ((cleanPhoneNumber.Length == 10 && phone.StartsWith("0")) || (cleanPhoneNumber.Length < 15 && phone.StartsWith("+") && cleanPhoneNumber.Length >= 11));

        }
    }
}
