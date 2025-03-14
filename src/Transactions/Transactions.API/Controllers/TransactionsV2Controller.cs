using Finacle.Core;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Serilog;
using Transactions.API.Entities;
using Transactions.API.Entities.DTOs;
using Transactions.API.Entities.Models;
using Transactions.API.Services;
using EodStatusRequest = Transactions.API.Entities.DTOs.EodStatusRequest;
using TransactionRequest = Transactions.API.Entities.DTOs.TransactionRequest;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Security.Cryptography.Xml;
using Transactions.API.Filters;
using System.Text.RegularExpressions;

namespace Transactions.API.Controllers;

[ApiController]
[Route("api/v2/transactions")]
public class TransactionsV2Controller : ControllerBase
{
    private readonly FinacleDbContext _context;
    private readonly ITransactionClient _transactionClient;
    private readonly FinacleClient _finacleClient;
    private readonly AccountUtil _accountUtil;

    public TransactionsV2Controller(FinacleDbContext context, ITransactionClient transactionClient, AccountUtil accountUtil)
    {
        _context = context;
        _transactionClient = transactionClient;
        _accountUtil = accountUtil;
    }

    /// <summary>
    /// Method to Perform Intra Bank Transfer
    /// </summary>
    /// <param name="accountTransferRequest"></param>
    /// <returns></returns>
    [HttpPost("transfer")]
    [ServiceFilter(typeof(ValidateModelFilter))]
    public async Task<IActionResult> PerformIntraBankTransfer([FromBody] AccountTransferRequest accountTransferRequest)
    {
        Regex regex = new(@"\[[^\]]*\]", RegexOptions.None, TimeSpan.FromSeconds(1));

        var apirequestresponse = new Call
        {
            Channel = accountTransferRequest.ChannelId,
            RequestParam = JsonSerializer.Serialize(accountTransferRequest),
            RequestDate = DateTime.UtcNow,
            Controller = "TransactionController",
            EndPoint = "transactions/api/v1/transfer",
            Amount = accountTransferRequest.Amount,
            Apiservice = "PerformIntraBankTransfer",
            RequestId = accountTransferRequest.ChannelReference
        };
        try
        {
            if (accountTransferRequest == null)
            {
                apirequestresponse.TraceId = "Invalid request data.";
                apirequestresponse.ResponseCode = "400";
                Log.Information("Invalid request data.: {@Request}", accountTransferRequest);
                return BadRequest("Invalid request data.");
            }

            Log.Information("About to Map Finacle intra transfer request with request: {@Request}", accountTransferRequest);
            var request = IntraBankTransferRequestFactory.Create(accountTransferRequest.SourceAccountId, accountTransferRequest.DestinationAccountId, accountTransferRequest.Amount,
                    accountTransferRequest.Remarks, accountTransferRequest.ChannelReference, accountTransferRequest.ChannelId, accountTransferRequest.DocNum, accountTransferRequest.ChargeEventId);
            Log.Information("Back from Mapping Finacle intra transfer request with finacle request: {@Request}", request);

            Log.Information("About to PerformIntraBankTransfer with request: {@Request}", request);
            var response = await _transactionClient.PerformIntraBankTransfer(accountTransferRequest.ChannelReference, request);
            apirequestresponse.ReponseTime = DateTime.UtcNow;
            apirequestresponse.ResponseCode = "200";
            apirequestresponse.ResponseParam = JsonSerializer.Serialize(response);
            Log.Information("Back from Performing IntraBankTransfer with response: {@Response}", response);

            var jsonResponse = JObject.Parse(apirequestresponse.ResponseParam);


            if (response == null)
            {
                apirequestresponse.TraceId = "Invalid request data.";
                apirequestresponse.ResponseCode = "500";
                Log.Information("An error occurred while processing the transaction: {@Request}", request);
                //return StatusCode(500, "Failed to process the intra-bank transfer.");

                return Ok(ResponseManager.ReturnFailed(null, "Failed to process the intra - bank transfer."));
            }

            if (response != null && jsonResponse.ContainsKey("errorDetails"))
            {
                return Ok(ResponseManager.ReturnFailedValidations(null, null));

            }

            if (jsonResponse != null && jsonResponse.ContainsKey("tranStatus") && jsonResponse["tranStatus"].ToString() == "DUPSUCESS")
            {
                return Ok(ResponseManager.ReturnFinacleDuplicateRecord(response, null));
            }

            return Ok(ResponseManager.ReturnSuccess(response, "Transfer successful"));
        }
        catch (Refit.ApiException apiEx)
        {
            apirequestresponse.TraceId = $"{apiEx.Message}: {apiEx.StackTrace}";
            apirequestresponse.ResponseCode = apiEx.StatusCode.ToString();


            string errorMessage = null, errorCode=null;
            if (apiEx.Content != null)
            {
               var jsonResponse = JObject.Parse(apiEx.Content);

                errorCode = (string)jsonResponse["errorDetails"]?.FirstOrDefault()?["errorCode"];
                errorMessage = (string)jsonResponse["errorDetails"]?.FirstOrDefault()?["errorMessage"];

            }

            Log.Error("An exception has occured at PerformIntraBankTransfer with message: {Message} and stack trace: {StackTrace}", apiEx.Message, apiEx.StackTrace);
            //return StatusCode((int)apiEx.StatusCode, apiEx.Content ?? apiEx.Message);

            if(errorCode == "W0205")
                return Ok(ResponseManager.ReturnInsufficientBalance(null, regex.Replace(errorMessage, "").Trim()));
            else
                return Ok(ResponseManager.ReturnFailed(null, regex.Replace(errorMessage, "").Trim()));
        }
        catch (Exception ex)
        {
            apirequestresponse.TraceId = $"{ex.Message}: {ex.StackTrace}";
            apirequestresponse.ResponseCode = "500";
            Log.Error("An exception has occured at PerformIntraBankTransfer with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
            //return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            return Ok(ResponseManager.ReturnFailed(null, null));
        }
        //finally
        //{
        //    await _context.ApiCalls.AddAsync(apirequestresponse);
        //    await _context.SaveChangesAsync();
        //}
    }


}
