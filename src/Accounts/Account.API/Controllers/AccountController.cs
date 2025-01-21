using Acccount.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Acccount.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    //[Authorize]
    public class AccountController(IAccountClient accountClient) : ControllerBase
    {

        [HttpGet("{accountId}")]
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
    }
}
