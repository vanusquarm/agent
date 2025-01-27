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
        public async Task<IActionResult> GetAccountBalance([FromRoute] string accountId, [FromQuery] string accountType = null)
        {
            var balances = await accountClient.GetAccountBalance(accountId);
            var balance = balances?.Balances?.FirstOrDefault(balance => balance.BalanceType == accountType);
            if (balance != null) 
            {
                return Ok(balance);
            }

            return Ok(balances);
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
