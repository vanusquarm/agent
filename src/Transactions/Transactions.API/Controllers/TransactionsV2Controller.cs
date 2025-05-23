using Finacle.Core;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Transactions.API.Entities.DTOs;
using Transactions.API.Services;
using EodStatusRequest = Transactions.API.Entities.DTOs.EodStatusRequest;
using TransactionRequest = Transactions.API.Entities.DTOs.TransactionRequest;
using Microsoft.AspNetCore.Authorization;
using Transactions.API.Filters;
using Transactions.API.Entities;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Linq.Expressions;
using Transactions.API.Validators;
using static MassTransit.ValidationResultExtensions;
using Finacle.Infrastructure.Utils;

namespace Transactions.API.Controllers;

[ApiController]
[Route("api/v2/transactions")]

public class TransactionsV2Controller : ControllerBase
{
    private readonly ITransactionClient _transactionClient;
    private readonly TransferValidator _transferValidator;

    public TransactionsV2Controller(ITransactionClient transactionClient, TransferValidator transferValidator)
    {
        _transactionClient = transactionClient;
        _transferValidator = transferValidator;
    }

    /// <summary>
    /// Method to Perform Intra Bank Transfer
    /// </summary>
    /// <param name="accountTransferRequest"></param>
    /// <returns></returns>
    [HttpPost("transfer")]
    [ServiceFilter(typeof(ValidateModelFilter))]
    public async Task<IActionResult> PerformIntraBankTransferV2([FromBody] AccountTransferRequest accountTransferRequest)
    {
        Regex regex = new(@"\[[^\]]*\]", RegexOptions.None, TimeSpan.FromSeconds(1));
        JsonElement response;
        try
        {

            //Validate SQL Injection/Special Chaaracters
            var validator = new GenericSqlInjectSpecialCharsValidator<AccountTransferRequest>(
            new List<Expression<Func<AccountTransferRequest, string>>>
            {
                x => x.SourceAccountId,
                x => x.DestinationAccountId,
                x => x.UserPartTransactionCode,
                x => x.Remarks,
                x => x.ChannelReference,
                x => x.ChannelId,
                x => x.ChargeEventId,
                x => x. BatchId,
                x => x.DocNum,
            });

            var result = validator.Validate(accountTransferRequest);

            if (!result.IsValid)
            {
                var firstError = result.Errors.FirstOrDefault();

                return BadRequest(ResponseManager.ReturnFailedValidations(null, firstError!.ErrorMessage));
            }
        

            // Validate the request
            var validationResult = await _transferValidator.ValidateAsync(accountTransferRequest);

            if (!validationResult.IsValid)
            {
                Log.Warning("Transfer validation failed: {Error}", validationResult.ErrorMessage);
                //return BadRequest(new { validationResult.ErrorMessage });
                return BadRequest(ResponseManager.ReturnFailedValidations(null, validationResult.ErrorMessage));

            }

            if (validationResult.RequiresAdditionalVerification)
            {
                //return BadRequest(new
                //{
                //    Message = "Additional verification required",
                //    Methods = validationResult.AvailableVerificationMethods
                //});
                return BadRequest(ResponseManager.ReturnFailedValidations(null, validationResult.ErrorMessage));

            }
            var requestId = System.Text.Json.JsonSerializer.Serialize(accountTransferRequest).ToHash();

            Log.Information("About to Map Finacle intra transfer request with request: {@Request}", accountTransferRequest);
            var request = IntraBankTransferRequestFactory.Create(requestId, accountTransferRequest.SourceAccountId, accountTransferRequest.DestinationAccountId, accountTransferRequest.Amount,
                    accountTransferRequest.Remarks, accountTransferRequest.ChannelReference, accountTransferRequest.ChannelId, accountTransferRequest.Currency, accountTransferRequest.DocNum, 
                    accountTransferRequest.ChargeEventId,accountTransferRequest.BatchId, userPartTransactionCode: accountTransferRequest.UserPartTransactionCode);
            Log.Information("Back from Mapping Finacle intra transfer request with finacle request: {@Request}", request);

            Log.Information("About to PerformIntraBankTransfer with request: {@Request}", request);
            dynamic apiResponse = await _transactionClient.PerformIntraBankTransfer(accountTransferRequest.ChannelReference, request);

            response=apiResponse;


            Log.Information("Back from Performing IntraBankTransfer with response: {@Response}", response);

            if (response.ValueKind == JsonValueKind.Undefined || response.ValueKind == JsonValueKind.Null)
            {
                Log.Information("An error occurred while processing the transaction: {@Request}", request);
                return BadRequest(ResponseManager.ReturnFailed(null, "Failed to process the intra - bank transfer."));
            }

  
            if (response.TryGetProperty("errorDetails", out _))
            {
                return BadRequest(ResponseManager.ReturnFailedValidations(null, null));
            }

            if (response.TryGetProperty("tranStatus", out _) && response.GetProperty("tranStatus").GetString() == "DUPSUCESS")
            {
                return BadRequest(ResponseManager.ReturnFinacleDuplicateRecord(apiResponse, null));
            }


            return Ok(ResponseManager.ReturnSuccess(response, "Transfer successful"));
        }
        catch (Refit.ApiException apiEx)
        {
            Log.Error("An exception has occured at PerformIntraBankTransfer with message: {Message} and stack trace: {StackTrace}", apiEx.Message, apiEx.StackTrace);
            //return StatusCode((int)apiEx.StatusCode, apiEx.Content ?? apiEx.Message);

            string errorMessage = null, errorCode = null;
            if (apiEx.Content != null)
            {

               errorCode = ((int)apiEx.StatusCode).ToString();
               errorMessage = apiEx.Content;
            }

            if (errorCode == "W0205")
                return BadRequest(ResponseManager.ReturnInsufficientBalance(null, regex.Replace(errorMessage, "").Trim()));
            else
                return StatusCode(500,ResponseManager.ReturnFailed(null, regex.Replace(errorMessage, "").Trim()));
        }
        catch (Exception ex)
        {
            Log.Error("An exception has occured at PerformIntraBankTransfer with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
            return StatusCode(500, ResponseManager.ReturnFailed(null, regex.Replace(ex.Message, "").Trim()));
        }
    }

}
