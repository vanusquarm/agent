using Customers.API.Entities;
using Customers.API.Entities.DTOs;
using Customers.API.Features;
using Customers.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Customers.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]

    public class CustomersController(ICustomerClient customerClient) : ControllerBase
    {
        /// <summary>
        /// Method to get Get Customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomer(string customerId, string? filter = null)
        {
            Log.Information("Recieved Request in GetCustomer method: {Request}", customerId);
            try
            {
                if (customerId.StartsWith("C"))
                {
                    Log.Information("cif starts with C. Now about to GetCorporateCustomer with request: {Request}", customerId);
                    var customer = await customerClient.GetCorporateCustomer(customerId);
                    Log.Information("Back from Getting CorporateCustomer with response: {@Response}", customer);

                    var filteredProperty = filter == null ? customer : customer.GetType().GetProperty(filter)?.GetValue(customer);
                    Log.Information("Response after filtering Corporate: {@Response}", filteredProperty);
                    return Ok(filteredProperty);
                }
                else if (customerId.StartsWith("R"))
                {
                    Log.Information("cif starts with R. Now about to GetRetailCustomer with request: {Request}", customerId);
                    var customer = await customerClient.GetRetailCustomer(customerId);
                    Log.Information("Back from Getting RetailCustomer with response: {@Response}", customer);

                    var filteredProperty = filter == null ? customer : customer.GetType().GetProperty(filter)?.GetValue(customer);
                    Log.Information("Response after filtering Retail customers: {@Response}", filteredProperty);
                    return Ok(filteredProperty);
                }
                else if (AccountHelper.IsValidPhoneNumber(customerId, out string localNumber))
                {
                    Log.Information("phone numer is valid: {Request}", customerId);
                    var request = new Dictionary<string, string>
                    {
                        { "PhoneNumber", $"'{ localNumber }'" }
                    };

                    Log.Information("About to GetCustomerDetailsByPhone with request: {Request}", request);
                    var customer = await customerClient.GetCustomerDetailsByPhoneAsync(request);
                    Log.Information("Back from Getting CustomerDetailsByPhone with response: {@Response}", customer);                    
                    var preferredCustomer = customer.CustomerDetails[0].CifId;
                    var retailCustomer = await customerClient.GetRetailCustomer(preferredCustomer);
                    Log.Information("Retail customers is: {@Response}", retailCustomer);
                    return Ok(retailCustomer);
                }
                else
                {
                    Log.Information("No record found for customer: {Customer}", customerId);
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at GetCustomer with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.WriteLine(ex.Message);
                return NotFound();
            }
            
        }

        /* <summary>This is a generic endpoint to fetch specific details (properties) for a customer</summary>
         * <param name="customerId">The cif or customer reference</param>
         * <param name="prop">
         * phones, emails, addresses, products, RelationshipmanagerInfos, documents, related - parties, communication-preferences,
         * product-preferences, education, employment, risk-rating, tax-details, notes, suspend, 
         * </param>
         */
        [HttpGet("{customerId}/{prop?}")]
        public async Task<IActionResult> GetCustomerProps([FromRoute] string customerId, [FromRoute] string? prop)
        {
            Log.Information("Recieved Request in GetCustomer method: {Request}", customerId);

            try
            {
                if (string.IsNullOrWhiteSpace(customerId))
                {
                    Log.Information("The request is invalid as customer Id is null or empty: {Request}", customerId);
                    return BadRequest("Customer ID cannot be null or empty.");
                }

                if (customerId.StartsWith("C"))
                {
                    Log.Information("cif starts with C. Now about to GetCorporateCustomer with request: {@Request}", (customerId, prop));
                    var customer = await customerClient.GetCorporateCustomer(customerId, prop);
                    Log.Information("Back from Getting CorporateCustomer with response: {@Response}", customer);
                    return Ok(customer); 
                }
                else if (customerId.StartsWith("R"))
                {
                    Log.Information("cif starts with R. Now about to GetRetailCustomer with request: {Request}", customerId);
                    var customer = await customerClient.GetRetailCustomer(customerId, prop);
                    Log.Information("Back from Getting RetailCustomer with response: {@Response}", customer);
                    return Ok(customer);
                }

                Log.Information("Invalid Customer ID format. Must start with 'C' or 'R': {Request}", customerId);
                return BadRequest("Invalid Customer ID format. Must start with 'C' or 'R'.");
            }
            catch (Exception ex)
            {
                // Structured logging with exception details
                Log.Error("An exception has occured at GetCustomerProps with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.Error.WriteLine($"Error occurred while fetching customer details: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Method to Create Customer
        /// </summary>
        /// <param name="customerDto"></param>
        /// <param name="customerType"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(object customerDto, string customerType)
        {
            try
            {
                if (customerType.ToUpper() == CustomerType.Corporate)
                {
                    Log.Information("Corporate. Now about to CreateCorporateCustomer with request: {@Request}", (customerDto, customerType));
                    var customer = await customerClient.CreateCorporateCustomer((CreateCorpCustomerRequest)customerDto);
                    Log.Information("Back from Creating CorporateCustomer with response: {@Response}", customer); 
                    return Ok(customer);
                }
                else if (customerType.ToUpper() == CustomerType.Retail)
                {
                    Log.Information("Retail. Now about to CreateRetailCustomer with request: @Request}", (customerDto, customerType));
                    var customer = await customerClient.CreateRetailCustomer((CreateRetailCustomerRequest)customerDto);
                    Log.Information("Back from Creating RetailCustomer with response: {@Response}", customer);
                    return Ok(customer);
                }
                else
                {
                    Log.Information("No record found for request: {@Request}", (customerDto, customerType));
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at CreateCustomer with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.Error.WriteLine($"Error occurred while creating customer: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Method to Create Customer2
        /// </summary>
        /// <param name="customerDto"></param>
        /// <param name="customerType"></param>
        /// <returns></returns>
        [HttpPost("createCustomer/{customerType}")]
        public async Task<IActionResult> CreateCustomer2(CreateRetailCustomerRequestDto2 customerDto, string customerType)
        {
            try
            {
                if (customerType.ToUpper() == CustomerType.Corporate)
                {
                    Log.Information("Corporate. Now about to CreateCorporateCustomer with request: {@Request}", (customerDto, customerType));
                    //await customerClient.CreateCorporateCustomer((CreateCorpCustomerRequest)customerDto);
                    Log.Information("Back from Creating CorporateCustomer with response: {@Response}");
                    return Ok();
                }
                else if (customerType.ToUpper() == CustomerType.Retail)
                {
                    Log.Information("Retail. Now about to CreateRetailCustomer2 with request: @Request}", (customerDto, customerType));
                    var customer = await customerClient.CreateRetailCustomer2((CreateRetailCustomerRequestDto2) customerDto);
                    Log.Information("Back from Creating RetailCustomer2 with response: {@Response}", customer);
                    return Ok(customer);
                }
                else
                {
                    Log.Information("No record found for request: {@Request}", (customerDto, customerType));
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at CreateCustomer2 with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.Error.WriteLine($"Error occurred while creating customer: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Method to Get Customer Accounts
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="includeClosedAccounts"></param>
        /// <returns></returns>
        [HttpGet("{customerId}/accounts")]
        public async Task<IActionResult> GetCustomerAccounts(string customerId, string includeClosedAccounts = "N")
        {
            try
            {
                // Check if the customerId is a phone number
                if (AccountHelper.IsValidPhoneNumber(customerId, out string localNumber))
                {
                    Log.Information("phone numer is valid: {Request}", customerId);
                    var request = new Dictionary<string, string>
                    {
                        { "PhoneNumber", $"'{ localNumber }'" }
                    };

                    Log.Information("About to GetCustomerDetailsByPhone with request: {Request}", request);
                    var customer = await customerClient.GetCustomerDetailsByPhoneAsync(request);
                    Log.Information("Back from Getting CustomerDetailsByPhone with response: {@Response}", customer);

                    var retailCustomer = customer?.Transform();
                    Log.Information("Retail customers is: {@Response}", retailCustomer);
                    customerId = retailCustomer.CustomerIdentifier;
                }

                Log.Information("About to GetCustomerAccounts with request: {Request}", customerId);
                var accounts = await customerClient.GetCustomerAccounts(customerId, includeClosedAccounts);
                Log.Information("Back from Getting CustomerAccounts with response: {@Response}", accounts);

                return Ok(accounts);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at GetCustomerAccounts with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.Error.WriteLine($"Error occurred while fetching customer accounts: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Method to Get Customer Accounts Balances
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet("{customerId}/account-balances")]
        public async Task<IActionResult> GetCustomerAccountsBalances(string customerId)
        {
            try
            {
                Log.Information("About to GetCustomerAccountsBalances with request: {Request}", customerId);
                var accountsBalances = await customerClient.GetCustomerAccountsBalances(customerId);
                Log.Information("Back from Getting CustomerAccountsBalances with response: {@Response}", accountsBalances);
                return Ok(accountsBalances);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at GetCustomerAccountsBalances with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.Error.WriteLine($"Error occurred while fetching customer account balances: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Method to Get Customer Account Transactions
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet("{customerId}/accounts/{accountId}/transactions")]
        public async Task<IActionResult> GetCustomerAccountTransactions(string customerId, string accountId)
        {
            try
            {
                Log.Information("About to GetCustomer with request: {Request}", customerId);
                var customerExits = await GetCustomer(customerId);
                Log.Information("Back from Getting Customer with response: {@Response}", customerExits);
                
                if (customerExits == null)
                {
                    Log.Information("No record found for request: {@Request}", (customerId, accountId));
                    return NotFound();
                }

                Log.Information("Corporate. About to GetCustomerAccountTransactions with request: {Request}", accountId);
                var accountsBalances = await customerClient.GetCustomerAccountTransactions(accountId);
                Log.Information("Corporate. Back from Getting CustomerAccountTransactions with response: {@Response}", accountsBalances);

                return Ok(accountsBalances);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at GetCustomerAccountTransactions with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.Error.WriteLine($"Error occurred while fetching customer transactions: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Method to Get Customer Standing Instructions 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet("{customerId}/standing-instructions")]
        public async Task<IActionResult> GetCustomerStandingInstructions(string customerId)
        {
            Log.Information("Recieved Request at GetCustomerStandingInstructions: {Request}", customerId);
            try
            {
                GetStandingInstructionsRequest payload = new GetStandingInstructionsRequest();
                payload.cifId = customerId;

                Log.Information("About to GetCustomerStandingInstructions with request: {@Request}", payload);
                var standingOrders = await customerClient.GetCustomerStandingInstructions(payload);
                Log.Information("Back from Getting CustomerStandingInstructions with response: {@Response}", standingOrders);

                return Ok(standingOrders);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at GetCustomerAccountTransactions with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        /// <summary>
        /// Method to Post Customer Standing Instructions
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("standing-instructions")]
        public async Task<IActionResult> PostCustomerStandingInstructions([FromBody]PostCustomerStandingInstructionsRequest request)
        {
            Log.Information("Recieved Request at PostCustomerStandingInstructions: {@Request}", request);
            try
            {
                Log.Information("About to PostCustomerStandingInstructions with request: {@Request}", request);
                var resp = await customerClient.PostCustomerStandingInstructions(request);
                Log.Information("Back from Posting CustomerStandingInstructions with response: {@Response}", resp);

                return Ok(resp);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at PostCustomerStandingInstructions with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        /// <summary>
        /// Method to Delete Customer Standing Instructions
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete("standing-instructions")]
        public async Task<IActionResult> DeleteCustomerStandingInstruction([FromBody] CancelCustomerStandingInstructionRequest request)
        {
            Log.Information("Recieved Request at PostCustomerStandingInstructions: {@Request}", request);
            try
            {
                Log.Information("About to PostCustomerStandingInstructions with request: {@Request}", request);
                var resp = await customerClient.DeleteCustomerStandingInstruction(request);
                Log.Information("Back from Posting CustomerStandingInstructions with response: {@Response}", resp);

                return Ok(resp);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at PostCustomerStandingInstructions with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }
        /// <summary>
        /// Method to Get Loan Details
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet("{customerId}/loan")]
        public async Task<IActionResult> GetLoanDetails(string customerId)
        {
            Log.Information("Recieved Request at GetLoanDetails: {Request}", customerId);
            try
            {
                var request = new Dictionary<string, string> { { "customerId", customerId } };

                Log.Information("About to GetLoanDetails with request: {@Request}", request);
                var resp = await customerClient.GetCustomerLoan(request);
                Log.Information("Back from Getting LoanDetails with response: {@Response}", resp);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at GetLoanDetails with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        /// <summary>
        /// Method to Get Teller Transactions
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet("{customerId}/teller-transactions")]
        public async Task<IActionResult> GetTellerTransactions(string customerId, string? date)
        {
            Log.Information("Recieved Request at GetTellerTransactions: {Request}", customerId);
            try
            {   
                var request = new Dictionary<string, string> { { "tellerId", customerId }, { "tranDate", date ?? DateTime.UtcNow.ToString("yyyy-MM-dd") }  };
                Log.Information("About to GetTellerTransactionDetails with request: {@Request}", request);
                var resp = await customerClient.GetTellerTransactionDetails(request);
                Log.Information("Back from Getting TellerTransactionDetails with response: {@Response}", resp);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at GetTellerTransactions with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        /// <summary>
        /// Method to Get Teller Accounts
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet("{customerId}/teller-accounts")]
        public async Task<IActionResult> GetTellerAccounts(string customerId)
        {
            Log.Information("Recieved Request at GetTellerAccounts: {Request}", customerId);
            try
            {
                var request = new Dictionary<string, string> { { "userId", customerId } };
                Log.Information("About to GetTellerAccountDetails with request: {@Request}", request);
                var resp = await customerClient.GetTellerAccountDetails(request);
                Log.Information("Back from Getting TellerAccountDetails with response: {@Response}", resp);

                if(resp.ErrorFlg == "N") return Ok(resp);
                else return NotFound();
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at GetTellerAccounts with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }   
    }
}
