using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Loans.API.Entities.Models;
using Loans.API.Services;

namespace Loan.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    //[Authorize]
    public class LoansController(ILoanClient loanClient) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetLoans()
        {
            var loans = await loanClient.GetLoans();
            return Ok(loans);
        }
    }
}
