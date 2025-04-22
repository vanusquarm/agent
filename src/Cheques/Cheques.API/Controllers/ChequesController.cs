using Cheques.API.Entities.DTOs;
using Cheques.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Refit;
using Serilog;

namespace Cheques.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    //[Authorize]
    public class ChequesController (IChequeClient chequesClient) : ControllerBase
    {
        /// <summary>
        /// Method to Get Cheque Transactions
        /// </summary>
        /// <param name="chequeTransactionsRequest"></param>
        /// <returns></returns>
        [HttpGet("{chequeNumber}/transactions")]
        public async Task<IActionResult> GetChequeTransactions(string chequeNumber, string amount, string date)
        {
            Log.Information("Received Request at GetChequeTransactions: {@Request}", new { chequeNumber, amount, date });
            try
            {
                var chequeTransactionsRequest = new ChequeTransactionsRequest
                {
                    ChqSrlNum = chequeNumber,
                    TranAmount = amount,
                    TranDate = date ?? DateTime.UtcNow.ToString("yyyy-MM-dd")
                };

                Log.Information("About to GetChequeTransactions with request: {@Request}", chequeTransactionsRequest);

                ChequeTransactionsResponse responseDto = await chequesClient.GetChequeTransactionsAsync(chequeTransactionsRequest);

                Log.Information("Back from GetChequeTransactions with response: {@Response}", responseDto);

                if (responseDto == null)
                {
                    return NotFound("No transaction available");
                }

                return Ok(responseDto);
            }
            catch (ApiException apiEx) when (apiEx.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                Log.Error("Remote server returned 500 Internal Server Error. Response: {ResponseContent}", apiEx.Content);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "The remote server encountered an error while processing your request.",
                    Details = apiEx.Content
                });
            }
            catch (Exception ex)
            {
                Log.Error("An exception occurred at GetChequeTransactions with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }


        /// <summary>
        /// Method to Issue Cheque Book
        /// </summary>
        /// <param name="chequeBookIssueRequestDto"></param>
        /// <returns></returns>
        [HttpPost("issueChequeBook")]
        public async Task<IActionResult> IssueChequeBook( [FromBody] ChequeBookIssueRequestDto chequeBookIssueRequestDto)
        {
            Log.Information("Recieved Request at IssueChequeBook: {@Request}", chequeBookIssueRequestDto);
            try
            {
                Log.Information("About to IssueChequeBook with request: {Request}", chequeBookIssueRequestDto);
                CheckBookIssueResponseDto responseDto = await chequesClient.IssueChequeBookAsync((ChequeBookIssueRequestDto)chequeBookIssueRequestDto);
                Log.Information("Back from Issueing ChequeBook with response: {@Response}", responseDto);
                return Ok(responseDto); 
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at IssueChequeBook with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.Error.WriteLine($"Error occurred while issueing cheque book: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Method to Get Account Cheque Status
        /// </summary>
        /// <param name="accountIdentification"></param>
        /// <param name="chequeNumber"></param>
        /// <returns></returns>
        [HttpGet("{chequeNumber}/checkStatus")]
        public async Task<IActionResult> GetAccountChequeStatus(string accountIdentification, [FromRoute] string chequeNumber)
        {
            Log.Information("Recieved Request at GetAccountChequeStatus: {@Request}", (accountIdentification, chequeNumber));
            try
            {
                Log.Information("About to GetAccountChequeStatus with request: {@Request}", (accountIdentification, chequeNumber));
                ChequeStatusResponseDto responseDto  = await chequesClient.GetChequeStatusAsync(accountIdentification, chequeNumber);
                Log.Information("Back from Getting AccountChequeStatus with response: {@Response}", responseDto);
                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at GetAccountChequeStatus with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.Error.WriteLine($"Error occurred while fetching customer transactions: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Method to Validate Cheque
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("status")]
        public async Task<IActionResult> ValidateCheque(ChequeConfirmationRequest request)
        {
            Log.Information("Recieved Request at ValidateCheque: {@Request}", request);
            try
            {
                Log.Information("About to ValidateCheque with request: {@Request}", request);
                var responseDto = await chequesClient.ConfirmChequeAsync(request);
                Log.Information("Back from Validating Cheque with response: {@Response}", responseDto);
                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at ValidateCheque with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.Error.WriteLine($"Error occurred while fetching customer transactions: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Async Method to Stop Cheque Book
        /// </summary>
        /// <param name="accountIdentification"></param>
        /// <param name="stopCheque"></param>
        /// <returns></returns>
        [HttpPost("stopCheque")]
        public async Task<IActionResult> StopChequeBookAsync([AliasAs("accountIdentification")]string accountIdentification, [FromBody] StopChequeRequestDto stopCheque)
        {
            Log.Information("Recieved Request at StopChequeBookAsync: {@Request}", (accountIdentification, stopCheque));
            try
            {
                Log.Information("About to StopChequeBook with request: {@Request}", (accountIdentification, stopCheque));
                StopChequeResponseDto responseDto = await chequesClient.StopChequeBookAsync(accountIdentification, (StopChequeRequestDto)stopCheque);
                Log.Information("Back from Stopping ChequeBook with response: {@Response}", responseDto); 
                return Ok(responseDto);

            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at StopChequeBookAsync with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.Error.WriteLine($"Error occurred while stopping cheque book: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Method to Revoke Stop Cheque Book
        /// </summary>
        /// <param name="accountIdentification"></param>
        /// <param name="revokeCheque"></param>
        /// <returns></returns>
        [HttpPost("revokeStopCheque")]
        public async Task<IActionResult> RevokeStopChequeBook([AliasAs("accountIdentification")]string accountIdentification ,[FromBody] RevokeChequeRequestDto revokeCheque)
        {
            Log.Information("Recieved Request at RevokeStopChequeBook: {@Request}", (accountIdentification, revokeCheque));
            try
            {
                Log.Information("About to RevokeStopChequeBook with request: {@Request}", (accountIdentification, revokeCheque));
                RevokeChequeResponseDto responseDto = await chequesClient.RevokeStopChequeBookAsync(accountIdentification, (RevokeChequeRequestDto)revokeCheque);
                Log.Information("Back from Revoking StopChequeBook with response: {@Response}", responseDto);
                return Ok(responseDto);

            }
            catch (Exception ex)
            {
                Log.Error("An exception has occured at RevokeStopChequeBook with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                Console.Error.WriteLine($"Error occurred while revoking cheque book: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
