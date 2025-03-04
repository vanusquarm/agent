using Finacle.Core;
using Microsoft.AspNetCore.Mvc;
using Transactions.API.Entities;
using Transactions.API.Entities.DTOs;
using Transactions.API.Services;
using static Transactions.API.Entities.ResponseManager;
using EodStatusRequest = Transactions.API.Entities.DTOs.EodStatusRequest;
using TransactionRequest = Transactions.API.Entities.DTOs.TransactionRequest;

namespace Transactions.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TransactionsController : ControllerBase
{

    private readonly ITransactionClient _transactionClient;
    private readonly FinacleClient _finacleClient;
    private readonly AccountUtil _accountUtil;

    public TransactionsController(ITransactionClient transactionClient, AccountUtil accountUtil)
    {
        _transactionClient = transactionClient;
        _accountUtil = accountUtil;
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

    [HttpPost("transfer")]
    public async Task<IActionResult> PerformIntraBankTransfer([FromBody] AccountTransferRequest accountTransferRequest)
    {
        try
        {
            if (accountTransferRequest == null)
            {
                return BadRequest("Invalid request data.");
            }

            var request = IntraBankTransferRequestFactory.Create(accountTransferRequest.SourceAccountId, accountTransferRequest.DestinationAccountId, accountTransferRequest.Amount,
                    accountTransferRequest.Remarks, accountTransferRequest.ChannelReference, accountTransferRequest.ChannelId, accountTransferRequest.DocNum, accountTransferRequest.ChargeEventId);

            var response = await _transactionClient.PerformIntraBankTransfer(accountTransferRequest.ChannelReference, request);

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

    [HttpPost("charges")]
    public async Task<IActionResult> PerformCharge([FromBody] PerformChargeRequestDto chargeRequest)
    {
        try
        {
            var request = new ChargeDetailsDto
            {
                AccountNumber = chargeRequest.AccountNumber,
                EventId = chargeRequest.EventId,
                ChrgAmt = chargeRequest.ChargeAmount.ToString(),
                Requestreference = chargeRequest.RequestReference
            };
            var response = await _transactionClient.PerformCharge(request);

            //if (response == null || string.IsNullOrEmpty(response.TransactionReference))
            //{
            //    return StatusCode(500, "Failed to charge");
            //}

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
