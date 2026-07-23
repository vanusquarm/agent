using Accounts.API.Entities;
using Accounts.API.Entities.DTOs;
using Accounts.API.Entities.Models;
using Accounts.API.Services;
using Accounts.API.Validators;
using Finacle.Infrastructure.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json;


namespace Accounts.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]

    public class AccountsController(IAccountClient accountClient, AccountValidator accountValidator, IBasisClient basisFinacleConversion) : ControllerBase
    {
        /// <summary>
        /// Async Method to Get Account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetAccountAsync(string accountId)
        {
            Log.Information("Received request at GetAccountAsync: {Request}", accountId);

            var validationResult = await accountValidator.ValidateAsync(accountId);
            if (!validationResult.IsValid)
            {
                Log.Information("Account number is invalid: {Request}", accountId);
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
            }

            var strategies = new List<Func<Task<object>>>
            {
                () => accountClient.GetAccountDetails(new { accountId }),
                () => accountClient.GetSavingsAccount(accountId),
                () => accountClient.GetCurrentAccount(accountId),
                () => accountClient.GetCorpCurrentAccount(accountId)
            };

            foreach (var strategy in strategies)
            {
                try
                {
                    Log.Information("Attempting to retrieve account with strategy: {Strategy}", strategy.Method.Name);
                    var result = await strategy();
                    Log.Information("Successful response from strategy {Strategy}: {@Response}", strategy.Method.Name, result);
                    return Ok(result);
                }
                catch (Refit.ApiException apiEx)
                {
                    Log.Warning("Refit API exception in {Strategy}: {StatusCode} - {Content}", strategy.Method.Name, apiEx.StatusCode, apiEx.Content);
                    if (strategy == strategies.Last())
                    {
                        return StatusCode((int)apiEx.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: apiEx.Content));
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning("Exception in {Strategy}: {Message} | {StackTrace}", strategy.Method.Name, ex.Message, ex.StackTrace);
                }
            }

            Log.Information("Account details could not be retrieved after all failover attempts.");
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
                Log.Error("Back from getting account statements with response: {@Response}", ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
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
                Log.Error("Back from getting account statements with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
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
                Log.Error("Back from getting account statements with response: {@Response}", ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
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
                Log.Error("Back from Adding Account Restriction with response: {@Response}", ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
                return BadRequest(ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
            }
            try
            {
                Log.Information("Recieved Request: {@Request}", request);
                Log.Information("About to Add Account Restriction with request: {@Request}", request);
                var accountDetails = await accountClient.AddAccountRestriction(request);
                Log.Information("Back from Adding Account Restriction with response: {@Response}", accountDetails);

                if (accountDetails == null)
                {
                    Log.Information("Account could not be restricted: {Account}", request.AccountNumber);
                    return Ok("Account Restriction Not Successful");
                }
                return Ok(accountDetails);
            }
            catch (Refit.ApiException ex)
            {
                Log.Error("Back from Adding Account Restriction with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
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
                Log.Error("Back from Removing Account Restriction with response: {@Response}", ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
                return BadRequest(ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
            }
            try
            {
                Log.Information("Recieved Request: {@Request}", request);
                Log.Information("About to Remove Account Restriction with request: {@Request}", request);
                var accountDetails = await accountClient.RemoveAccountRestriction(request);
                Log.Information("Back from Removing Account Restriction with response: {@Response}", accountDetails);

                if (accountDetails == null)
                {
                    Log.Information("Removing Account Restriction failed: {Account}", request.CustomerId);
                    return Ok("Account Restriction Removal Not Successful");
                }
                return Ok(accountDetails);
            }
            catch (Refit.ApiException ex)
            {
                Log.Error("Back from Removing Account Restriction with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
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
                Log.Error("Back from Getting Customers Details By AccountId  with response: {@Response}", ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
                return BadRequest(ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
            }
            try
            {
                Log.Information("Recieved Request: {Request}", accountId);
                Log.Information("About to Get Customers Details By AccountId with request: {Request}", accountId);
                var customerDetails = await accountClient.GetCustomersDetailsByAccountIdAsync(new { accountId });
                Log.Information("Back from Getting Customers Details By AccountId  with response: {@Response}", customerDetails);

                if (customerDetails == null)
                {
                    Log.Information("Get Customers Details By AccountId  failed: {Account}", accountId);
                    return NotFound();
                }

                return Ok(customerDetails);
            }
            catch (Refit.ApiException ex)
            {
                Log.Error("Back from Getting Customers Details By AccountId  with response: {@Response}",
                    ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
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
                Log.Error("Back from Getting Customer Details By AccountId  with response: {@Response}", ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
                return BadRequest(ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
            }
            try
            {
                Log.Information("Recieved Request: {Request}", accountId);
                Log.Information("About to Get Customer Details By AccountId with request: {Request}", accountId);
                var customerDetails = await accountClient.GetCustomerDetailsByAccountIdAsync(new { accountNumber = accountId });
                Log.Information("Back from Getting Customer Details By AccountId  with response: {@Response}", customerDetails);

                if (customerDetails == null)
                {
                    Log.Information("Get Customers Details By AccountId  failed: {Account}", accountId);
                    return NotFound();
                }
                return Ok(customerDetails);
            }
            catch (Refit.ApiException ex)
            {
                Log.Error("Back from Getting Customer Details By AccountId  with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
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
                Log.Error("Back from Getting AccountBalances with response: {@Response}", ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
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
                        Log.Error("Back from Getting AccountBalances with response: {@Response}", ResponseManager.ReturnFailedValidations(
                            $"Invalid account type: {accountType}. Please provide a valid account type."));
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
                Log.Error("Back from Getting AccountBalances with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
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
                Log.Error("Back from Getting AccountBalance with response: {@Response}", ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
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
                Log.Error("Back from Getting AccountBalance with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
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
                Log.Error("Back from Creating SavingsAccount with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
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
                Log.Error("Back from Creating CorpCurrentAccount with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
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
                Log.Error("Back from Getting TerminalOfficeAcct with response: {@Response}", ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
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
                Log.Error("Back from Getting TerminalOfficeAcct with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
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
                Log.Error("Back from Getting AccountLiens with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
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
                Log.Error("Back from Creating AccountLiens with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }
        /// <summary>
        /// Method to get finacle account number from basis account details
        /// </summary>
        /// <param name="basisAccount"></param>
        /// <returns></returns>
        [HttpPost("basisfinacle")]
        public async Task<IActionResult> BasisFinacleAccount([FromBody] BasisAccount data)
        {
            Log.Information("Recieved Request at BasisFinacleAccount: {@Request}", data);
            try
            {
                Log.Information("About to BasisFinacleAccount with request: {@Request}", data);
                var basisFin = await basisFinacleConversion.ConvertToFinacleAccount(data);
                Log.Information("Back from BasisFinacleAccount with response: {@Response}", data);
                return Ok(basisFin);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at BasisFinacleAccount with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
