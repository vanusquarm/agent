using Cheques.API.Entities.DTOs;
using Cheques.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace Cheques.API.Controllers
{
    [ApiController]
    [Route("api/v2/corporate-current-accounts/")]
    //[Authorize]
    public class ChequesController (IChequesClient chequesClient) : ControllerBase
    {
        [HttpPost("{accountIdentification}/:issueChequeBook")]
        public async Task<IActionResult> IssueChequeBook( [FromBody] ChequeBookIssueRequestDto chequeBookIssueRequestDto)
        {
            try
            {       
               CheckBookIssueResponseDto responseDto = await chequesClient.IssueChequeBookAsync((ChequeBookIssueRequestDto)chequeBookIssueRequestDto);
               return Ok(responseDto);              
               
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while issueing cheque book: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{accountIdentification}/checkStatus")]
        public async Task<IActionResult> GetAccountChequeStatus(string accountIdentification, string chequeNumber)
        {
            try
            {               
                ChequeStatusResponseDto responseDto  = await chequesClient.GetChequeStatusAsync(accountIdentification, chequeNumber);
                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while fetching customer transactions: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("{accountIdentification}/:stopCheque")]
        public async Task<IActionResult> StopChequeBookAsync([AliasAs("accountIdentification")]string accountIdentification, [FromBody] StopChequeRequestDto stopCheque)
        {
            try
            {
                StopChequeResponseDto responseDto = await chequesClient.StopChequeBookAsync(accountIdentification, (StopChequeRequestDto)stopCheque);
                return Ok(responseDto);

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while stopping cheque book: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("{accountIdentification}/:revokeStopCheque")]
        public async Task<IActionResult> RevokeStopChequeBook([AliasAs("accountIdentification")]string accountIdentification ,[FromBody] RevokeChequeRequestDto revokeCheque)
        {
            try
            {
                RevokeChequeResponseDto responseDto = await chequesClient.RevokeStopChequeBookAsync(accountIdentification, (RevokeChequeRequestDto)revokeCheque);
                return Ok(responseDto);

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while revoking cheque book: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }


    }
}
