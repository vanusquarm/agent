using Acccounts.API.Entities;
using Acccounts.API.Entities.DTOs;
using Acccounts.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

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
            var accountDetails=new object();
            try
            {
                accountDetails = await accountClient.GetAccountDetails(new { accountId });

               
            }
            catch (Exception ex)
            {
                /

                try
                {
                    accountDetails = await accountClient.GetSavingsAccount(accountId);
                }
                catch (Exception exi)
                {
                    try
                    {
                        accountDetails = await accountClient.GetCurrentAccount(accountId);
                    }
                    catch (Exception exii)
                    {
                        try
                        {
                          accountDetails=await accountClient.GetCorpCurrentAccount(accountId);
                        }
                        catch(Exception exiii) {
                          Console.Error.WriteLine($"Error occurred while fetching account details: {ex.Message}");
                            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
                        }
                    }

                }
                   
            }

            if (accountDetails != null) {
                return Ok(accountDetails);
            }

            return NotFound();

        }

        [HttpGet("{accountId}/statements")]
        public async Task<IActionResult> GetAccountStatementsAsync(
            [FromRoute] string accountId,
            [FromQuery] AccountStatementRequest request)
        {
            try
            {
                // Ensure the route parameter is mapped to the request model
                request.AccountId = accountId;

                var accountDetails = await accountClient.GetAccountStatements(request);

                if (accountDetails == null)
                {
                    return Ok("No Account Statement found");
                }

                return Ok(accountDetails);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while fetching account statements: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }


        [HttpPost("{accountId}/freeze")]
        public async Task<IActionResult> AddAccountRestrictionAsync(FreezeAccountRequest request)
        {
            try
            {
                var accountDetails = await accountClient.AddAccountRestriction(request);

                if (accountDetails == null)
                {
                    return Ok("Account Restriction Not Successful");
                }

                return Ok(accountDetails);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while adding account restriction: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("{accountId}/unfreeze")]
        public async Task<IActionResult> RemoveAccountRestrictionAsync(UnFreezeAccountRequest request)
        {
            try
            {
                var accountDetails = await accountClient.RemoveAccountRestriction(request);

                if (accountDetails == null)
                {
                    return Ok("Account Restriction Removal Not Successful");
                }

                return Ok(accountDetails);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while removing account restriction: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{accountId}/customers")]
        public async Task<IActionResult> GetCustomerDetailsByAccountIdAsync(string accountId)
        {
            try
            {
                var customerDetails = await accountClient.GetCustomerDetailsByAccountIdAsync(new { accountId });

                if (customerDetails == null)
                {
                    return NotFound();
                }

                return Ok(customerDetails);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while fetching customers of the account {accountId}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{accountId}/balances")]
        public async Task<IActionResult> GetAccountBalance([FromRoute] string accountId, [FromQuery] string? accountType)
        {
            try
            {
                // Validate accountType using the enum
                if (string.IsNullOrEmpty(accountType))
                {
                    accountType = null;
                }
                else 
                {
                    if (!Enum.IsDefined(typeof(AccountType), accountType))
                    {
                        return BadRequest($"Invalid account type: {accountType}. Please provide a valid account type.");
                    }
                }
               
                var balances = await accountClient.GetAccountBalance(accountId);
                // Check if the balances object is null or does not contain any balances
                if (balances == null || balances.Balances == null || !balances.Balances.Any())
                {
                    return NotFound("No balances found for the specified account.");
                }

                //filter for a specific account here

                // Look for the specific balance matching the accountType
                var balance = balances.Balances.FirstOrDefault(b => b.BalanceType == accountType);

                if (balance != null)
                {
                    return Ok(balance);
                }

                // If no balance matches the accountType, return all balances
                return Ok(balances);
            }
            catch (Exception ex)
            {
                return NoContent();
            }
        }

        [HttpPost("current-account")]
        public async Task<IActionResult> CreateCurrentAccount([FromBody] CreateCurrentAcctRequest currentAcct)
        {
            try
            {
                return Ok(await accountClient.CreateCurrentAccount(currentAcct));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while creating current account: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("savings-account")]
        public async Task<IActionResult> CreateSavingsAccount([FromBody] CreateSavingsAcctRequest savingsAcct)
        {
            try
            {
                return Ok(await accountClient.CreateSavingsAccount(savingsAcct));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while creating savings account: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("corporate-current-account")]
        public async Task<IActionResult> CreateCorpCurrentAccount([FromBody] CreateCorpCurrentAcctRequest corpCurrentAcct)
        {
            try
            {
                return Ok(await accountClient.CreateCorpCurrentAccount(corpCurrentAcct));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while creating corporate current account: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{TerminalId}/atm-office-account")]
        public async Task<IActionResult> GetTerminalOfficeAcct([FromRoute] string TerminalId)
        {
            try
            {
                return Ok(await accountClient.GetTerminalOfficeAcct(new TerminalOfficeAcctRequest { TerminalId=TerminalId}));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while qyerying atm office account: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        private string Clean(string input)
        {
            return Regex.Replace(input, "[^a-zA-Z0-9 ]", "");
        }
    }
}
