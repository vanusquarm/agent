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
        public async Task<IActionResult> CreateCurrentAccount([FromBody] CreateCurrentAcctRequest createCurrentAcct)
        {
            return Ok(await accountClient.CreateCurrentAccount(createCurrentAcct));
        }
    }
}
