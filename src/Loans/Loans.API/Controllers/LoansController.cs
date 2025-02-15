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

        [HttpGet("{loanId}")]
        public async Task<IActionResult> GetLoanDetails(string loanId, bool isSecured = false)
        {
            var loans = isSecured ? await loanClient.GetSecuredLoanDetails(loanId) : await loanClient.GetLoanDetails(loanId);
            return Ok(loans);
        }

        [HttpPost("{accountId}")]
        public async Task<IActionResult> DiburseLoan(string accountId, bool isSecured = false)
        {
            var loans = isSecured ? await loanClient.DiburseSecuredLoan(accountId) : await loanClient.DiburseLoan(accountId);
            return Ok(loans);
        }

        [HttpPatch("{loanId}")]
        public async Task<IActionResult> PayOffLoan(string loanId, bool isSecured = false)
        {
            var loans = isSecured ? await loanClient.PayOffSecuredLoan(loanId) : await loanClient.PayOffLoan(loanId);
            return Ok(loans);
        }

        [HttpPut("{loanId}")]
        public async Task<IActionResult> UpdateLoan(string loanId, bool isSecured = false)
        {
            var loans = isSecured ? await loanClient.UpdateSecuredLoan(loanId) : await loanClient.UpdateLoan(loanId);
            return Ok(loans);
        }
    }
}
