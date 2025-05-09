using Microsoft.AspNetCore.Mvc;
using Serilog;
using Transactions.API.Entities.DTOs;
using Transactions.API.Services;
using Microsoft.AspNetCore.Authorization;
using Finacle.Infrastructure.Utils;
using System.Text.Json;

namespace Transactions.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]

public class TransactionsController : ControllerBase
{
    private readonly ITransactionClient _transactionClient;
    private readonly TransferValidator _transferValidator;

    public TransactionsController(ITransactionClient transactionClient, TransferValidator transferValidator)
    {
        _transactionClient = transactionClient;
        _transferValidator = transferValidator;
    }

    /// <summary>
    /// Method to Get Transactions
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetTransactions([FromQuery] TransactionRequest request)
    {
        Log.Information("Recieved Request at GetTransactions: {@Request}", request);
        try
        {
            Log.Information("About to GetTransactions with request: {@Request}", request);
            var transactions = await _transactionClient.GetTransactionDetails(request);
            Log.Information("Back from Getting Transactions with response: {@Response}", transactions);

            return Ok(transactions);
        }
        catch (Exception ex)
        {
            Log.Error("An exception has occured at GetTransactions with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Method to Search Transactions
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("search")]
    public async Task<IActionResult> SearchTransactions([FromBody] TransactionRequest request)
    {
        Log.Information("Recieved Request at SearchTransactions: {@Request}", request);
        try
        {
            Log.Information("About to SearchTransactions with request: {@Request}", request);
            var transactions = await _transactionClient.GetTransactionDetails(request);
            Log.Information("Back from searching Transactions with response: {@Response}", transactions);

            return Ok(transactions);
        }
        catch (Exception ex)
        {
            Log.Error("An exception has occured at SearchTransactions with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Method to Perform Cash Deposit
    /// </summary>
    /// <param name="cashDepositRequest"></param>
    /// <returns></returns>
    [HttpPost("cash-deposit")]
    public async Task<IActionResult> PerformCashDeposit([FromBody] CashDepositRequest cashDepositRequest)
    {
        Log.Information("Recieved Request at PerformCashDeposit: {@Request}", cashDepositRequest);
        try
        {
            if (cashDepositRequest == null)
            {
                Log.Information("Invalid request data.: {@Request}", cashDepositRequest);
                return BadRequest("Invalid request data.");
            }

            Log.Information("About to PerformCashDeposit with request: {@Request}", cashDepositRequest);
            var response = await _transactionClient.PerformCashDeposit(cashDepositRequest);
            Log.Information("Back from Performing CashDeposit with response: {@Response}", response);

            if (response == null || response.Status != "Success")
            {
                Log.Information("An error occurred while processing the transaction: {@Request}", cashDepositRequest);
                return StatusCode(500, response?.Message ?? "An error occurred while processing the transaction.");
            }

            return Ok(response);
        }
        catch (Refit.ApiException apiEx)
        {
            Log.Error("An exception has occured at PerformCashDeposit with message: {Message} and stack trace: {StackTrace}", apiEx.Message, apiEx.StackTrace);
            return StatusCode((int)apiEx.StatusCode, apiEx.Content ?? apiEx.Message);
        }
        catch (Exception ex)
        {
            Log.Error("An exception has occured at PerformCashDeposit with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Method to Perform Cash Withdrawal
    /// </summary>
    /// <param name="cashWithdrawalRequest"></param>
    /// <returns></returns>
    [HttpPost("cash-withdrawal")]
    public async Task<IActionResult> PerformCashWithdrawal([FromBody] CashWithdrawalRequest cashWithdrawalRequest)
    {
        Log.Information("Recieved Request at PerformCashWithdrawal: {@Request}", cashWithdrawalRequest);
        try
        {
            if (cashWithdrawalRequest == null)
            {
                Log.Information("Invalid request data.: {@Request}", cashWithdrawalRequest);
                return BadRequest("Invalid request data.");
            }

            Log.Information("About to PerformCashWithdrawal with request: {@Request}", cashWithdrawalRequest);
            var response = await _transactionClient.PerformCashWithdrawal(cashWithdrawalRequest);
            Log.Information("Back from Performing CashWithdrawal with response: {@Response}", response);

            if (response == null || string.IsNullOrEmpty(response.TransactionIdentification))
            {
                Log.Information("An error occurred while processing the transaction: {@Request}", cashWithdrawalRequest);
                return StatusCode(StatusCodes.Status422UnprocessableEntity, "Failed to process the cash withdrawal.");
            }

            return Ok(response);
        }
        catch (Refit.ApiException apiEx)
        {
            Log.Error("An exception has occured at PerformCashDeposit with message: {Message} and stack trace: {StackTrace}", apiEx.Message, apiEx.StackTrace);
            return StatusCode((int)apiEx.StatusCode, apiEx.Content ?? apiEx.Message);
        }
        catch (Exception ex)
        {
            Log.Error("An exception has occured at PerformCashDeposit with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Method to Perform Intra Bank Transfer
    /// </summary>
    /// <param name="accountTransferRequest"></param>
    /// <returns></returns>
    [HttpPost("transfer")]
    public async Task<IActionResult> PerformIntraBankTransfer([FromBody] AccountTransferRequest accountTransferRequest)
    {
        try
        {
            // Validate the request
            accountTransferRequest.Trim();
            var validationResult = await _transferValidator.ValidateAsync(accountTransferRequest);

            if (!validationResult.IsValid)
            {
                Log.Warning("Transfer validation failed: {Error}", validationResult.ErrorMessage);
                return BadRequest(new { validationResult.ErrorMessage });
            }

            if (validationResult.RequiresAdditionalVerification)
            {
                return Accepted(new
                {
                    Message = "Additional verification required",
                    Methods = validationResult.AvailableVerificationMethods
                });
            }

            var requestId = JsonSerializer.Serialize(accountTransferRequest).ToHash();

            Log.Information("About to Map Finacle intra transfer request with request: {@Request}", accountTransferRequest);
            var request = IntraBankTransferRequestFactory.Create(accountTransferRequest.SourceAccountId, accountTransferRequest.DestinationAccountId, accountTransferRequest.Amount,
                    accountTransferRequest.Remarks, accountTransferRequest.ChannelReference, accountTransferRequest.ChannelId, accountTransferRequest.Currency, accountTransferRequest.DocNum, accountTransferRequest.ChargeEventId, accountTransferRequest.BatchId);
            Log.Information("Back from Mapping Finacle intra transfer request with finacle request: {@Request}", request);

            Log.Information("About to PerformIntraBankTransfer - RequestId: {RequestId}, Request: {@Request}", requestId, request);
            var response = await _transactionClient.PerformIntraBankTransfer(requestId, request);

            Log.Information("Back from Performing IntraBankTransfer with response: {@Response}", response);

            if (response.TransactionReference == null)
            {
                Log.Information("An error occurred while processing the transaction: {@Request}", request);
                return StatusCode(500, $"Failed to process the intra-bank transfer with Response: {response}");
            }

            return Ok(response);
        }
        catch (Refit.ApiException apiEx)
        {
            Log.Error("Finacle remote server encountered an error while processing your request. Response: {ResponseContent}", apiEx.Content);
            return StatusCode((int)apiEx.StatusCode, new
            {
                Message = apiEx.Message,
                Details = apiEx.Content
            });
        }
        catch (Exception ex)
        {
            Log.Error("An exception has occured at PerformIntraBankTransfer with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Method to Get Eod Status
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("eod-status")]
    public async Task<IActionResult> GetEodStatus([FromBody] EodStatusRequest request)
    {
        Log.Information("Recieved Request at GetEodStatus: {@Request}", request);
        try
        {
            if (request == null)
            {
                Log.Information("Invalid request data.: {@Request}", request);
                return BadRequest("Invalid request data.");
            }

            Log.Information("About to GetEodStatus with request: {@Request}", request);
            var response = await _transactionClient.GetEodStatusDetails(request);
            Log.Information("Back from getting EodStatus with response: {@Response}", response);

            if (response == null || string.IsNullOrEmpty(response.BankStatus))
            {
                Log.Information("An error occurred while processing the transaction: {@Request}", request);
                return StatusCode(500, "Failed to retrieve EOD status.");
            }

            return Ok(response);
        }
        catch (Refit.ApiException apiEx)
        {
            Log.Error("An exception has occured at GetEodStatus with message: {Message} and stack trace: {StackTrace}", apiEx.Message, apiEx.StackTrace);
            return StatusCode((int)apiEx.StatusCode, apiEx.Content ?? apiEx.Message);
        }
        catch (Exception ex)
        {
            Log.Error("An exception has occured at GetEodStatus with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Method to Perform Charge
    /// </summary>
    /// <param name="chargeRequest"></param>
    /// <returns></returns>
    [HttpPost("charges")]
    public async Task<IActionResult> PerformCharge([FromBody] PerformChargeRequestDto chargeRequest)
    {
        Log.Information("Recieved Request at PerformCharge: {@Request}", chargeRequest);
        if (!_transferValidator.isValid(chargeRequest, out List<string> Errors))
        {
            Log.Warning("Perform charge validation failed: {Error}", chargeRequest);
            return BadRequest(Errors);
        }
        try
        {
            var request = new ChargeDetailsDto
            {
                AccountNumber = chargeRequest.AccountId,
                EventId = chargeRequest.ChargeType,
                ChrgCrncy = chargeRequest.Currency ?? "GHS",
                ChrgAmt = chargeRequest.ChargeAmount.ToString(),
                Requestreference = chargeRequest.TransactionReference
            };

            Log.Information("About to PerformCharge with request: {@Request}", request);
            var response = await _transactionClient.PerformCharge(request);
            Log.Information("Back from Performing Charge with response: {@Response}", response);

            return Ok(response);
        }
        catch (Refit.ApiException apiEx)
        {
            Log.Error("An exception has occured at PerformCharge with message: {Message} and stack trace: {StackTrace}", apiEx.Message, apiEx.StackTrace);
            return StatusCode((int)apiEx.StatusCode, apiEx.Content ?? apiEx.Message);
        }
        catch (Exception ex)
        {
            Log.Error("An exception has occured at PerformCharge with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }

    [HttpGet("charges")]
    public async Task<IActionResult> PerformChargeInquiry([FromQuery] PerformChargeInquiryDto chargeInquiryRequest)
    {
        Log.Information("Recieved Request at PerformChargeInquiry: {@Request}", chargeInquiryRequest);

        if (!_transferValidator.isValid(chargeInquiryRequest, out List<string> Errors))
        {
            Log.Warning("Perform charge validation failed: {Error}", chargeInquiryRequest);
            return BadRequest(Errors);
        }
        try
        {
            var request = new ChargeInquiryRequest
            {
                accountId = chargeInquiryRequest.AccountId,
                tranAmt = chargeInquiryRequest.TranAmt.ToString(),
                eventId = chargeInquiryRequest.EventId,
                eventType = chargeInquiryRequest.EventType
            };

            Log.Information("About to PerformChargeInquiry with request: {@Request}", request);
            var response = await _transactionClient.ChargeInquiry(request);
            Log.Information("Back from PerformChargeInquiry with response: {@Response}", response);

            return Ok(response);
        }
        catch (Refit.ApiException apiEx)
        {
            Log.Error("Remote server returned 500 Internal Server Error. Response: {ResponseContent}", apiEx?.Content);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Message = "The remote server encountered an error while processing your request.",
                Details = apiEx.Content
            });
        }
        catch (Exception ex)
        {
            Log.Error("An exception has occured at PerformChargeInquiry with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }

    [HttpPost("reverse")]
    public async Task<IActionResult> PerformReversal([FromBody] TransactionIdentification reversalRequest)
    {
        Log.Information("Recieved Request at PerformReversal: {@Request}", reversalRequest);
        try
        {
            var request = new ReversalTransferRequest
            {
                TransactionIdentification = new TransactionIdentification
                {
                    TransactionDate = reversalRequest?.TransactionDate,
                    TransactionReference = reversalRequest.TransactionReference,
                    Remarks = reversalRequest?.Remarks
                }
            };


            Log.Information("About to PerformReversal with request: {@Request}", request);
            var response = await _transactionClient.Reverse(reversalRequest.TransactionReference, request);
            Log.Information("Back from PerformReversal with response: {@Response}", response);

            return Ok(response);
        }
        catch (Refit.ApiException apiEx)
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
            Log.Error("An exception has occured at PerformReversal with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }

    [HttpGet("bankdate")]
    public async Task<IActionResult> GetTransactionBankDate(string branchCode)
    {
        try
        {
            var request = new Dictionary<string, string>
                    {
                        { "solId", $"{ branchCode }" }
                    };

            Log.Information("About to GetTransactionBankDate with request: {@Request}", request);
            BankDateDto date = await _transactionClient.GetTransactionBankDate(request);
            Log.Information("Back from GetTransactionBankDate with response: {@Response}", date);

            return Ok(date);
        }
        catch (Exception ex)
        {
            Log.Error("An exception has occured at GetTransactionBankDate with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
            return NotFound();
        }

    }

}
