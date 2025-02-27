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
                else if (IsValidPhoneNumber(customerId, out string localNumber))
                {
                    var request = new Dictionary<string, string>
                    {
                        { "PhoneNumber", $"'{ localNumber }'" }
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
        public async Task<IActionResult> GetCustomerProps([FromRoute] string customerId, [FromRoute] string? route)
        {
            /* <summary>This is a generic endpoint to fetch specific details (properties) for a customer</summary>
             * <param name="customerId">The cif or customer reference</param>
             * <param name="route">
             * phones, emails, addresses, products, RelationshipmanagerInfos, documents, related - parties, communication-preferences,
             * product-preferences, education, employment, risk-rating, tax-details, notes, suspend, 
             * </param>
             */

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




        [HttpPost]
        public async Task<IActionResult> CreateCustomer(object customerDto, string customerType)
        {
            try
            {
                if (customerType.ToUpper() == CustomerType.Corporate)
                {
                    var customer = await customerClient.CreateCorporateCustomer((CreateCorpCustomerRequest)customerDto);
                    return Ok(customer);
                }
                else if (customerType.ToUpper() == CustomerType.Retail)
                {
                    var customer = await customerClient.CreateRetailCustomer((CreateRetailCustomerRequest)customerDto);
                    return Ok(customer);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while creating customer: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("createCustomer/{customerType}")]
        public async Task<IActionResult> CreateCustomer2(CreateRetailCustomerRequestDto2 customerDto, string customerType)
        {
            try
            {
                if (customerType.ToUpper() == CustomerType.Corporate)
                {
                 // await customerClient.CreateCorporateCustomer((CreateCorpCustomerRequest)customerDto);
                    return Ok();
                }
                else if (customerType.ToUpper() == CustomerType.Retail)
                {
                    var customer = await customerClient.CreateRetailCustomer2((CreateRetailCustomerRequestDto2) customerDto);
                    return Ok(customer);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while creating customer: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{customerId}/accounts")]
        public async Task<IActionResult> GetCustomerAccounts(string customerId, string includeClosedAccounts = "N")
        {
            try
            {
                // Check if the customerId is a phone number
                if (IsValidPhoneNumber(customerId, out string localNumber))
                {
                    var request = new Dictionary<string, string>
                    {
                        { "PhoneNumber", $"'{ localNumber }'" }
                    };
                    var customer = await customerClient.GetCustomerDetailsByPhoneAsync(request);
                    var retailCustomer = customer?.Transform();
                    customerId = retailCustomer.CustomerIdentifier;
                }
                var accounts = await customerClient.GetCustomerAccounts(customerId, includeClosedAccounts);
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while fetching customer accounts: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{customerId}/account-balances")]
        public async Task<IActionResult> GetCustomerAccountsBalances(string customerId)
        {
            try
            {
                var accountsBalances = await customerClient.GetCustomerAccountsBalances(customerId);
                return Ok(accountsBalances);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while fetching customer account balances: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{customerId}/accounts/{accountId}/transactions")]
        public async Task<IActionResult> GetCustomerAccountTransactions(string customerId, string accountId)
        {
            try
            {
                var customerExits = await GetCustomer(customerId);
                if (customerExits == null)
                {
                    return NotFound();
                }
                var accountsBalances = await customerClient.GetCustomerAccountTransactions(accountId);
                return Ok(accountsBalances);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while fetching customer transactions: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{customerId}/standing-instructions")]
        public async Task<IActionResult> GetCustomerStandingInstructions(string customerId)
        {
            try
            {
                GetStandingInstructionsRequest payload = new GetStandingInstructionsRequest();
                payload.cifId = customerId;
                var standingOrders = await customerClient.GetCustomerStandingInstructions(payload);
                return Ok(standingOrders);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        [HttpPost("standing-instructions")]
        public async Task<IActionResult> PostCustomerStandingInstructions([FromBody]PostCustomerStandingInstructionsRequest request)
        {
            try
            {
                var resp = await customerClient.PostCustomerStandingInstructions(request);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        [HttpGet("{customerId}/loan")]
        public async Task<IActionResult> GetLoanDetails(string customerId)
        {
            try
            {
                var request = new Dictionary<string, string>
                    {
                        { "customerId", customerId }
                    };
                var resp = await customerClient.GetCustomerLoan(request);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        [HttpGet("{customerId}/teller-transactions")]
        public async Task<IActionResult> GetTellerTransactions(string customerId, string? date)
        {
            try
            {   
                var request = new Dictionary<string, string>
                    {
                        { "tellerId", customerId },
                        { "tranDate", date ?? DateTime.UtcNow.ToString("yyyy-MM-dd") } 
                    };
                var resp = await customerClient.GetTellerTransactionDetails(request);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        [HttpGet("{customerId}/teller-accounts")]
        public async Task<IActionResult> GetTellerAccounts(string customerId)
        {
            try
            {
                var request = new Dictionary<string, string>
                    {
                        { "userId", customerId }
                    };
                var resp = await customerClient.GetTellerAccountDetails(request);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        private bool IsValidPhoneNumber(string phone, out string localNumber)
        {
            localNumber = null;

            try
            {
                if (string.IsNullOrWhiteSpace(phone))
                    return false;

                // Remove all non-digit characters
                string cleanPhoneNumber = Regex.Replace(phone, @"[^\d]", "");

                // Ensure it is numeric
                if (!long.TryParse(cleanPhoneNumber, out _))
                    return false;

                // Ghana local number format (Example: +233XXXXXXXXX -> 0XXXXXXXXX)
                if (cleanPhoneNumber.StartsWith("233") && cleanPhoneNumber.Length == 12)
                {
                    localNumber = "0" + cleanPhoneNumber.Substring(3);
                    return true;
                }

                // Already in local format (e.g., 024XXXXXXX)
                if (cleanPhoneNumber.Length == 10 && cleanPhoneNumber.StartsWith("0"))
                {
                    localNumber = cleanPhoneNumber;
                    return true;
                }

                return false; // Invalid format
            }
            catch (Exception)
            {
                return false;
            }
        }
        private string Clean(string input)
        {
            return Regex.Replace(input, "[^a-zA-Z0-9 ]", "");
        }
    }
}
