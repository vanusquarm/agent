using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Net.Http;
using Branches.API.Services;
using Microsoft.AspNetCore.Authorization;
using Finacle.Infrastructure.Persistence;
using System.Security.Cryptography;

namespace Branches.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class BranchesController : ControllerBase
{
    private readonly IBranchesClient _branchesClient;
    private readonly FinacleDbContext _finacleDbContext;

    public BranchesController(IBranchesClient branchesClient, FinacleDbContext finacleDbContext)
    {
        _branchesClient = branchesClient;
        _finacleDbContext = finacleDbContext;
    }

    /// <summary>
    /// Async Method to Get Branch Details
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetBranchDetailsAsync()
    {
        Log.Information("Recieved Request at GetBranchDetailsAsync");
        try
        {
            Log.Information("About to GetBranchDetails with request");
            var branchDetails = await _branchesClient.GetBranchDetails();
            Log.Information("Back from Getting BranchDetails with response: {@Response}", branchDetails);

            if (branchDetails?.BranchList == null || !branchDetails.BranchList.Any())
            {
                Log.Information("No branch details found");
                return NotFound("No branch details found.");
            }
            return Ok(branchDetails);
        }
        catch (HttpRequestException httpRequestException)
        {
            Log.Error("An exception has occured at GetBranchDetailsAsync with message: {Message} and stack trace: {StackTrace}", httpRequestException.Message, httpRequestException.StackTrace);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new 
            {
                Message = "Failed to retrieve branch details due to service unavailability.",
                Error = httpRequestException.Message
            });
        }
        catch (Exception ex)
        {
            Log.Error("An exception has occured at GetBranchDetailsAsync with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
            return StatusCode(StatusCodes.Status500InternalServerError, new 
            {
                Message = "An unexpected error occurred while retrieving branch details.",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Async Method Get Branch Details By Code
    /// </summary>
    /// <param name="branchCode"></param>
    /// <param name="bankCode"></param>
    /// <returns></returns>
    [HttpGet("{branchCode}")]
    public async Task<IActionResult> GetBranchDetailsByCodeAsync([FromRoute] string branchCode, [FromQuery] string bankCode)
    {
        Log.Information("Recieved Request at GetBranchDetailsByCodeAsync: {@Request}", (bankCode, branchCode));
        try
        {
            Log.Information("About to GetBranchDetailsByCode with request. {@Request}", (bankCode, branchCode));
            var branchDetails = await _branchesClient.GetBranchDetailsByCode(branchCode, bankCode);
            Log.Information("Back from Getting BranchDetailsByCode with response: {@Response}", branchDetails);

            if (branchDetails == null)
            {
                Log.Information("Branch with code: {BranchCode}, and bank code {BankCode} not found.", branchCode, bankCode);
                return NotFound($"Branch with code {branchCode} and bank code {bankCode} not found.");
            }

            return Ok(branchDetails);
        }
        catch (HttpRequestException httpRequestException)
        {
            // Handles errors related to HTTP requests
            Log.Error("An exception has occured at GetBranchDetailsByCode with message: {Message} and stack trace: {StackTrace}", httpRequestException.Message, httpRequestException.StackTrace);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                Message = "Failed to retrieve branch details due to service unavailability.",
                Error = httpRequestException.Message
            });
        }
        catch (Exception ex)
        {
            // General exception handling
            Log.Error("An exception has occured at GetBranchDetailsByCode with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Message = "An unexpected error occurred while retrieving branch details.",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Async Method to Get Customer Accounts
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="includeClosedAccounts"></param>
    /// <param name="setNumber"></param>
    /// <param name="maximumRecords"></param>
    /// <returns></returns>
    [HttpGet("customers/{customerId}/accounts")]
    public async Task<IActionResult> GetCustomerAccountsAsync(string customerId, [FromQuery] string includeClosedAccounts = "N", [FromQuery] int setNumber = 1, [FromQuery] int maximumRecords = 10)
    {
        Log.Information("Recieved Request at GetCustomerAccountsAsync: {@Request}", (customerId, includeClosedAccounts, setNumber, maximumRecords));
        try
        {
            Log.Information("About to GetCustomerAccounts with request. {@Request}", (customerId, includeClosedAccounts, setNumber, maximumRecords));
            var response = await _branchesClient.GetCustomerAccounts(customerId, includeClosedAccounts, setNumber, maximumRecords);
            Log.Information("Back from Getting CustomerAccounts with response: {@Response}", response);

            if (response?.BranchList == null || !response.BranchList.Any())
            {
                Log.Information("No accounts found for customer ID: {customerId}", customerId);
                return NotFound($"No accounts found for customer ID '{customerId}'.");
            }

            return Ok(response);
        }
        catch (HttpRequestException httpRequestException)
        {
            Log.Error("An exception has occured at GetCustomerAccountsAsync with message: {Message} and stack trace: {StackTrace}", httpRequestException.Message, httpRequestException.StackTrace);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                Message = "Failed to retrieve customer accounts due to service unavailability.",
                Error = httpRequestException.Message
            });
        }
        catch (Exception ex)
        {
            Log.Error("An exception has occured at GetCustomerAccountsAsync with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Message = "An unexpected error occurred while retrieving customer accounts.",
                Error = ex.Message
            });
        }
    }

    [HttpGet("holidays/is-holiday")]
    public async Task<IActionResult> IsHoliday([FromQuery] DateTime date)
    {
        Log.Information("Recieved Request at IsHoliday: {@Request}", date);
        try
        {
            var userAgent = Request.Headers["User-Agent"].ToString();
            var request = new Dictionary<string, string>
            {
                { "solId", "201" },
                { "tranDate", date.ToString("yyyy-MM-dd") }
            };

            Log.Information("About to check if the date is a holiday with request: {@Request}", request);


            var response = _branchesClient.GetHoliday(request).Result;
            return Ok(bool.Parse(response?.HolidayStatus?.Status)); 


            // Failover to using Database
            //var holiday = _finacleDbContext.Holidays
            //    .FirstOrDefault(x => x.Date.Date == date.Date);
            //Log.Information("Back from checking if the date is a holiday with response: {@Response}", holiday);

            //return Ok(holiday != null);
        }
        catch (Exception)
        {
            Log.Error("An exception has occured at IsHoliday with message: {Message} and stack trace: {StackTrace}", "An error occurred while checking if the date is a holiday.", "StackTrace");
            return Ok(false);
        }
    }

}