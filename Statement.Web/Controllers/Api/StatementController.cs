using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Statement.Web.Filters;
using Statement.Web.Helpers;
using Statement.Web.Interfaces;
using Statement.Web.Models;

namespace Statement.Web.Controllers.Api
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [ApiKeyAuth]
    public class StatementController : ControllerBase
    {
        private readonly ILogger<StatementController> _logger;
        private readonly IGeneratePdfService _pdfService;
        private readonly IEmailService _emailService;
        private readonly IStatementClient _statementClient;
        private readonly IConfiguration _configuration;

        public StatementController(ILogger<StatementController> logger, IGeneratePdfService pdfService, IEmailService emailService, IStatementClient statementClient, IConfiguration configuration)
        {
            _logger = logger;
            _pdfService = pdfService;
            _emailService = emailService;
            _statementClient = statementClient;
            _configuration = configuration;
        }

        /// <summary>
        /// Send PDF statement as email to customer
        /// </summary>
        /// <param name="request">Email request with account details</param>
        /// <returns>Success/failure response</returns>
        [HttpPost("email-statement")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> SendStatementEmail([FromBody] EmailStatementRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Invalid request data" });

                //if (string.IsNullOrEmpty(request.Email))
                //    return BadRequest(new ApiResponse<string> { Success = false, Message = "Email address is required" });

                if (request.Accounts == null || request.Accounts.Count == 0)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "At least one account is required" });

                if (request.StartDate > request.EndDate)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Start date cannot be later than end date" });

                var pdfRequest = new PDFRequest
                {
                    Accounts = request.Accounts,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    isCompliance = false,
                    TypeOfSending = "email",
                    //CIF_ID = request.CIF_ID,
                    //AccountToDebit = request.AccountToDebit,
                    //DebitCurrency = request.DebitCurrency
                };

                // Generate PDF
                var pdfResponse = await _pdfService.GenerateStatementsPdf(pdfRequest);

                if (!System.IO.File.Exists(pdfResponse.FilePath))
                {
                    _logger.LogError("PDF file not found at: {FilePath}", pdfResponse.FilePath);
                    return StatusCode(500, new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Failed to generate PDF statement"
                    });
                }

                // Send email
                var senderEmail = _configuration["SenderEmail"] ?? "gh.statement@gtbank.com";

                var emailResult = _emailService.SendIntroHtmlMessage(
                    sendTo: pdfResponse.EmailAddress,
                    sendFrom: senderEmail,
                    sendSubject: $"Account Statement for {pdfResponse.CustomerName} ({request.EndDate:MMM yyyy})",
                    customerName: pdfResponse.CustomerName ?? "Valued Customer",
                    statementPeriod: $"{request.StartDate:dd MMM yyyy} to {request.EndDate:dd MMM yyyy}",
                    attachmentPath: pdfResponse.FilePath,
                    accountNumToEmail: request.Accounts.FirstOrDefault().AccountNumber,
                    productToEmail: "pdf");

                if (System.IO.File.Exists(pdfResponse.FilePath))
                {
                    try { System.IO.File.Delete(pdfResponse.FilePath); } catch { }
                }

                if (emailResult)
                {
                    return Ok(new ApiResponse<string>
                    {
                        Success = true,
                        Message = "Statement emailed successfully",
                        Data = $"Statement sent to {pdfResponse.EmailAddress}"
                    });
                }

                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Failed to send email. Please try again later."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending statement email for CIF: {CIF_ID}", request?.Accounts.FirstOrDefault().AccountNumber);
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get statement data with customer info, opening and closing balances as JSON
        /// </summary>
        /// <param name="request">Statement request with account details</param>
        /// <returns>Statement data with balances</returns>
        [HttpPost("get-statement")]
        [ProducesResponseType(typeof(ApiResponse<StatementDataResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> GetStatementData([FromBody] StatementDataRequest request)
        {
            try
            {
                // Validate request
                if (request == null)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Invalid request data" });

                if (string.IsNullOrEmpty(request.AccountNumber))
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Account number is required" });

                if (request.StartDate > request.EndDate)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Start date cannot be later than end date" });

                // Get account details
                var accountDetails = await _statementClient.GetCustomerAccountDetailsAsync(request.AccountNumber);

                if (accountDetails == null)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"Account {request.AccountNumber} not found"
                    });
                }

                var (statement, openingBalance, closingBalance) = await _statementClient
                    .GetStatementFromFinacleDbAsync(request.AccountNumber, request.StartDate, request.EndDate);

                // Build response
                var response = new StatementDataResponse
                {
                    AccountNumber = request.AccountNumber,
                    CustomerName = accountDetails.AccountName,
                    Email = accountDetails.Email,
                    AccountType = accountDetails.AccountType,
                    Branch = accountDetails.Branch,
                    Address = accountDetails.Address,
                    Currency = accountDetails.AccountCurrency,
                    OpeningBalance = openingBalance,
                    ClosingBalance = closingBalance,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    HasTransactions = statement != null && statement.Rows.Count > 0,
                    TransactionCount = statement?.Rows.Count ?? 0,
                    Transactions = new List<TransactionData>()
                };

                // Add transactions if available
                if (statement != null && statement.Rows.Count > 0)
                {
                    foreach (System.Data.DataRow row in statement.Rows)
                    {
                        var transaction = new TransactionData
                        {
                            TransactionDate = row["TransactionDate"]?.ToString() ?? "",
                            ValueDate = row["TransactionValueDate"]?.ToString() ?? "",
                            Debit = row["Debit"] != DBNull.Value ? Convert.ToDecimal(row["Debit"]) : 0m,
                            Credit = row["Credit"] != DBNull.Value ? Convert.ToDecimal(row["Credit"]) : 0m,
                            Balance = row["Balance"] != DBNull.Value ? Convert.ToDecimal(row["Balance"]) : 0m,
                            Remarks = row["Remarks"]?.ToString()?.Trim() ?? ""
                        };
                        response.Transactions.Add(transaction);
                    }
                }

                // Calculate totals
                response.TotalDebit = response.Transactions.Sum(t => t.Debit);
                response.TotalCredit = response.Transactions.Sum(t => t.Credit);
                response.DebitCount = response.Transactions.Count(t => t.Debit > 0);
                response.CreditCount = response.Transactions.Count(t => t.Credit > 0);

                return Ok(new ApiResponse<StatementDataResponse>
                {
                    Success = true,
                    Message = "Statement data retrieved successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statement data for account: {AccountNumber}", request?.AccountNumber);
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get statement data for multiple accounts
        /// </summary>
        [HttpPost("get-statement-multiple")]
        [ProducesResponseType(typeof(ApiResponse<List<StatementDataResponse>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> GetStatementDataMultiple([FromBody] MultipleStatementDataRequest request)
        {
            try
            {
                if (request == null || request.AccountNumbers == null || request.AccountNumbers.Count == 0)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "At least one account number is required" });

                if (request.StartDate > request.EndDate)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Start date cannot be later than end date" });

                var results = new List<StatementDataResponse>();

                foreach (var accountNumber in request.AccountNumbers)
                {
                    try
                    {
                        var accountDetails = await _statementClient.GetCustomerAccountDetailsAsync(accountNumber);

                        if (accountDetails == null)
                        {
                            _logger.LogWarning("Account not found: {AccountNumber}", accountNumber);
                            continue;
                        }

                        var (statement, openingBalance, closingBalance) = await _statementClient
                            .GetStatementFromFinacleDbAsync(accountNumber, request.StartDate, request.EndDate);

                        var response = new StatementDataResponse
                        {
                            AccountNumber = accountNumber,
                            CustomerName = accountDetails.AccountName,
                            Email = accountDetails.Email,
                            AccountType = accountDetails.AccountType,
                            Branch = accountDetails.Branch,
                            Address = accountDetails.Address,
                            Currency = accountDetails.AccountCurrency,
                            OpeningBalance = openingBalance,
                            ClosingBalance = closingBalance,
                            StartDate = request.StartDate,
                            EndDate = request.EndDate,
                            HasTransactions = statement != null && statement.Rows.Count > 0,
                            TransactionCount = statement?.Rows.Count ?? 0,
                            Transactions = new List<TransactionData>()
                        };

                        if (statement != null && statement.Rows.Count > 0)
                        {
                            foreach (System.Data.DataRow row in statement.Rows)
                            {
                                var transaction = new TransactionData
                                {
                                    TransactionDate = row["TransactionDate"]?.ToString() ?? "",
                                    ValueDate = row["TransactionValueDate"]?.ToString() ?? "",
                                    Debit = row["Debit"] != DBNull.Value ? Convert.ToDecimal(row["Debit"]) : 0m,
                                    Credit = row["Credit"] != DBNull.Value ? Convert.ToDecimal(row["Credit"]) : 0m,
                                    Balance = row["Balance"] != DBNull.Value ? Convert.ToDecimal(row["Balance"]) : 0m,
                                    Remarks = row["Remarks"]?.ToString()?.Trim() ?? ""
                                };
                                response.Transactions.Add(transaction);
                            }

                            response.TotalDebit = response.Transactions.Sum(t => t.Debit);
                            response.TotalCredit = response.Transactions.Sum(t => t.Credit);
                            response.DebitCount = response.Transactions.Count(t => t.Debit > 0);
                            response.CreditCount = response.Transactions.Count(t => t.Credit > 0);
                        }

                        results.Add(response);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing account: {AccountNumber}", accountNumber);
                        // Continue with next account
                    }
                }

                if (results.Count == 0)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "No valid accounts found"
                    });
                }

                return Ok(new ApiResponse<List<StatementDataResponse>>
                {
                    Success = true,
                    Message = $"Statement data retrieved for {results.Count} account(s)",
                    Data = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting multiple statement data");
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                });
            }
        }

    }
}

