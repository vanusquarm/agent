using Microsoft.AspNetCore.Mvc;
using Transactions.API.Entities.DTOs;
using Transactions.API.Services;

namespace Transactions.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TransactionsController : ControllerBase
{
  
    private readonly ITransactionClient _transactionClient;

        public TransactionsController(ITransactionClient transactionClient)
        {
            _transactionClient = transactionClient;
        }
        
        
        [HttpPost("cash-deposit")]
        public async Task<IActionResult> PerformCashDeposit([FromBody] CashDepositRequest cashDepositRequest)
        {
            try
            {
                if (cashDepositRequest == null)
                {
                    return BadRequest("Invalid request data.");
                }

                var response = await _transactionClient.PerformCashDeposit(cashDepositRequest);

                if (response == null || response.Status != "Success")
                {
                    return StatusCode(500, response?.Message ?? "An error occurred while processing the transaction.");
                }

                return Ok(response);
            }
            catch (Refit.ApiException apiEx)
            {
                return StatusCode((int)apiEx.StatusCode, apiEx.Content ?? apiEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
        
        [HttpPost("cash-withdrawal")]
        public async Task<IActionResult> PerformCashWithdrawal([FromBody] CashWithdrawalRequest cashWithdrawalRequest)
        {
            try
            {
                if (cashWithdrawalRequest == null)
                {
                    return BadRequest("Invalid request data.");
                }

                var response = await _transactionClient.PerformCashWithdrawal(cashWithdrawalRequest);

                if (response == null || string.IsNullOrEmpty(response.TransactionIdentification))
                {
                    return StatusCode(500, "Failed to process the cash withdrawal.");
                }

                return Ok(response);
            }
            catch (Refit.ApiException apiEx)
            {
                return StatusCode((int)apiEx.StatusCode, apiEx.Content ?? apiEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
        
        [HttpPost("{customerReference}/tranDate/{transactionDate}")]
        public async Task<IActionResult> GetTransactionDetails([FromRoute] string customerReference, [FromRoute] DateTime transactionDate, [FromQuery] string accountIdentification)
        {
            try
            {
                if (string.IsNullOrEmpty(customerReference) || string.IsNullOrEmpty(accountIdentification))
                {
                    return BadRequest("Invalid request data.");
                }

                var response = await _transactionClient.GetTransactionDetails(customerReference, transactionDate, accountIdentification);

                if (response == null || response.TransactionDetails == null || !response.TransactionDetails.Any())
                {
                    return NotFound("No transaction details found.");
                }

                return Ok(response);
            }
            catch (Refit.ApiException apiEx)
            {
                return StatusCode((int)apiEx.StatusCode, apiEx.Content ?? apiEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
        
        [HttpPost("intra-bank-transfer")]
        public async Task<IActionResult> PerformIntraBankTransfer([FromBody] IntraBankTransferRequest intraBankTransferRequest)
        {
            try
            {
                if (intraBankTransferRequest == null)
                {
                    return BadRequest("Invalid request data.");
                }

                var response = await _transactionClient.PerformIntraBankTransfer(intraBankTransferRequest);

                if (response == null || string.IsNullOrEmpty(response.TransactionReference))
                {
                    return StatusCode(500, "Failed to process the intra-bank transfer.");
                }

                return Ok(response);
            }
            catch (Refit.ApiException apiEx)
            {
                return StatusCode((int)apiEx.StatusCode, apiEx.Content ?? apiEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
        
        [HttpPost("query-transactions")]
        public async Task<IActionResult> GetTransactions([FromQuery] string accountIdentification, [FromQuery] string transactionFromDate, [FromQuery] string transactionToDate, [FromQuery] decimal amountRangeFrom, [FromQuery] decimal amountRangeTo)
        {
            try
            {
                if (string.IsNullOrEmpty(accountIdentification) || string.IsNullOrEmpty(transactionFromDate) || string.IsNullOrEmpty(transactionToDate))
                {
                    return BadRequest("Invalid request data.");
                }

                var response = await _transactionClient.GetTransactions(accountIdentification, transactionFromDate, transactionToDate, amountRangeFrom, amountRangeTo);

                if (response == null || response.Transactions == null || !response.Transactions.Any())
                {
                    return NotFound("No transactions found.");
                }

                return Ok(response);
            }
            catch (Refit.ApiException apiEx)
            {
                return StatusCode((int)apiEx.StatusCode, apiEx.Content ?? apiEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

}
