using Acccounts.API.Entities;
using Acccounts.API.Entities.DTOs;
using Acccounts.API.Services;
using Accounts.API.Validators;
using Finacle.Infrastructure.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json;


namespace Acccounts.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]

    public class AccountsController(IAccountClient accountClient, AccountValidator accountValidator) : ControllerBase
    {
        /// <summary>
        /// Async Method to Get Account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetAccountAsync(string accountId)
        {
            Log.Information("Recieved Request at GetAccountAsync: {Request}", accountId);

            var validationResult = await accountValidator.ValidateAsync(accountId);
            if (!validationResult.IsValid)
            {
                Log.Information("Account number is invalid: {Request}", accountId);
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
            }
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
                        catch (Refit.ApiException exiii)
                        {
                            Log.Error("An exception has occured trying to get Corporate Current account with message: {Message} and stack trace: {StackTrace}", exiii.Message, exiii.StackTrace);
                            Console.Error.WriteLine($"Error occurred while fetching account details: {ex.Message}");
                            return StatusCode((int)exiii.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: exiii.Content));
                        }
                    }
                }
            }
            if (accountDetails != null) { return Ok(accountDetails); }
            Log.Information("Account details could not be retrieved");
            return NotFound("Account details could not be retrieved");
        }

        /// <summary>
        /// Async Method to Get Account Statements
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("{accountId}/statements")]
        public async Task<IActionResult> GetAccountStatementsAsync([FromRoute] string accountId, [FromQuery] AccountStatementRequest request)
        {
            Log.Information("Recieved Request at GetAccountStatementsAsync: {@Request}", request);

            var validationResult = await accountValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                Log.Information("Account statement request payload is invalid: {@Request}", request);
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
            }

            try
            {
                var json = JsonSerializer.Serialize(request);
                var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                dict["accountId"] = accountId;

                Log.Information("About to get account statements with request: {@Request}", dict);
                var accountDetails = await accountClient.GetAccountStatements(dict);
                Log.Information("Back from getting account statements with response: {@Response}", accountDetails);

                if (accountDetails == null)
                {
                    Log.Information("Account statements could not be retrived for: {Account}", accountId);
                    return NotFound("No Account Statement found");
                }
                return Ok(accountDetails);
            }
            catch (Refit.ApiException ex)
            {
                Log.Error("Finacle call unsuccessful. Message: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured trying to get Account Statements with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Async Method to Get Account Statement
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{accountId}/statements")]
        public async Task<IActionResult> GetAccountStatementAsync([FromRoute] string accountId, [FromBody] AccountStatementRequest request)
        {
            Log.Information("Recieved Request at GetAccountStatementAsync: {@Request}", request);

            var validationResult = await accountValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                Log.Information("Account statement request payload is invalid: {Request}", request);
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
            }
            try
            {
                Log.Information("About to get account statements with request: {@Request}", new { accountId, fromDate = request.FromDate, toDate = request.ToDate });
                var accountDetails = await accountClient.GetAccountStatement(new { accountId, fromDate = request.FromDate, toDate = request.ToDate });
                Log.Information("Back from getting account statements with response: {@Response}", accountDetails);

                if (accountDetails == null)
                {
                    Log.Information("Account statements could not be retrived for: {Account}", accountId);
                    return Ok("No Account Statement found");
                }
                return Ok(accountDetails);
            }
            catch (Refit.ApiException ex)
            {
                Log.Error("An exception has occured trying to get Account Statements with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }

        /// <summary>
        /// Async Method to Add Account Restriction
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{accountId}/freeze")]
        public async Task<IActionResult> AddAccountRestrictionAsync(FreezeAccountRequest request)
        {
            Log.Information("Recieved Request at AddAccountRestrictionAsync: {@Request}", request);

            var validationResult = await accountValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                Log.Information("Account restriction request payload is invalid: {@Request}", request);
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
            }
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
            catch (Refit.ApiException ex)
            {
                Log.Error("An exception has occured trying to adding account restriction with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }

        /// <summary>
        /// Async Method to Remove Account Restriction
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{accountId}/unfreeze")]
        public async Task<IActionResult> RemoveAccountRestrictionAsync(UnFreezeAccountRequest request)
        {
            Log.Information("Recieved Request at RemoveAccountRestrictionAsync: {@Request}", request);

            var validationResult = await accountValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                Log.Information("Account restriction removal request payload is invalid: {@Request}", request);
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
            }
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
            catch (Refit.ApiException ex)
            {
                Log.Error("An exception has occured trying to Remove account restriction with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }

        /// <summary>
        /// Async method to Get Customers Details By AccountId 
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet("{accountId}/customers")]
        public async Task<IActionResult> GetCustomersDetailsByAccountIdAsync(string accountId)
        {
            Log.Information("Recieved Request at GetCustomersDetailsByAccountIdAsync: {Request}", accountId);

            var validationResult = await accountValidator.ValidateAsync(accountId);
            if (!validationResult.IsValid)
            {
                Log.Information("Account number is invalid: {Request}", accountId);
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
            }
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
            catch (Refit.ApiException ex)
            {
                Log.Error("An exception has occured trying to Get Customers Details By AccountId with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }

        /// <summary>
        /// Async Method Get Customer Details By AccountId
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet("{accountId}/customer")]
        public async Task<IActionResult> GetCustomerDetailsByAccountIdAsync(string accountId)
        {
            Log.Information("Recieved Request at GetCustomerDetailsByAccountIdAsync: {Request}", accountId);

            var validationResult = await accountValidator.ValidateAsync(accountId);
            if (!validationResult.IsValid)
            {
                Log.Information("Account number is invalid: {Request}", accountId);
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
            }
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
            catch (Refit.ApiException ex)
            {
                Log.Error("An exception has occured trying to Get Customer Details By AccountId with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }

        /// <summary>
        /// Method to Get Account Balances
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        [HttpGet("{accountId}/balances")]
        public async Task<IActionResult> GetAccountBalances([FromRoute] string accountId, [FromQuery] string? accountType)
        {
            Log.Information("Recieved Request at GetAccountBalances: {Request}", accountId);

            var validationResult = await accountValidator.ValidateAsync(accountId);
            if (!validationResult.IsValid)
            {
                Log.Information("Account number is invalid: {Request}", accountId);
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
            }
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
                        return BadRequest(ResponseManager.ReturnFailedValidations($"Invalid account type: {accountType}. Please provide a valid account type."));
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
            catch (Refit.ApiException ex)
            {
                Log.Error("An exception has occured at GetAccountBalances with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }

        /// <summary>
        /// Method to Get Account Balance
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet("{accountId}/balance")]
        public async Task<IActionResult> GetAccountBalance([FromRoute] string accountId, [FromQuery] string? date = null)
        {
            Log.Information("Recieved Request at GetAccountBalance: {Request}", accountId);

            var validationResult = await accountValidator.ValidateAsync(accountId);
            if (!validationResult.IsValid)
            {
                Log.Information("Account number is invalid: {Request}", accountId);
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
            }
            try
            {

                date ??= DateTime.UtcNow.ToString("yyyy-MM-dd");
                var request = new { accountNumber = accountId, date };
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
            catch (Refit.ApiException ex)
            {
                Log.Error("An exception has occured at GetAccountBalance with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }

        /// <summary>
        /// Method to Create Current Account
        /// </summary>
        /// <param name="currentAcct"></param>
        /// <returns></returns>
        [HttpPost("current-account")]
        public async Task<IActionResult> CreateCurrentAccount([FromBody] CreateCurrentAcctRequest currentAcct)
        {
            Log.Information("Recieved Request at CreateCurrentAccount: {@Request}", currentAcct);
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
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Method to Create Savings Account
        /// </summary>
        /// <param name="savingsAcct"></param>
        /// <returns></returns>
        [HttpPost("savings-account")]
        public async Task<IActionResult> CreateSavingsAccount([FromBody] CreateSavingsAcctRequest savingsAcct)
        {
            Log.Information("Recieved Request at CreateSavingsAccount: {@Request}", savingsAcct);
            try
            {
                Log.Information("About to CreateSavingsAccount with request: {@Request}", savingsAcct);
                var createSavingsAccountResponse = await accountClient.CreateSavingsAccount(savingsAcct);
                Log.Information("Back from Creating SavingsAccount with response: {@Response}", savingsAcct);
                return Ok(createSavingsAccountResponse);
            }
            catch (Refit.ApiException ex)
            {
                Log.Error("An exception has occured at CreateSavingsAccount with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }

        /// <summary>
        /// Method to Create Corporate Current Account
        /// </summary>
        /// <param name="corpCurrentAcct"></param>
        /// <returns></returns>
        [HttpPost("corporate-current-account")]
        public async Task<IActionResult> CreateCorpCurrentAccount([FromBody] CreateCorpCurrentAcctRequest corpCurrentAcct)
        {
            Log.Information("Recieved Request at CreateCorpCurrentAccount: {@Request}", corpCurrentAcct);
            try
            {
                Log.Information("About to CreateCorpCurrentAccount with request: {@Request}", corpCurrentAcct);
                var CreateCorpCurrentAccountResponse = await accountClient.CreateCorpCurrentAccount(corpCurrentAcct);
                Log.Information("Back from Creating CorpCurrentAccount with response: {@Response}", corpCurrentAcct);
                return Ok(CreateCorpCurrentAccountResponse);
            }
            catch (Refit.ApiException ex)
            {
                Log.Error("An exception has occured at CreateCorpCurrentAccount with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }

        /// <summary>
        /// Method to Get Terminal Office Account
        /// </summary>
        /// <param name="TerminalId"></param>
        /// <returns></returns>
        [HttpPost("atm-office-account")]
        public async Task<IActionResult> GetTerminalOfficeAcct([FromBody] TerminalOfficeAcctRequest request)
        {
            Log.Information("About to GetTerminalOfficeAcct with request: {@Request}", request);

            var validationResult = await accountValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                Log.Information("Terminal office account request payload is invalid: {@Request}", request);
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
            }
            try
            {
                var GetTerminalOfficeAcctResponse = await accountClient.GetTerminalOfficeAccount(request);
                Log.Information("Back from Getting TerminalOfficeAcct with response: {@Response}", GetTerminalOfficeAcctResponse);
                return Ok(GetTerminalOfficeAcctResponse);
            }
            catch (Refit.ApiException ex)
            {
                Log.Error("Finacle call unsuccessful. Message: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }

        }

        /// <summary>
        /// Async method to Get Account Liens
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="lienType"></param>
        /// <returns></returns>
        [HttpGet("{accountId}/liens/{lienType}")]
        public async Task<IActionResult> GetAccountLiensAsync([FromRoute] string accountId, [FromRoute] string lienType)
        {
            Log.Information("Recieved Request at GetAccountLiensAsync: {@Request}", (accountId, lienType));
            try
            {
                // Ensure the route parameter is mapped to the request model
                Log.Information("Recieved Request: {Request}", accountId);
                Log.Information("About to GetAccountLiens with request: {Request}", accountId);
                var accountDetails = await accountClient.GetAccountLiens(accountId, lienType);
                Log.Information("Back from Getting AccountLiens with response: {@Response}", accountDetails);

                if (accountDetails == null)
                {
                    Log.Information("No Account Lien found for:{Account}", accountId);
                    return Ok("No Account Lien found");
                }

                return Ok(accountDetails);
            }
            catch (Refit.ApiException ex)
            {
                Log.Error("An exception has occured at GetAccountLiensAsync with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }

        /// <summary>
        /// Async Method to Create Account Liens
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{accountId}/liens")]
        public async Task<IActionResult> CreateAccountLiensAsync([FromRoute] string accountId, [FromBody] LienRequestDto request)
        {
            Log.Information("Recieved Request at CreateAccountLiensAsync: {@Request}", (accountId, request));
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
            catch (Refit.ApiException ex)
            {
                Log.Error("An exception has occured at CreateAccountLiensAsync with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }
    }
}
