using Microsoft.AspNetCore.Mvc;
using Serilog;
using Transactions.API.Entities.DTOs;
using Transactions.API.Services;
using Finacle.Infrastructure.Utils;
using Finacle.Infrastructure.Logging;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Transactions.API.Entities;
using Transactions.API.Validators;

namespace Transactions.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionClient _transactionClient;
    private readonly TransferValidator _transferValidator;
    private readonly Dictionary<string, string> _serviceMap;

    public TransactionsController(ITransactionClient transactionClient, TransferValidator transferValidator, IOptions<ServiceMapOptions> options)
    {
        _transactionClient = transactionClient;
        _transferValidator = transferValidator;
        _serviceMap = options.Value.ServiceMap ?? new Dictionary<string, string>();
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
        catch (Refit.ApiException ex)
        {
            Log.Error("Back from Getting Transactions with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
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
        catch (Refit.ApiException ex)
        {
            Log.Error("Back from searching Transactions with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
        }
    }

    /// <summary>
    /// Method to Perform Cash Deposit to Teller Account
    /// </summary>
    /// <param name="cashDepositRequest"></param>
    /// <returns></returns>
    [HttpPost("cash-deposit")]
    public async Task<IActionResult> PerformCashDeposit([FromBody] CashDepositRequest cashDepositRequest)
    {
        Log.Information("Recieved Request at PerformCashDeposit: {@Request}", cashDepositRequest);
        try
        {
            if (!_transferValidator.isValid(cashDepositRequest, out List<string> Errors))
            {
                Log.Warning("Perform cash deposit validation failed: {Error}", cashDepositRequest);
                return BadRequest(ResponseManager.ReturnFailedValidations(string.Join("; ", Errors)));
            }

            var solId = cashDepositRequest.SolId ?? await _transferValidator.GetTellerSolIdByIdAsync(cashDepositRequest.TellerId);

            Log.Information("About to PerformCashDeposit with request: {@Request}", cashDepositRequest);
            var request = CashTransferRequestFactory.Create(cashDepositRequest.TellerId, cashDepositRequest.DebitAccount, cashDepositRequest.Amount, cashDepositRequest.Remarks,
                cashDepositRequest.ChannelReference, "C", cashDepositRequest.Currency, cashDepositRequest.DenomDetails);

            var response = await _transactionClient.PerformCashDeposit(request, cashDepositRequest.TellerId, solId);
            Log.Information("Back from Performing CashDeposit with response: {@Response}", response);
            response.TranMessage = "SUCCESSFUL";

            if (response == null || string.IsNullOrEmpty(response.TransactionIdentification))
            {
                response.TranMessage = "FAILED";
                Log.Information("An error occurred while processing the transaction: {@Request}", request);
                return StatusCode(StatusCodes.Status422UnprocessableEntity, "Failed to process the cash withdrawal.");
            }

            return Ok(response);
        }
        catch (Refit.ApiException apiEx)
        {
            Log.Error("Back from Performing CashDeposit with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: apiEx.Content));
            if (apiEx.StatusCode == System.Net.HttpStatusCode.UnprocessableContent)
            {
                return Ok(new CashTransferResponse
                {
                    TranMessage = apiEx.Content,
                    TransactionIdentification = "",
                    TransactionDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    TransactionStatus = "N"
                });
            }
            else
            {
                return StatusCode((int)apiEx.StatusCode, apiEx.Content ?? apiEx.Message);
            }
        }
        catch (Exception ex)
        {
            Log.Error("An exception has occured at PerformCashDeposit with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Method to Perform Cash Withdrawal from Teller Account
    /// </summary>
    /// <param name="cashWithdrawalRequest"></param>
    /// <returns></returns>
    [HttpPost("cash-withdrawal")]
    public async Task<IActionResult> PerformCashWithdrawal([FromBody] CashWithdrawalRequest cashWithdrawalRequest)
    {
        Log.Information("Recieved Request at PerformCashWithdrawal: {@Request}", cashWithdrawalRequest);
        try
        {
            if (!_transferValidator.isValid(cashWithdrawalRequest, out List<string> Errors))
            {
                Log.Warning("Perform cash withdrawal validation failed: {Error}", cashWithdrawalRequest);
                return BadRequest(ResponseManager.ReturnFailedValidations(string.Join("; ", Errors)));
            }

            var solId = cashWithdrawalRequest.SolId ?? await _transferValidator.GetTellerSolIdByIdAsync(cashWithdrawalRequest.TellerId);

            Log.Information("About to PerformCashWithdrawal with request: {@Request}", cashWithdrawalRequest);
            var request = CashTransferRequestFactory.Create(cashWithdrawalRequest.TellerId, cashWithdrawalRequest.CreditAccount, cashWithdrawalRequest.Amount, cashWithdrawalRequest.Remarks,
                cashWithdrawalRequest.ChannelReference, "D", cashWithdrawalRequest.Currency, cashWithdrawalRequest.DenomDetails);

            var response = await _transactionClient.PerformCashWithdrawal(request, cashWithdrawalRequest.TellerId, solId);
            Log.Information("Back from Performing CashWithdrawal with response: {@Response}", response);
            response.TranMessage = "SUCCESSFUL";

            if (response == null || string.IsNullOrEmpty(response.TransactionIdentification))
            {
                response.TranMessage = "FAILED";
                Log.Information("An error occurred while processing the transaction: {@Request}", request);
                return StatusCode(StatusCodes.Status422UnprocessableEntity, "Failed to process the cash withdrawal.");
            }

            return Ok(response);
        }
        catch (Refit.ApiException apiEx)
        {
            Log.Error("Back from Performing PerformCashWithdrawal with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: apiEx.Content));
            if (apiEx.StatusCode == System.Net.HttpStatusCode.UnprocessableContent)
            {
                return Ok(new CashTransferResponse
                {
                    TranMessage = apiEx.Message,
                    TransactionIdentification = "",
                    TransactionDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    TransactionStatus = "N"
                });
            }
            else
            {
                return StatusCode((int)apiEx.StatusCode, apiEx.Content ?? apiEx.Message);
            }
        }
        catch (Exception ex)
        {
            Log.Error("An exception has occured at PerformCashDeposit with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Method to Perform Cash Transfer
    /// </summary>
    /// <param name="cashTransferRequest"></param>
    /// <returns></returns>
    [HttpPost("cash-transfer")]
    public async Task<IActionResult> PerformCashTransfer (PerformCashTransferRequest performCashTransferRequest)
    {
        Log.Information("Recieved Request at PerformCashTransfer");
        try
        {
            if (!_transferValidator.isValid(performCashTransferRequest, out List<string> Errors))
            {
                Log.Warning("Perform cash transfer validation failed: {Error}", performCashTransferRequest);
                return BadRequest(ResponseManager.ReturnFailedValidations(string.Join("; ", Errors)));
            }

            Log.Information("About to PerformCashTransfer with request: {@Request}", performCashTransferRequest);
            var request = CashTransferRequestFactory.Create(
                performCashTransferRequest.TellerId, performCashTransferRequest.CustomerAccount, performCashTransferRequest.Amount, performCashTransferRequest.Remarks,
                performCashTransferRequest.ChannelReference, performCashTransferRequest.TransType, performCashTransferRequest.Currency, performCashTransferRequest.DenomDetails);

            var solId = await _transferValidator.GetTellerSolIdByIdAsync(performCashTransferRequest.TellerId);

            // Crediting or Debiting Teller's Till
            var response = performCashTransferRequest.TransType == "C" ? await _transactionClient.PerformCashWithdrawal(request, performCashTransferRequest.TellerId, solId) :
                await _transactionClient.PerformCashDeposit(request, performCashTransferRequest.TellerId, solId);

            Log.Information("Back from Performing CashTransfer with response: {@Response}", response);
            response.TranMessage = "SUCCESSFUL";

            if (response == null || string.IsNullOrEmpty(response.TransactionIdentification))
            {
                response.TranMessage = "FAILED";
                Log.Information("An error occurred while processing the transaction: {@Request}", request);
                return StatusCode(StatusCodes.Status422UnprocessableEntity, "Failed to process the cash withdrawal.");
            }

            return Ok(response);
        }
        catch (Refit.ApiException apiEx)
        {
            Log.Error("Back from Performing PerformCashTransfer with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: apiEx.Content));
            if (apiEx.StatusCode == System.Net.HttpStatusCode.UnprocessableContent)
            {
                return Ok(new CashTransferResponse
                {
                    TranMessage = apiEx.Content,
                    TransactionIdentification = "",
                    TransactionDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    TransactionStatus = "N"
                });
            }
            else
            {
                return StatusCode((int)apiEx.StatusCode, apiEx.Content ?? apiEx.Message);
            }
        }
        catch (Exception ex)
        {
            Log.Error("An exception has occured at PerformCashTransfer with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
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
                return BadRequest(ResponseManager.ReturnFailedValidations(validationResult.ErrorMessage));
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
            var request = IntraBankTransferRequestFactory.Create(requestId, accountTransferRequest.SourceAccountId, accountTransferRequest.DestinationAccountId, accountTransferRequest.Amount,
                    accountTransferRequest.Remarks, accountTransferRequest.ChannelReference, "", accountTransferRequest.Currency, accountTransferRequest.DocNum, accountTransferRequest.ChargeEventId, accountTransferRequest.BatchId, userPartTransactionCode: accountTransferRequest.UserPartTransactionCode);
            Log.Information("Back from Mapping Finacle intra transfer request with finacle request: {@Request}", request);

            Log.Information("About to PerformIntraBankTransfer - RequestId: {RequestId}, Request: {@Request}, Header: {@Header}", requestId, request, new Header());
            var response = await _transactionClient.PerformIntraBankTransfer(requestId, request);

            Log.Information("Back from Performing IntraBankTransfer with response: {@Response}", response);

            if (response.TransactionReference == null)
            {
                Log.Information("An error occurred while processing the transaction: {@Request}", request);
                return StatusCode(500, $"Failed to process the intra-bank transfer with Response: {response}");
            }

            return Ok(response);
        }
        catch (Refit.ApiException ex)
        {
            Log.Error("Back from Performing IntraBankTransfer with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
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
                return BadRequest(ResponseManager.ReturnFailedValidations("Invalid request data."));
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
        catch (Refit.ApiException ex)
        {
            Log.Error("Back from getting EodStatus with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
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
            return BadRequest(ResponseManager.ReturnFailedValidations(string.Join("; ", Errors)));
        }
        try
        {
            var currency = chargeRequest.Currency ?? Utils.ParseAccountNumber(chargeRequest.AccountId).Currency;
            var request = new ChargeDetailsDto
            {
                AccountNumber = chargeRequest.AccountId,
                EventId = chargeRequest.ChargeType,
                ChrgCrncy = currency,
                ChrgAmt = chargeRequest.ChargeAmount.ToString(),
                Requestreference = chargeRequest.TransactionReference
            };

            Log.Information("About to PerformCharge with request: {@Request}", request);
            var response = await _transactionClient.PerformCharge(request);
            Log.Information("Back from Performing Charge with response: {@Response}", response);

            return Ok(response);
        }
        catch (Refit.ApiException ex)
        {
            Log.Error("Back from Performing Charge with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
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
            return BadRequest(ResponseManager.ReturnFailedValidations(string.Join("; ", Errors)));
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
        catch (Refit.ApiException ex)
        {
            Log.Error("Back from PerformChargeInquiry with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
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
        catch (Refit.ApiException ex)
        {
            Log.Error("Back from PerformReversal with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
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
        catch (Refit.ApiException ex)
        {
            Log.Information("Back from GetTransactionBankDate with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
        }

    }

    /// <summary>
    /// Method to Perform Charge
    /// </summary>
    /// <param name="StatementChargeRequest"></param>
    /// <returns></returns>
    [HttpGet("statement-charges")]
    public async Task<IActionResult> PerformStatementCharge([FromQuery] StatementChargeRquest chargeRequest)
    {
        Log.Information("Recieved Request at PerformCharge: {@Request}", chargeRequest);
        if (!_transferValidator.isValid(chargeRequest, out List<string> Errors))
        {
            Log.Warning("Perform charge validation failed: {Error}", chargeRequest);
            return BadRequest(ResponseManager.ReturnFailedValidations(string.Join("; ", Errors)));
        }
        try
        {
            Log.Information("About to PerformCharge with request: {@Request}", chargeRequest);
            var response = await _transactionClient.PerformStatementCharge(chargeRequest);
            Log.Information("Back from Performing Charge with response: {@Response}", response);

            return Ok(response);
        }
        catch (Refit.ApiException ex)
        {
            Log.Error("Back from Performing Charge with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
        }
        catch (Exception ex)
        {
            Log.Error("An exception has occured at PerformCharge with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }
}
