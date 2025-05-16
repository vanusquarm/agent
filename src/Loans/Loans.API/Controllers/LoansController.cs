using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Loans.API.Entities.DTOs;
using Loans.API.Services;
using Serilog;
using Finacle.Infrastructure.Logging;


namespace Loan.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class LoansController(ILoanClient loanClient) : ControllerBase
    {
        /// <summary>
        /// Method to Create Loan
        /// </summary>
        /// <param name="createUnsecuredLoanRequest"></param>
        /// <param name="isSecured"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateLoan(CreateUnsecuredLoanRequest createUnsecuredLoanRequest, bool isSecured = false)
        {
            Log.Information("Recieved Request at CreateLoan: {@Request}", createUnsecuredLoanRequest);
            try
            {
                if (createUnsecuredLoanRequest != null)
                {
                    Log.Information("About to CreateSecureLoan with request: {@Request}", createUnsecuredLoanRequest);
                    var loan = isSecured ? await loanClient.CreateSecureLoan((CreateUnsecuredLoanRequest)createUnsecuredLoanRequest) : await loanClient.CreateUnsecureLoan((CreateUnsecuredLoanRequest)createUnsecuredLoanRequest);
                    Log.Information("Back from Creating SecureLoan with response: {@Response}", loan);
                    return Ok(loan);
                }
                else
                {
                    Log.Information("No record found for request: {@Request}", createUnsecuredLoanRequest);
                    return NotFound();
                }
            }
            catch (Refit.ApiException ex)
            {
                Log.Error("Finacle call unsuccessful. Message: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }

        /// <summary>
        /// Method to Update Loan
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="updateLoanFeesRequestDto"></param>
        /// <param name="isSecured"></param>
        /// <returns></returns>
        [HttpPut("{accountId}")]
        public async Task<IActionResult> UpdateLoan(string accountId, UpdateLoanFeesRequestDto updateLoanFeesRequestDto, bool isSecured = false)
        {
            Log.Information("Recieved Request at UpdateLoan: {@Request}", updateLoanFeesRequestDto);
            try
            {
                if (updateLoanFeesRequestDto != null)
                {
                    Log.Information("About to UpdateLoan with request: {@Request}", updateLoanFeesRequestDto);
                    var loan = isSecured ? await loanClient.UpdateUnsecureLoan(accountId, updateLoanFeesRequestDto) : await loanClient.UpdateUnsecureLoan(accountId, updateLoanFeesRequestDto);
                    Log.Information("Back from Updating Loan with response: {@Response}", loan);
                    return Ok(loan);
                }
                else
                {
                    Log.Information("No record found for request: {@Request}", updateLoanFeesRequestDto);
                    return NotFound();
                }
            }
            catch (Refit.ApiException ex)
            {
                Log.Error("Finacle call unsuccessful. Message: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }

        /// <summary>
        /// Method to Disburse Loan
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="disburseRequestDTO"></param>
        /// <param name="isSecured"></param>
        /// <returns></returns>
        [HttpPost("Disbursement/{accountId}")]
        public async Task<IActionResult> DisburseLoan(string accountId, DisbursementRequestDTO disburseRequestDTO, bool isSecured = false)
        {
            Log.Information("Recieved Request at DisburseLoan: {@Request}", (disburseRequestDTO, accountId));
            try
            {
                if (disburseRequestDTO != null) // secured loan condition
                {
                    Log.Information("About to DisburseLoan with request: {@Request}", (disburseRequestDTO, accountId));
                    var result = isSecured ? null : await loanClient.DiburseLoan(accountId, disburseRequestDTO);
                    Log.Information("Back from Disbursing Loan with response: {@Response}", result);
                    return Ok(result);
                }
                else
                {
                    Log.Information("No record found for request: {@Request}", disburseRequestDTO);
                    return NotFound();
                }
            }
            catch (Refit.ApiException ex)
            {
                Log.Error("Finacle call unsuccessful. Message: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }

        /// <summary>
        /// Method to PayOff
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="payoffRequestDTO"></param>
        /// <param name="isSecured"></param>
        /// <returns></returns>
        [HttpPost("PayOff/{accountId}")]
        public async Task<IActionResult> PayOff(string accountId, PayoffRequestDTO payoffRequestDTO, bool isSecured = false)
        {
            Log.Information("Recieved Request at PayOff: {@Request}", (payoffRequestDTO, accountId));
            try
            {
                if (payoffRequestDTO != null) // secured loan condition
                {
                    Log.Information("About to PayOff with request: {@Request}", (payoffRequestDTO, accountId));
                    var paid = isSecured ? null : await loanClient.PayOffLoan(accountId, payoffRequestDTO);
                    Log.Information("Back from PayingOff with response: {@Response}", paid);
                    if (paid != null && !string.IsNullOrEmpty(paid.AccountIdentification))
                    {
                        if (accountId.Length > 3)
                        {
                            var branchCode = accountId.Substring(1, 3);
                            var res = await loanClient.CloseAccount(accountId, branchCode);
                        }

                        return Ok(paid);
                    }

                    return NotFound();
                }
                else
                {
                    Log.Information("No record found for request: {@Request}", payoffRequestDTO);
                    return NotFound();
                }
            }
            catch (Refit.ApiException ex)
            {
                Log.Error("Finacle call unsuccessful. Message: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }

        /// <summary>
        /// Method to Get Outstanding Inquiry
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="isSecured"></param>
        /// <returns></returns>
        [HttpGet("outstanding-inquiry/{customerId}")]
        public async Task<IActionResult> GetOutstandingInquiry(string customerId, bool isSecured = false)
        {
            Log.Information("Recieved Request at outstanding-inquiry: {Request}", customerId);
            try
            {

                if (!string.IsNullOrEmpty(customerId)) // secured loan condition
                {
                    var request = new Dictionary<string, string> { { "customerId", $"{customerId}" } };

                    Log.Information("About to outstanding-inquiry with request: {@Request}", request);
                    var result = isSecured ? null : await loanClient.GetOutStandingIniquiry(request);
                    Log.Information("Back from Getting OutstandingInquiry with response: {@Response}", result);
                    return Ok(result);
                }
                else
                {
                    Log.Information("No record found for request: {Request}", customerId);
                    return NotFound();
                }
            }
            catch (Refit.ApiException ex)
            {
                Log.Error("Finacle call unsuccessful. Message: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }

        /// <summary>
        /// Method to Schedule Payment
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="schedulePaymentDTO"></param>
        /// <param name="isSecured"></param>
        /// <returns></returns>
        [HttpPost("SchedulePayment/{accountId}")]
        public async Task<IActionResult> SchedulePayment(string accountId, SchedulePaymentDTO schedulePaymentDTO, bool isSecured = false)
        {
            Log.Information("Recieved Request at SchedulePayment: {@Request}", (schedulePaymentDTO, accountId));
            try
            {
                if (schedulePaymentDTO != null) // secured loan condition
                {
                    Log.Information("About to SchedulePayment with request: {@Request}", (schedulePaymentDTO, accountId));
                    var result = isSecured ? null : await loanClient.SchedulePayment(accountId, schedulePaymentDTO);
                    Log.Information("Back from Scheduling Payment with response: {@Response}", result);
                    return Ok(result);
                }
                else
                {
                    Log.Information("No record found for request: {@Request}", (schedulePaymentDTO, accountId));
                    return NotFound();
                }
            }
            catch (Refit.ApiException ex)
            {
                Log.Error("Finacle call unsuccessful. Message: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }

        /// <summary>
        /// Method to Get Interest Details
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="isSecured"></param>
        /// <returns></returns>
        [HttpGet("InterestDetails/{accountId}")]
        public async Task<IActionResult> GetInterestDetails(string accountId, bool isSecured = false)
        {
            Log.Information("Recieved Request at GetInterestDetails: {Request}", accountId);
            try
            {
                if (!string.IsNullOrEmpty(accountId)) // secured loan condition
                {
                    Log.Information("About to GetInterestDetails with request: {Request}", accountId);
                    var result = isSecured ? null : await loanClient.GetUnsecuredLoanInterest(accountId);
                    Log.Information("Back from Getting InterestDetails with response: {@Response}", result);
                    return Ok(result);
                }
                else
                {
                    Log.Information("No record found for request: {Request}", accountId);
                    return NotFound();
                }
            }
            catch (Refit.ApiException ex)
            {
                Log.Error("Finacle call unsuccessful. Message: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }

        /// <summary>
        /// Method to Modify Loan Interest Details
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="unsecuredLoanInterestRateDto"></param>
        /// <param name="isSecured"></param>
        /// <returns></returns>
        [HttpPut("InterestDetails/{accountId}")]
        public async Task<IActionResult> ModifyLoanInterestDetails(string accountId, UnsecuredLoanInterestRateDto unsecuredLoanInterestRateDto, bool isSecured = false)
        {
            Log.Information("Recieved Request at ModifyLoanInterestDetails: {@Request}", (accountId, unsecuredLoanInterestRateDto));
            try
            {
                if (!string.IsNullOrEmpty(accountId)) // secured loan condition
                {
                    Log.Information("About to ModifyUnsecuredLoanInterest with request: {@Request}", (accountId, unsecuredLoanInterestRateDto));
                    var result = isSecured ? null : await loanClient.ModifyUnsecuredLoanInterest(accountId, unsecuredLoanInterestRateDto);
                    Log.Information("Back from Modifying UnsecuredLoanInterest with response: {@Response}", result);
                    return Ok(result);
                }
                else
                {
                    Log.Information("No record found for request: {Request}", accountId);
                    return NotFound();
                }
            }
            catch (Refit.ApiException ex)
            {
                Log.Error("Finacle call unsuccessful. Message: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }

        /// <summary>
        /// Method to Get Loan Details
        /// </summary>
        /// <param name="loanId"></param>
        /// <param name="isSecured"></param>
        /// <returns></returns>
        [HttpGet("{loanId}")]
        public async Task<IActionResult> GetLoanDetails(string loanId, bool isSecured = false)
        {
            try
            {
                Log.Information("Recieved Request at GetLoanDetails: {Request}", loanId);
                if (string.IsNullOrEmpty(loanId)) { Log.Information("No record found for request: {Request}", loanId); return NotFound(); }

                Log.Information("About to GetLoanDetails with request: {Request}", loanId);
                var loans = isSecured ? await loanClient.GetSecuredLoanDetails(loanId) : await loanClient.GetLoanDetails(loanId);
                Log.Information("Back from Getting LoanDetails with response: {@Response}", loans);
                return Ok(loans);
            }
            catch (Refit.ApiException ex)
            {
                Log.Error("Finacle call unsuccessful. Message: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }

        /// <summary>
        /// Method to Get Unsecured Loan Details
        /// </summary>
        /// <param name="loanId"></param>
        /// <param name="isSecured"></param>
        /// <returns></returns>
        [HttpGet("unsecuredLoan/{loanId}")]
        public async Task<IActionResult> GetUnsecuredLoanDetails(string loanId, bool isSecured = false)
        {
            try
            {
                Log.Information("Recieved Request at GetUnsecuredLoanDetails: {Request}", loanId);
                if (string.IsNullOrEmpty(loanId)) { Log.Information("No record found for request: {Request}", loanId); return NotFound(); }

                Log.Information("About to GetUnsecuredLoanDetails with request: {Request}", loanId);
                var loans = await loanClient.GetLoanDetails(loanId);
                Log.Information("Back from Getting UnsecuredLoanDetails with response: {@Response}", loans);
                return Ok(loans);
            }
            catch (Refit.ApiException ex)
            {
                Log.Error("Finacle call unsuccessful. Message: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }

        /// <summary>
        /// Method to PayOff Loan
        /// </summary>
        /// <param name="loanId"></param>
        /// <param name="payoffRequestDTO"></param>
        /// <param name="isSecured"></param>
        /// <returns></returns>
        [HttpPatch("{loanId}")]
        public async Task<IActionResult> PayOffLoan(string loanId, PayoffRequestDTO payoffRequestDTO, bool isSecured = false)
        {
            try
            {
                Log.Information("Recieved Request at PayOffLoan: {@Request}", (payoffRequestDTO, loanId));

                Log.Information("About to PayOffLoan with request: {@Request}", (payoffRequestDTO, loanId));
                var loans = isSecured ? await loanClient.PayOffSecuredLoan(loanId) : await loanClient.PayOffLoan(loanId, payoffRequestDTO);
                Log.Information("Back from PayingOff Loan with response: {@Response}", loans);
                return Ok(loans);
            }
            catch (Refit.ApiException ex)
            {
                Log.Error("Finacle call unsuccessful. Message: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }

        /// <summary>
        /// Method to do Loan Inquiry
        /// </summary>
        /// <param name="loanInquiry"></param>
        /// <returns></returns>
        [HttpPost("inquiry")]
        public async Task<IActionResult> LoanInquiry(LoanInquiry loanInquiry)
        {
            Log.Information("Recieved Request at LoanInquiry: {@Request}", loanInquiry);
            try
            {
                if (!string.IsNullOrEmpty(loanInquiry.CustomerId))
                {
                    Log.Information("About to do LoanInquiry with request: {@Request}", loanInquiry);
                    var result = await loanClient.GetLoanInquiry(loanInquiry);
                    Log.Information("Back from doing LoanInquiry Loan with response: {@Response}", result);
                    return Ok(result);
                }
                else { Log.Information("No record found for request: {Request}", loanInquiry.CustomerId); return NotFound(); }
            }
            catch (Refit.ApiException ex)
            {
                Log.Error("Finacle call unsuccessful. Message: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode((int)ex.StatusCode, ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
            }
        }

    }
}
