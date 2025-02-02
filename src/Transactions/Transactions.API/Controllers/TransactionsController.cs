using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
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

    [HttpGet]
    public async Task<IActionResult> GetTransactions([FromQuery] TransactionRequest request)
    {
        try
        {
            var transactions = await _transactionClient.GetTransactionDetails(request);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }

    [HttpPost("search")]
    public async Task<IActionResult> SearchTransactions([FromBody] TransactionRequest request)
    {
        try
        {
            var transactions = await _transactionClient.GetTransactionDetails(request);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
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

    [HttpPost("eod-status")]
    public async Task<IActionResult> GetEodStatus([FromBody] EodStatusRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest("Invalid request data.");
            }
            var response = await _transactionClient.GetEodStatusDetails(request);

            if (response == null || string.IsNullOrEmpty(response.BankStatus))
            {
                return StatusCode(500, "Failed to retrieve EOD status.");
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
