using Microsoft.AspNetCore.Mvc;
using Todo.API.Services;

namespace Todo.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BranchesController : ControllerBase
{
    private readonly IBranchesClient _branchesClient;

    public BranchesController(IBranchesClient branchesClient)
    {
        _branchesClient = branchesClient;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetBranchDetailsAsync()
    {
        try
        {
            var branchDetails = await _branchesClient.GetBranchDetails();

            if (branchDetails?.BranchList == null || !branchDetails.BranchList.Any())
            {
                return NotFound("No branch details found.");
            }

            return Ok(branchDetails);
        }
        catch (HttpRequestException httpRequestException)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new 
            {
                Message = "Failed to retrieve branch details due to service unavailability.",
                Error = httpRequestException.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new 
            {
                Message = "An unexpected error occurred while retrieving branch details.",
                Error = ex.Message
            });
        }
    }
    
    
    [HttpGet("{branchCode}")]
    public async Task<IActionResult> GetBranchDetailsByCodeAsync([FromRoute] string branchCode, [FromQuery] string bankCode)
    {
        try
        {
            var branchDetails = await _branchesClient.GetBranchDetailsByCode(branchCode, bankCode);

            if (branchDetails == null)
            {
                return NotFound($"Branch with code '{branchCode}' and bank code '{bankCode}' not found.");
            }

            return Ok(branchDetails);
        }
        catch (HttpRequestException httpRequestException)
        {
            // Handles errors related to HTTP requests
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                Message = "Failed to retrieve branch details due to service unavailability.",
                Error = httpRequestException.Message
            });
        }
        catch (Exception ex)
        {
            // General exception handling
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Message = "An unexpected error occurred while retrieving branch details.",
                Error = ex.Message
            });
        }
    }

    [HttpGet("customers/{customerId}/accounts")]
    public async Task<IActionResult> GetCustomerAccountsAsync(
        string customerId,
        [FromQuery] string includeClosedAccounts = "N",
        [FromQuery] int setNumber = 1,
        [FromQuery] int maximumRecords = 10)
    {
        try
        {
            var response = await _branchesClient.GetCustomerAccounts(customerId, includeClosedAccounts, setNumber, maximumRecords);

            if (response?.BranchList == null || !response.BranchList.Any())
            {
                return NotFound($"No accounts found for customer ID '{customerId}'.");
            }

            return Ok(response);
        }
        catch (HttpRequestException httpRequestException)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                Message = "Failed to retrieve customer accounts due to service unavailability.",
                Error = httpRequestException.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Message = "An unexpected error occurred while retrieving customer accounts.",
                Error = ex.Message
            });
        }
    }
    
    
}