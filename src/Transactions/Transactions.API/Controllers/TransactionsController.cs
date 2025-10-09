using Finacle.Infrastructure.Logging;
using Finacle.Infrastructure.Persistence;
using Finacle.Infrastructure.Utils;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text.Json;
using Transactions.API.Entities;
using Transactions.API.Entities.DTOs;
using Transactions.API.Services;
using Transactions.API.Validators;

namespace Transactions.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionClient _transactionClient;
    private readonly TransferValidator _transferValidator;
    private readonly FinacleDbContext _finacleDbContext;

    public TransactionsController(ITransactionClient transactionClient, TransferValidator transferValidator, FinacleDbContext finacleDbContext)
    {
        _transactionClient = transactionClient;
        _transferValidator = transferValidator;
        _finacleDbContext = finacleDbContext;
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

            var request = CashTransferRequestFactory.Create(cashDepositRequest.TellerId, cashDepositRequest.DebitAccount, cashDepositRequest.Amount, cashDepositRequest.Remarks,
                cashDepositRequest.ChannelReference, "C", cashDepositRequest.Currency, cashDepositRequest.DenomDetails);

            Log.Information("About to PerformCashDeposit with request: {@Request}, headers: {@Headers}", request, new { cashDepositRequest.TellerId, solId });
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

            var request = CashTransferRequestFactory.Create(cashWithdrawalRequest.TellerId, cashWithdrawalRequest.CreditAccount, cashWithdrawalRequest.Amount, cashWithdrawalRequest.Remarks,
                cashWithdrawalRequest.ChannelReference, "D", cashWithdrawalRequest.Currency, cashWithdrawalRequest.DenomDetails);

            Log.Information("About to PerformCashWithdrawal with request: {@Request}, headers: {@Headers}", request, new { cashWithdrawalRequest.TellerId, solId });
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
        Log.Information("Recieved Request at PerformCashTransfer: {@Request}", performCashTransferRequest);
        try
        {
            if (!_transferValidator.isValid(performCashTransferRequest, out List<string> Errors))
            {
                Log.Warning("Perform cash transfer validation failed: {Error}", performCashTransferRequest);
                return BadRequest(ResponseManager.ReturnFailedValidations(string.Join("; ", Errors)));
            }

            var request = CashTransferRequestFactory.Create(
                performCashTransferRequest.TellerId, performCashTransferRequest.CustomerAccount, performCashTransferRequest.Amount, performCashTransferRequest.Remarks,
                performCashTransferRequest.ChannelReference, performCashTransferRequest.TransType, performCashTransferRequest.Currency, performCashTransferRequest.DenomDetails);

            var solId = await _transferValidator.GetTellerSolIdByIdAsync(performCashTransferRequest.TellerId);

            Log.Information("About to PerformCashTransfer with request: {@Request}, headers: {@Headers}", request, new { performCashTransferRequest.TellerId, solId });
            var response = performCashTransferRequest.TransType == TransactionType.WITHDRAWAL ? await _transactionClient.PerformCashWithdrawal(request, performCashTransferRequest.TellerId, solId) :
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

            HttpContext.Items.TryGetValue("requestId", out var requestIdObj);
            string requestId = requestIdObj?.ToString() ?? string.Empty;

            Log.Information("About to Map Finacle intra transfer request with request: {@Request}", accountTransferRequest);
            var request = IntraBankTransferRequestFactory.Create(requestId, accountTransferRequest.SourceAccountId, accountTransferRequest.DestinationAccountId, accountTransferRequest.Amount,
                    accountTransferRequest.Remarks, accountTransferRequest.ChannelReference, "", accountTransferRequest.Currency, accountTransferRequest.DocNum, accountTransferRequest.ChargeEventId, accountTransferRequest.BatchId, explanationCode: accountTransferRequest.UserPartTransactionCode);
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
    /// Method to Perform Cheque Transfer
    /// </summary>
    /// <param name="accountTransferRequest"></param>
    /// <returns></returns>
    [HttpPost("cheque-deposit")]
    public async Task<IActionResult> PerformChequeDeposit([FromBody] ChequeDepositRequest chequeDepositRequest)
    {
        try
        {
            // Validate the request
            var validationResult = await _transferValidator.ValidateAsync(AccountTransferRequest.Map(chequeDepositRequest));

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

            // Todo: Check clearing status. If cleared, give value to customer

            HttpContext.Items.TryGetValue("requestId", out var requestIdObj);
            string requestId = requestIdObj?.ToString() ?? string.Empty;

            Log.Information("About to Map Finacle intra transfer request with request: {@Request}", chequeDepositRequest);
            var request = IntraBankTransferRequestFactory.Create(requestId, chequeDepositRequest.ChequeAccountNumber, chequeDepositRequest.BeneficiaryAccountNumber, chequeDepositRequest.Amount,
                    chequeDepositRequest.ChequeAccountName, chequeDepositRequest.ChequeAccountName, "", null, null, null, null, explanationCode: null, chequeDetails: ChequeDepositRequest.Map(chequeDepositRequest));
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
    /// Method to Perform Intra Bank Transfer
    /// </summary>
    /// <param name="accountTransferRequest"></param>
    /// <returns></returns>
    [HttpPost("batch-transfer")]
    public async Task<IActionResult> PerformBatchTransfer([FromBody] BatchTransferRequest accountTransferRequest)
    {
        try
        {
            // Validate the request
            //var validationResult = _transferValidator.ValidateAsync(accountTransferRequest, out string errorMessage);
            if (!_transferValidator.ValidateAsync(accountTransferRequest, out var ErrorMessage))
            {
                Log.Warning("Transfer validation failed: {Error}", ErrorMessage);
                return BadRequest(ResponseManager.ReturnFailedValidations(ErrorMessage));
            }

            HttpContext.Items.TryGetValue("requestId", out var requestIdObj);
            string requestId = requestIdObj?.ToString() ?? string.Empty;

            Log.Information("About to Map Finacle intra transfer request with request: {@Request}", accountTransferRequest);
            var request = IntraBankTransferRequestFactory.CreateBatch(requestId, accountTransferRequest.SourceAccountId, accountTransferRequest.CreditTargets,
                    accountTransferRequest.Remarks, accountTransferRequest.ChannelReference, accountTransferRequest.ChannelId, accountTransferRequest.Currency, accountTransferRequest.ChargeEventId, accountTransferRequest.BatchId, explanationCode: accountTransferRequest.ExplanationCode);
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

    [HttpPost("reversals")]
    public async Task<IActionResult> PerformReversal([FromBody] TransactionIdentification reversalRequest)
    {
        Log.Information("Recieved Request at PerformReversal: {@Request}", reversalRequest);
        try
        {
            var apiCall = await _finacleDbContext.ApiCalls
                .Where(a => a.ResponseParam != null && a.ResponseParam.Contains(reversalRequest.TransactionReference))
                .OrderByDescending(a => a.Id)
                .FirstOrDefaultAsync();

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

            Log.Information("About to Update ApiCall with reversal request: {@Request}", request);
            if (!string.IsNullOrEmpty(response.ReversalTransactionIdentification.TransactionReference))
            {
                apiCall.IsReversed = true;
                apiCall.ReversalResponseParam = JsonSerializer.Serialize(response);
                _finacleDbContext.ApiCalls.Update(apiCall);
                await _finacleDbContext.SaveChangesAsync();
            }
            Log.Information("Back from Updating ApiCall with reversal response: {@Response}", response);

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

    [HttpPost("reversals/{channelReference}")]
    public async Task<IActionResult> PerformReversal(string channelReference)
    {
        Log.Information("Recieved Request at PerformReversal: {@Request}", channelReference);
        var request = new ReversalTransferRequest();

        try
        {
            var apiCall = await _finacleDbContext.ApiCalls
                .Where(a => a.RequestParam.Contains(channelReference))
                .OrderByDescending(a => a.Id)
                .FirstOrDefaultAsync();

            if (apiCall != null && apiCall?.ResponseCode == "200")
            {
                var deserializedResponse = JsonSerializer.Deserialize<IntraBankTransferResponse>(apiCall?.ResponseParam);

                request = new ReversalTransferRequest
                {
                    TransactionIdentification = new TransactionIdentification
                    {
                        TransactionDate = deserializedResponse?.TransactionDate,
                        TransactionReference = deserializedResponse.TransactionReference,
                        Remarks = channelReference
                    }
                };
            }
            else
            {
                return StatusCode(422, ResponseManager.ReturnFinacleCallUnsuccessful(exception: 
                    $"No record found for the given channelReference: {channelReference} to reverse"));
            }

                Log.Information("About to PerformReversal with request: {@Request}", request);
            var response = await _transactionClient.Reverse(apiCall?.RequestId, request);
            Log.Information("Back from PerformReversal with response: {@Response}", response);

            Log.Information("About to Update ApiCall with reversal request: {@Request}", request);
            if (!string.IsNullOrEmpty(response.ReversalTransactionIdentification.TransactionReference))
            {
                apiCall.IsReversed = true;
                apiCall.ReversalResponseParam = JsonSerializer.Serialize(response);
                _finacleDbContext.ApiCalls.Update(apiCall);
                await _finacleDbContext.SaveChangesAsync();
            }
            Log.Information("Back from Updating ApiCall with reversal response: {@Response}", response);

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
