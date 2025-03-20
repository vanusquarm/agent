using Finacle.Core;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Transactions.API.Entities.DTOs;
using Transactions.API.Services;
using EodStatusRequest = Transactions.API.Entities.DTOs.EodStatusRequest;
using TransactionRequest = Transactions.API.Entities.DTOs.TransactionRequest;
using Microsoft.AspNetCore.Authorization;

namespace Transactions.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionClient _transactionClient;
    private readonly FinacleClient _finacleClient;

    public TransactionsController(ITransactionClient transactionClient, FinacleClient finacleClient)
    {
        _transactionClient = transactionClient;
        _finacleClient = finacleClient;
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
                return StatusCode(500, "Failed to process the cash withdrawal.");
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
    public async Task<IActionResult> PerformIntraBankTransfer([FromBody] Entities.DTOs.AccountTransferRequest accountTransferRequest)
    {
        try
        {
            if (accountTransferRequest == null)
            {
                Log.Information("Invalid request data.: {@Request}", accountTransferRequest);
                return BadRequest("Invalid request data.");
            }

            Log.Information("About to Map Finacle intra transfer request with request: {@Request}", accountTransferRequest);
            var request = IntraBankTransferRequestFactory.Create(accountTransferRequest.SourceAccountId, accountTransferRequest.DestinationAccountId, accountTransferRequest.Amount,
                    accountTransferRequest.Remarks, accountTransferRequest.ChannelReference, accountTransferRequest.ChannelId, accountTransferRequest.DocNum, accountTransferRequest.ChargeEventId);
            Log.Information("Back from Mapping Finacle intra transfer request with finacle request: {@Request}", request);

            Log.Information("About to PerformIntraBankTransfer with request: {@Request}", request);
            var response = await _transactionClient.PerformIntraBankTransfer(accountTransferRequest.ChannelReference, request);

            Log.Information("Back from Performing IntraBankTransfer with response: {@Response}", response);

            if (response == null)
            {
                Log.Information("An error occurred while processing the transaction: {@Request}", request);
                return StatusCode(500, "Failed to process the intra-bank transfer.");
            }

            return Ok(response);
        }
        catch (Refit.ApiException apiEx)
        {
            Log.Error("An exception has occured at PerformIntraBankTransfer with message: {Message} and stack trace: {StackTrace}", apiEx.Message, apiEx.StackTrace);
            return StatusCode((int)apiEx.StatusCode, apiEx.Content ?? apiEx.Message);
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

    [HttpPost("chargeinquiry")]
    public async Task<IActionResult> PerformChargeInquiry([FromBody] ChargeInquiryRequest request)
    {
        try
        {
            var response = await _transactionClient.ChargeInquiry(request);

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

    [HttpPost("reverse")]
    public async Task<IActionResult> PerformReverse([FromBody] ReversalTransferRequestDto reversalRequest)
    {
        try
        {
            var request = new
            {
                transactionIdentification=new 
                {
                    transactionDate = reversalRequest.TransactionDate,
                    transactionReference = reversalRequest.TransactionReference,
                    remarks = reversalRequest.Remarks
                }
               
            };

            var response = await _transactionClient.Reverse(reversalRequest.TransactionReference, request);

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

    [HttpGet("bankdate")]
    public async Task<IActionResult> GetTransactionBankDate(string branchCode)
    {
        try
        {
            var request = new Dictionary<string, string>
                    {
                        { "solId", $"{ branchCode }" }
                    };
            Entities.DTOs.BankDateDto date = await _transactionClient.GetTransactionBankDate(request);
            //var contentx = result.Content.ReadAsStringAsync().Result;


            return Ok(date);


        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return NotFound();
        }

    }

}
