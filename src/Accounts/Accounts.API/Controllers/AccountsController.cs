using Acccounts.API.Entities;
using Acccounts.API.Entities.DTOs;
using Acccounts.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            var accountDetails = await accountClient.GetAccountDetails(new { accountId })
                ?? await accountClient.GetSavingsAccount(accountId)
                ?? await accountClient.GetCurrentAccount(accountId)
                ?? await accountClient.GetCorpCurrentAccount(accountId);

            if (accountDetails == null) 
            { 
                return NotFound();
            }

            return Ok(accountDetails);
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
            return Ok(await accountClient.CreateCurrentAccount(currentAcct));
        }

        [HttpPost("savings-account")]
        public async Task<IActionResult> CreateSavingsAccount([FromBody] CreateSavingsAcctRequest savingsAcct)
        {
            return Ok(await accountClient.CreateSavingsAccount(savingsAcct));
        }

        [HttpPost("corporate-current-account")]
        public async Task<IActionResult> CreateCorpCurrentAccount([FromBody] CreateCorpCurrentAcctRequest corpCurrentAcct)
        {
            return Ok(await accountClient.CreateCorpCurrentAccount(corpCurrentAcct));
        }
    }
}
