using Acccounts.API.Entities;
using Acccounts.API.Entities.DTOs;
using Acccounts.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Acccounts.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    //[Authorize]
    public class AccountsController(IAccountClient accountClient) : ControllerBase
    {
        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetAccountAsync(string accountId)
        {
            var accountDetails = new object();
            try
            {
                Log.Information("Recieved Request: {Request}", accountId);
                Log.Information("About to get account details with request: {Request}", accountId);
                accountDetails = await accountClient.GetAccountDetails(new { accountId });
                Log.Information("Back from getting account details with response: {@Response}", accountDetails);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured trying to get account details with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                try
                {
                    Log.Information("About to get savings account with request: {Request}", accountId);
                    accountDetails = await accountClient.GetSavingsAccount(accountId);
                    Log.Information("Back from getting savings account with response: {@Response}", accountDetails);
                }
                catch (Exception exi)
                {
                    Log.Error("An exception has occured trying to get savings account with message: {Message} and stack trace: {StackTrace}", exi.Message, exi.StackTrace);
                    try
                    {
                        Log.Information("About to get current account with request: {Request}", accountId);
                        accountDetails = await accountClient.GetCurrentAccount(accountId);
                        Log.Information("Back from getting current account with response: {@Response}", accountDetails);
                    }
                    catch (Exception exii)
                    {
                        Log.Error("An exception has occured trying to get current account with message: {Message} and stack trace: {StackTrace}", exii.Message, exii.StackTrace);
                        try
                        {
                            Log.Information("About to get Corporate Current account with request: {Request}", accountId);
                            accountDetails = await accountClient.GetCorpCurrentAccount(accountId);
                            Log.Information("Back from getting Corporate Current account with response: {@Response}", accountDetails);
                        }
                        catch (Exception exiii)
                        {
                            Log.Error("An exception has occured trying to get Corporate Current account with message: {Message} and stack trace: {StackTrace}", exiii.Message, exiii.StackTrace);
                            Console.Error.WriteLine($"Error occurred while fetching account details: {ex.Message}");
                            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
                        }
                    }
                }
            }
            if (accountDetails != null) { return Ok(accountDetails); }
            Log.Information("Account details could not be retrieved");
            return NotFound();
        }

        [HttpGet("{accountId}/statements")]
        public async Task<IActionResult> GetAccountStatementsAsync(
            [FromRoute] string accountId,
            [FromQuery] AccountStatementRequest request)
        {
            try
            {
                Log.Information("Recieved Request: {@Request}", request);                
                // Ensure the route parameter is mapped to the request model
                request.AccountId = accountId;

                Log.Information("About to get account statements with request: {@Request}", request);
                var accountDetails = await accountClient.GetAccountStatements(request);
                Log.Information("Back from getting account statements with response: {@Response}", accountDetails);

                if (accountDetails == null)
                {
                    Log.Information("Account statements could not be retrived for: {Account}", accountId);
                    return Ok("No Account Statement found");
                }
                return Ok(accountDetails);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured trying to get Account Statements with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.Error.WriteLine($"Error occurred while fetching account statements: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("{accountId}/statements")]
        public async Task<IActionResult> GetAccountStatementAsync(
            [FromRoute] string accountId,
            [FromBody] AccountStatementRequest request)
        {
            try
            {                
                // Ensure the route parameter is mapped to the request model
                request.AccountId = accountId;

                Log.Information("Recieved Request: {@Request}", request);

                Log.Information("About to get account statements with request: {@Request}", request);
                var accountDetails = await accountClient.GetAccountStatement(request);
                Log.Information("Back from getting account statements with response: {@Response}", accountDetails);

                if (accountDetails == null)
                {
                    Log.Information("Account statements could not be retrived for: {Account}", accountId);
                    return Ok("No Account Statement found");
                }
                return Ok(accountDetails);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured trying to get Account Statements with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.Error.WriteLine($"Error occurred while fetching account statements: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }


        [HttpPost("{accountId}/freeze")]
        public async Task<IActionResult> AddAccountRestrictionAsync(FreezeAccountRequest request)
        {
            try
            {
                Log.Information("Recieved Request: {@Request}", request);

                Log.Information("About to Add Account Restriction with request: {@Request}", request);
                var accountDetails = await accountClient.AddAccountRestriction(request);
                Log.Information("Back from Adding Account Restriction with request: {@Request}", request);

                if (accountDetails == null)
                {
                    Log.Information("Account could not be restricted: {Account}", request.AccountNumber);
                    return Ok("Account Restriction Not Successful");
                }

                return Ok(accountDetails);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured trying to adding account restriction with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.Error.WriteLine($"Error occurred while adding account restriction: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("{accountId}/unfreeze")]
        public async Task<IActionResult> RemoveAccountRestrictionAsync(UnFreezeAccountRequest request)
        {
            try
            {
                Log.Information("Recieved Request: {@Request}", request);

                Log.Information("About to Remove Account Restriction with request: {@Request}", request);
                var accountDetails = await accountClient.RemoveAccountRestriction(request);
                Log.Information("Back from Removing Account Restriction with request: {@Request}", request);

                if (accountDetails == null)
                {
                    Log.Information("Removing Account Restriction failed: {Account}", request.CustomerId);
                    return Ok("Account Restriction Removal Not Successful");
                }
                return Ok(accountDetails);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured trying to Remove account restriction with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.Error.WriteLine($"Error occurred while removing account restriction: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{accountId}/customers")]
        public async Task<IActionResult> GetCustomersDetailsByAccountIdAsync(string accountId)
        {
            try
            {
                Log.Information("Recieved Request: {Request}", accountId);

                Log.Information("About to Get Customers Details By AccountId with request: {Request}", accountId);
                var customerDetails = await accountClient.GetCustomersDetailsByAccountIdAsync(new { accountId });
                Log.Information("Back from Getting Customers Details By AccountId  with request: {@Request}", customerDetails);

                if (customerDetails == null)
                {
                    Log.Information("Get Customers Details By AccountId  failed: {Account}", accountId);
                    return NotFound();
                }

                return Ok(customerDetails);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured trying to Get Customers Details By AccountId with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.Error.WriteLine($"Error occurred while fetching customers of the account {accountId}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{accountId}/customer")]
        public async Task<IActionResult> GetCustomerDetailsByAccountIdAsync(string accountId)
        {
            try
            {
                Log.Information("Recieved Request: {Request}", accountId);

                Log.Information("About to Get Customer Details By AccountId with request: {Request}", accountId);
                var customerDetails = await accountClient.GetCustomerDetailsByAccountIdAsync(new { accountNumber = accountId });
                Log.Information("Back from Getting Customer Details By AccountId  with request: {@Request}", customerDetails);

                if (customerDetails == null)
                {
                    Log.Information("Get Customers Details By AccountId  failed: {Account}", accountId);
                    return NotFound();
                }

                return Ok(customerDetails);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured trying to Get Customer Details By AccountId with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.Error.WriteLine($"Error occurred while fetching customers of the account {accountId}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{accountId}/balances")]
        public async Task<IActionResult> GetAccountBalances([FromRoute] string accountId, [FromQuery] string? accountType)
        {
            try
            {
                Log.Information("Recieved Request: {Request}", accountId);
                
                // Validate accountType using the enum
                if (string.IsNullOrEmpty(accountType)) { accountType = null; }
                else
                {
                    if (!Enum.IsDefined(typeof(AccountType), accountType))
                    {
                        Log.Information("Invalid account type for: {Request}", accountId);
                        return BadRequest($"Invalid account type: {accountType}. Please provide a valid account type.");
                    }
                }

                Log.Information("About to GetAccountBalances with request: {Request}", accountId);
                var balances = await accountClient.GetAccountBalances(accountId);
                Log.Information("Back from Getting AccountBalances with response: {@Response}", balances);

                // Check if the balances object is null or does not contain any balances
                if (balances == null || balances.Balances == null || !balances.Balances.Any())
                {
                    Log.Information("No balances found for the specified account for: {Account}", accountId);
                    return NotFound("No balances found for the specified account.");
                }

                //filter for a specific account here

                // Look for the specific balance matching the accountType
                var balance = balances.Balances.FirstOrDefault(b => b.BalanceType == accountType);
                if (balance != null) { return Ok(balance); }

                // If no balance matches the accountType, return all balances
                return Ok(balances);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at GetAccountBalances with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                return NoContent();
            }
        }

        [HttpGet("{accountId}/balance")]
        public async Task<IActionResult> GetAccountBalance([FromRoute] string accountId, [FromQuery] string? date = null)
        {
            try
            {
                Log.Information("Recieved Request: {Request}", accountId);                
                if (string.IsNullOrEmpty(accountId))
                {
                    Log.Information("Account number is null and is required");
                    return BadRequest("Account number is required.");
                }

                date ??= DateTime.UtcNow.ToString("yyyy-MM-dd");

                var request = new { accountNumber = accountId, date  };

                Log.Information("About to GetAccountBalance with request: {@Request}", request);
                var balance = await accountClient.GetAccountBalance(request);
                Log.Information("Back from Getting AccountBalance with response: {@Response}", balance);

                if (balance == null)
                {
                    Log.Information("No balance found on {date} for the specified account", date);
                    return NotFound($"No balance found on {date} for the specified account");
                }

                return Ok(balance);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at GetAccountBalance with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                return NoContent();
            }
        }

        [HttpPost("current-account")]
        public async Task<IActionResult> CreateCurrentAccount([FromBody] CreateCurrentAcctRequest currentAcct)
        {
            try
            {
                Log.Information("About to CreateCurrentAccount with request: {@Request}", currentAcct);
                var createCurrentAccountResponse = await accountClient.CreateCurrentAccount(currentAcct);
                Log.Information("Back from Creating CurrentAccount with response: {@Response}", currentAcct);
                return Ok(createCurrentAccountResponse);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at CreateCurrentAccount with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.Error.WriteLine($"Error occurred while creating current account: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("savings-account")]
        public async Task<IActionResult> CreateSavingsAccount([FromBody] CreateSavingsAcctRequest savingsAcct)
        {
            try
            {
                Log.Information("About to CreateSavingsAccount with request: {@Request}", savingsAcct);
                var createSavingsAccountResponse = await accountClient.CreateSavingsAccount(savingsAcct);
                Log.Information("Back from Creating SavingsAccount with response: {@Response}", savingsAcct);
                return Ok(createSavingsAccountResponse);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at CreateSavingsAccount with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.Error.WriteLine($"Error occurred while creating savings account: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("corporate-current-account")]
        public async Task<IActionResult> CreateCorpCurrentAccount([FromBody] CreateCorpCurrentAcctRequest corpCurrentAcct)
        {
            try
            {
                Log.Information("About to CreateCorpCurrentAccount with request: {@Request}", corpCurrentAcct);
                var CreateCorpCurrentAccountResponse = await accountClient.CreateCorpCurrentAccount(corpCurrentAcct);
                Log.Information("Back from Creating CorpCurrentAccount with response: {@Response}", corpCurrentAcct);
                return Ok(CreateCorpCurrentAccountResponse);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at CreateCorpCurrentAccount with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.Error.WriteLine($"Error occurred while creating corporate current account: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("atm-office-account")]
        public async Task<IActionResult> GetTerminalOfficeAcct([FromBody] TerminalOfficeAcctRequest request)
        {
            try
            {
                //var request = new { TerminalId };
                //var request = new TerminalOfficeAcctRequest { TerminalId = TerminalId };
                Log.Information("About to GetTerminalOfficeAcct with request: {@Request}", request);
                var GetTerminalOfficeAcctResponse = await accountClient.GetTerminalOfficeAccount(request);
                Log.Information("Back from Getting TerminalOfficeAcct with response: {@Response}", GetTerminalOfficeAcctResponse);
                return Ok(GetTerminalOfficeAcctResponse);
            }
            catch (Refit.ApiException apiEx)
            {
                return StatusCode((int)apiEx.StatusCode, apiEx.Content ?? apiEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }

        }

        //Dealing with liens      

        [HttpGet("{accountId}/liens/{lienType}")]
        public async Task<IActionResult> GetAccountLiensAsync(
           [FromRoute] string accountId,
           [FromRoute] string lienType)
        {
            try
            {
                // Ensure the route parameter is mapped to the request model
                Log.Information("Recieved Request: {Request}", accountId);
                Log.Information("About to GetAccountLiens with request: {Request}", accountId);
                var accountDetails = await accountClient.GetAccountLiens(accountId,lienType);
                Log.Information("Back from Getting AccountLiens with response: {@Response}", accountDetails);

                if (accountDetails == null)
                {
                    Log.Information("No Account Lien found for:{Account}", accountId);
                    return Ok("No Account Lien found");
                }

                return Ok(accountDetails);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at GetAccountLiensAsync with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.Error.WriteLine($"Error occurred while fetching account liens: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("{accountId}/liens")]
        public async Task<IActionResult> CreateAccountLiensAsync(
          [FromRoute] string accountId,
          [FromBody] LienRequestDto request)
        {
            try
            {
                // Ensure the route parameter is mapped to the request model
                Log.Information("Recieved Request: {@Request}", request);
                Log.Information("About to CreateAccountLiens with request: {@Request}", request);
                var accountDetails = await accountClient.CreateAccountLiens(accountId, request);
                Log.Information("Back from Creating AccountLiens with response: {@Response}", accountDetails);

                if (accountDetails == null)
                {
                    Log.Information("Account Lien creation failed:{Account}", accountDetails);
                    return Ok("No Account Lien creation failed");
                }

                return Ok(accountDetails);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at CreateAccountLiensAsync with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.Error.WriteLine($"Error occurred while creating account liens: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        private string Clean(string input)
        {
            return Regex.Replace(input, "[^a-zA-Z0-9 ]", "");
        }
    }
}
