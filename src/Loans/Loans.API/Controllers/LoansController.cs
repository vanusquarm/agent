using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Loans.API.Entities.DTOs;
using Loans.API.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using MassTransit;
using System.Text.Json;
using Newtonsoft.Json;
using Refit;


namespace Loan.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    //[Authorize]
    public class LoansController(ILoanClient loanClient) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> CreateLoan(CreateUnsecuredLoanRequest createUnsecuredLoanRequest, bool isSecured = false)
        {
            try
            {
                if (createUnsecuredLoanRequest!=null)
                {
                    var loan = isSecured ?  await loanClient.CreateSecureLoan((CreateUnsecuredLoanRequest)createUnsecuredLoanRequest) : await loanClient.CreateUnsecureLoan((CreateUnsecuredLoanRequest)createUnsecuredLoanRequest);
                    //var response =await loanClient.CreateUnsecureLoan<CreateUnsecuredLoanResponseJson>(createUnsecuredLoanRequest);

                    return Ok(loan);

                }
                else
                {
                    return NotFound();
                }

            }
            catch (ApiException ex)
            {
                var errorContent = await ex.GetContentAsAsync<LoanError>();
                foreach (var error in errorContent.ErrorDetails)
                {
                    Console.Error.WriteLine($"Error occurred while creating a loan: {error.ErrorMessage}");
                    Console.Error.WriteLine($"Error occurred while creating a loan: {error.ErrorCode}");
                }
                //var errmessage = $"Error occurred while creating a loan: {errorContent}";
                Console.Error.WriteLine($"Error occurred while creating a loan: {ex.Message}");

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }


        [HttpPut("{accountId}")]
        public async Task<IActionResult> UpdateLoan(string accountId , UpdateLoanFeesRequestDto updateLoanFeesRequestDto, bool isSecured = false)
        {
            try
            {
                if (updateLoanFeesRequestDto != null)
                {
                    var loan = isSecured ? await loanClient.UpdateUnsecureLoan(accountId, updateLoanFeesRequestDto) : await loanClient.UpdateUnsecureLoan(accountId, updateLoanFeesRequestDto);
                    return Ok(loan);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while updating a loan: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
        
 
        [HttpPost("Disbursement/{accountId}")]
        public async Task<IActionResult> DisburseLoan(string accountId, DisbursementRequestDTO disburseRequestDTO, bool isSecured = false)
        {
            try
            {
                if (disburseRequestDTO != null) // secured loan condition
                {
                    var result = isSecured ? null : await loanClient.DiburseLoan(accountId,disburseRequestDTO);
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while disbursing a loan: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("PayOff/{accountId}")]
        public async Task<IActionResult> PayOff(string accountId, PayoffRequestDTO payoffRequestDTO, bool isSecured = false)
        {
            try
            {
                if (payoffRequestDTO != null) // secured loan condition
                {
                    var result = isSecured ? null : await loanClient.PayOffLoan(accountId, payoffRequestDTO);
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while disbursing a loan: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

       
        [HttpGet("GetOutstandingInquiry/{customerId}")]
        public async Task<IActionResult> GetOutstandingInquiry(string customerId, bool isSecured = false)
        {
            try
            {
               
                if (!string.IsNullOrEmpty(customerId)) // secured loan condition
                {
                    var request = new Dictionary<string, string>
                    {
                        { "customerId", $"{ customerId }" }
                    };
                    var result = isSecured ? null : await loanClient.GetOutStandingIniquiry(request);
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while getting an inquiry: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("SchedulePayment/{accountId}")]
        public async Task<IActionResult> SchedulePayment(string accountId, SchedulePaymentDTO schedulePaymentDTO, bool isSecured = false)
        {
            try
            {
                if (schedulePaymentDTO != null) // secured loan condition
                {
                    var result = isSecured ? null : await loanClient.SchedulePayment(accountId, schedulePaymentDTO);
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while disbursing a loan: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("InterestDetails/{accountId}")]
        public async Task<IActionResult> GetInterestDetails(string accountId, bool isSecured = false)
        {
            try
            {
                if (!string.IsNullOrEmpty(accountId)) // secured loan condition
                {
                    var result = isSecured ? null : await loanClient.GetUnsecuredLoanInterest(accountId);
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while getting an interest: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPut("InterestDetails/{accountId}")]
        public async Task<IActionResult> ModifyLoanInterestDetails(string accountId, UnsecuredLoanInterestRateDto unsecuredLoanInterestRateDto, bool isSecured = false)
        {
            try
            {
                if (!string.IsNullOrEmpty(accountId)) // secured loan condition
                {
                    var result = isSecured ? null : await loanClient.ModifyUnsecuredLoanInterest(accountId,unsecuredLoanInterestRateDto);
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while getting an interest: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{loanId}")]
        public async Task<IActionResult> GetLoanDetails(string loanId, bool isSecured = false)
        {
            if (string.IsNullOrEmpty(loanId)) {  return NotFound(); }

            var loans = isSecured ? await loanClient.GetSecuredLoanDetails(loanId) : await loanClient.GetLoanDetails(loanId);
            return Ok(loans);
        }
        [HttpGet("unsecuredLoan/{loanId}")]
        public async Task<IActionResult> GetUnsecuredLoanDetails(string loanId, bool isSecured = false)
        {
            if (string.IsNullOrEmpty(loanId)) { return NotFound(); }

            var loans = await loanClient.GetLoanDetails(loanId);
            return Ok(loans);
        }


        [HttpPatch("{loanId}")]
        public async Task<IActionResult> PayOffLoan(string loanId,PayoffRequestDTO payoffRequestDTO, bool isSecured = false)
        {
            var loans = isSecured ? await loanClient.PayOffSecuredLoan(loanId) : await loanClient.PayOffLoan(loanId , payoffRequestDTO);
            return Ok(loans);
        }

        [HttpPost("inquiry")]
        public async Task<IActionResult> LoanInquiry(LoanInquiry loanInquiry)
        {
            try
            {
                if (!string.IsNullOrEmpty(loanInquiry.CustomerId)) 
                {
                    var result = await loanClient.GetLoanInquiry(loanInquiry);
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while getting an interest: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
       
    }
}
