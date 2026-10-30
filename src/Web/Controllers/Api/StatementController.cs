using Microsoft.AspNetCore.Mvc;
using Statement.Web.Filters;
using Statement.Web.Helpers;
using Statement.Web.Interfaces;
using Statement.Web.Models;

namespace Statement.Web.Controllers.Api
{
    [ApiController]
    //[Route("api/v1/[controller]")]
    [Route("api/v1/accounts")]
    [ApiKeyAuth]
    public class StatementController : ControllerBase
    {
        private readonly ILogger<StatementController> _logger;
        private readonly IGeneratePdfService _pdfService;
        private readonly IGenerateExcelService _excelService;
        private readonly IEmailService _emailService;
        private readonly IStatementClient _statementClient;
        private readonly IMiddlewareClient _middlewareClient;
        private readonly IConfiguration _configuration;

        public StatementController(ILogger<StatementController> logger, IGeneratePdfService pdfService,
            IGenerateExcelService excelService, IEmailService emailService, IStatementClient statementClient,IMiddlewareClient middlewareClient, IConfiguration configuration)
        {
            _logger = logger;
            _pdfService = pdfService;
            _excelService = excelService;
            _emailService = emailService;
            _statementClient = statementClient;
            _middlewareClient = middlewareClient;
            _configuration = configuration;
        }


        /// <summary>
        /// Get statement in requested format (PDF, Excel, or JSON)
        /// </summary>
        /// <param name="accountNumber">The account number (e.g., 101001234567)</param>
        /// <param name="startDate">Start date in ISO format (yyyy-MM-dd) - e.g., 2024-01-01</param>
        /// <param name="endDate">End date in ISO format (yyyy-MM-dd) - e.g., 2024-01-31</param>
        /// <param name="format">Output format: pdf, excel, or json (default: pdf)</param>
        /// <param name="debitCurrency">Currency code (default: GHS)</param>
        /// <returns>Statement in requested format</returns>
        /// <response code="200">Returns the statement file or JSON data</response>
        /// <response code="400">Invalid request parameters</response>
        /// <response code="404">Account not found</response>
        /// <response code="500">Server error</response>
        /// <example>
        /// GET /api/accounts/101001234567/statements?startDate=2024-01-01&endDate=2024-01-31&format=pdf
        /// </example>
        [HttpGet("{accountNumber}/statements")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> GetStatement(string accountNumber, 
        [FromQuery][DateFormat("2026-01-01", "Start date in ISO format (yyyy-MM-dd)")] DateTime startDate,
        [DateFormat("2026-01-31", "Start date in ISO format (yyyy-MM-dd)")][FromQuery] DateTime endDate, 
        [FromQuery] string format = "pdf", [FromQuery] string debitCurrency = "GHS")
        {
            try
            {
                if (startDate > endDate)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Start date cannot be later than end date" });

                var (accounts, cifId) = await GetAccountsAndCifFromAccount(accountNumber);
                if (accounts == null || accounts.Count == 0)
                    return NotFound(new ApiResponse<string> { Success = false, Message = "No accounts found" });

                // Handle JSON format
                if (format?.ToLower() == "json")
                {
                    var data = await GetSingleAccountStatementData(accountNumber, startDate, endDate);
                    if (data == null)
                        return NotFound(new ApiResponse<string> { Success = false, Message = "No statement data found" });

                    return Ok(new ApiResponse<SingleAccountStatementData>
                    {
                        Success = true,
                        Message = "Statement retrieved successfully",
                        Data = data
                    });
                }

                // Handle PDF or Excel
                if (format?.ToLower() == "pdf")
                {
                    var pdfRequest = new PDFRequest
                    {
                        Accounts = accounts,
                        StartDate = startDate,
                        EndDate = endDate,
                        TypeOfSending = "download",
                        CIF_ID = cifId,
                        AccountToDebit = accountNumber,
                        DebitCurrency = debitCurrency
                    };

                    var pdfResponse = await _pdfService.GenerateStatementsPdf(pdfRequest);

                    if (!System.IO.File.Exists(pdfResponse.FilePath))
                        return StatusCode(500, new ApiResponse<string> { Success = false, Message = "Failed to generate PDF" });

                    var fileBytes = await System.IO.File.ReadAllBytesAsync(pdfResponse.FilePath);
                    System.IO.File.Delete(pdfResponse.FilePath);

                    return File(fileBytes, "application/pdf", $"Statement_{cifId}_{endDate:yyyyMMdd}.pdf");
                }

                if (format?.ToLower() == "excel")
                {
                    var filePath = await _excelService.GenerateStatementsExcelAsync(accounts, cifId, startDate, endDate);

                    if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                        return StatusCode(500, new ApiResponse<string> { Success = false, Message = "Failed to generate Excel" });

                    var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                    System.IO.File.Delete(filePath);

                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Statement_{cifId}_{endDate:yyyyMMdd}.xlsx");
                }

                return BadRequest(new ApiResponse<string> { Success = false, Message = "Invalid format. Use pdf, excel, or json" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statement for account: {AccountNumber}", accountNumber);
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// Get statement as PDF
        /// </summary>
        [HttpGet("{accountNumber}/statements/pdf")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> GetStatementPdf(string accountNumber, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string debitCurrency = "GHS")
        {
            return await GetStatement(accountNumber, startDate, endDate, "pdf", debitCurrency);
        }

        /// <summary>
        /// Get statement as Excel
        /// </summary>
        [HttpGet("{accountNumber}/statements/excel")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> GetStatementExcel(string accountNumber,[FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] bool isCompliance = false, [FromQuery] string debitCurrency = "GHS")
        {
            return await GetStatement(accountNumber, startDate, endDate, "excel", debitCurrency);
        }

        /// <summary>
        /// Get statement as JSON
        /// </summary>
        [HttpGet("{accountNumber}/statements/json")]
        [ProducesResponseType(typeof(ApiResponse<SingleAccountStatementData>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> GetStatementJson(string accountNumber, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Start date cannot be later than end date" });

                var data = await GetSingleAccountStatementData(accountNumber, startDate, endDate);
                if (data == null)
                    return NotFound(new ApiResponse<string> { Success = false, Message = "No statement data found" });

                return Ok(new ApiResponse<SingleAccountStatementData>
                {
                    Success = true,
                    Message = "Statement retrieved successfully",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting JSON for account: {AccountNumber}", accountNumber);
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = ex.Message });
            }
        }
        /// <summary>
        /// Get statement as JSON (all accounts)
        /// </summary>
        [HttpGet("{accountNumber}/statements/all/json")]
        [ProducesResponseType(typeof(ApiResponse<StatementDataResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> GetJsonAllAccounts([FromBody] StatementRequest request)
        {
            try
            {
                var (accounts, cifId) = await GetAccountsAndCifFromAccount(request.AccountNumber);
                if (accounts == null || accounts.Count == 0)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "No accounts found" });

                var results = new List<SingleAccountStatementData>();

                foreach (var account in accounts)
                {
                    var data = await GetSingleAccountStatementData(account.AccountNumber, request.StartDate, request.EndDate);
                    if (data != null)
                        results.Add(data);
                }

                return Ok(new ApiResponse<List<SingleAccountStatementData>>
                {
                    Success = true,
                    Message = $"Statement data retrieved for {results.Count} account(s)",
                    Data = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting JSON for account: {AccountNumber}", request?.AccountNumber);
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// Email statement as PDF
        /// </summary>
        [HttpPost("{accountNumber}/statements/email/pdf")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> EmailStatementPdf(string accountNumber, [FromBody] StatementEmailRequest request)
        {
            try
            {
                if (request.StartDate > request.EndDate)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Start date cannot be later than end date" });

                var (accounts, cifId) = await GetAccountsAndCifFromAccount(accountNumber);
                if (accounts == null || accounts.Count == 0)
                    return NotFound(new ApiResponse<string> { Success = false, Message = "No accounts found" });

                var email = request.Email ?? await GetCustomerEmailAsync(cifId);
                if (string.IsNullOrEmpty(email))
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Email not provided and could not be fetched" });

                var pdfRequest = new PDFRequest
                {
                    Accounts = accounts,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    isCompliance = false,
                    TypeOfSending = "email",
                    CIF_ID = cifId,
                    AccountToDebit = accountNumber,
                    DebitCurrency = "GHS"
                };

                var pdfResponse = await _pdfService.GenerateStatementsPdf(pdfRequest);

                if (!System.IO.File.Exists(pdfResponse.FilePath))
                    return StatusCode(500, new ApiResponse<string> { Success = false, Message = "Failed to generate PDF" });

                var senderEmail = _configuration["SenderEmail"] ?? "gh.statement@gtbank.com";
                var (accNumber, accName, product) = GetPrioritizedAccount(accounts);

                var emailResult = _emailService.SendIntroHtmlMessage(
                    sendTo: email,
                    sendFrom: senderEmail,
                    sendSubject: $"Account Statement for {pdfResponse.CustomerName} ({request.EndDate:MMM yyyy})",
                    customerName: pdfResponse.CustomerName ?? "Valued Customer",
                    statementPeriod: $"{request.StartDate:dd MMM yyyy} to {request.EndDate:dd MMM yyyy}",
                    attachmentPath: pdfResponse.FilePath,
                    accountNumToEmail: accNumber,
                    productToEmail: product);

                if (System.IO.File.Exists(pdfResponse.FilePath))
                    try { System.IO.File.Delete(pdfResponse.FilePath); } catch { }

                if (emailResult)
                    return Ok(new ApiResponse<string> { Success = true, Message = "PDF emailed successfully", Data = email });

                return StatusCode(500, new ApiResponse<string> { Success = false, Message = "Failed to send email" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error emailing PDF for account: {AccountNumber}", accountNumber);
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// Email statement as Excel
        /// </summary>
        [HttpPost("{accountNumber}/statements/email/excel")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<IActionResult> EmailStatementExcel(string accountNumber,[FromBody] StatementEmailRequest request)
        {
            try
            {
                if (request.StartDate > request.EndDate)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Start date cannot be later than end date" });

                var (accounts, cifId) = await GetAccountsAndCifFromAccount(accountNumber);
                if (accounts == null || accounts.Count == 0)
                    return NotFound(new ApiResponse<string> { Success = false, Message = "No accounts found" });

                var email = request.Email ?? await GetCustomerEmailAsync(cifId);
                if (string.IsNullOrEmpty(email))
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Email not provided and could not be fetched" });

                var filePath = await _excelService.GenerateStatementsExcelAsync(accounts, cifId, request.StartDate, request.EndDate, email);

                if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                    return StatusCode(500, new ApiResponse<string> { Success = false, Message = "Failed to generate Excel" });

                var senderEmail = _configuration["SenderEmail"] ?? "gh.statement@gtbank.com";
                var (accNumber, accName, product) = GetPrioritizedAccount(accounts);

                var emailResult = _emailService.SendIntroHtmlMessage(
                    sendTo: email,
                    sendFrom: senderEmail,
                    sendSubject: $"Account Statement (Excel) for {accName} ({request.EndDate:MMM yyyy})",
                    customerName: accName ?? "Valued Customer",
                    statementPeriod: $"{request.StartDate:dd MMM yyyy} to {request.EndDate:dd MMM yyyy}",
                    attachmentPath: filePath,
                    accountNumToEmail: accNumber,
                    productToEmail: product);

                if (System.IO.File.Exists(filePath))
                    try { System.IO.File.Delete(filePath); } catch { }

                if (emailResult)
                    return Ok(new ApiResponse<string> { Success = true, Message = "Excel emailed successfully", Data = email });

                return StatusCode(500, new ApiResponse<string> { Success = false, Message = "Failed to send email" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error emailing Excel for account: {AccountNumber}", accountNumber);
                return StatusCode(500, new ApiResponse<string> { Success = false, Message = ex.Message });
            }
        }


        #region Helper Methods
        private async Task<(List<AccountInfo> accounts, string cifId)> GetAccountsAndCifFromAccount(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber))
                throw new ArgumentException("Account number is required");

            var result = await _middlewareClient.GetAccountsAndCustomersAsync(accountNumber);
            if (result.AccountDetails == null || result.CustomerDetails == null)
                throw new Exception("Account not found");

            var allAccounts = await _middlewareClient.GetCustomerCIFAccountsAsync(result.CustomerDetails.FirstOrDefault().CustomerReference);
            if (allAccounts == null || !allAccounts.Any())
                throw new Exception("No accounts found for this customer");

            var accounts = allAccounts
                .Where(a => a != null)
                .Select(a => new AccountInfo
                {
                    AccountNumber = a.AccountIdentification,
                    AccountName = a.AccountName,
                    Currency = a.AccountCurrency,
                    Balance = Convert.ToDecimal(a.EffectiveAvailableBalance),
                    BranchCode = int.TryParse(a.BranchIdentifier, out int bCode) ? bCode : 201,
                    ProductCode = a.ProductCode
                }).ToList();

            return (accounts, result.CustomerDetails.FirstOrDefault().CustomerReference);
        }
        private async Task<SingleAccountStatementData> GetSingleAccountStatementData(string accountNumber, DateTime startDate, DateTime endDate)
        {
            try
            {
                var details = await _statementClient.GetCustomerAccountDetailsAsync(accountNumber);
                if (details == null)
                    return null;

                var (statement, openingBalance, closingBalance) = await _statementClient
                    .GetStatementFromFinacleDbAsync(accountNumber, startDate, endDate);

                var data = new SingleAccountStatementData
                {
                    AccountNumber = accountNumber,
                    CustomerName = details.AccountName,
                    Email = details.Email,
                    AccountType = details.AccountType,
                    Branch = details.Branch,
                    Address = details.Address,
                    Currency = details.AccountCurrency,
                    OpeningBalance = openingBalance,
                    ClosingBalance = closingBalance,
                    StartDate = startDate,
                    EndDate = endDate,
                    HasTransactions = statement != null && statement.Rows.Count > 0,
                    TransactionCount = statement?.Rows.Count ?? 0,
                    Transactions = new List<StatementRecord>()
                };

                if (statement != null && statement.Rows.Count > 0)
                {
                    decimal debitSum = 0, creditSum = 0;
                    int debitCount = 0, creditCount = 0;

                    foreach (System.Data.DataRow row in statement.Rows)
                    {
                        var debit = row["Debit"] != DBNull.Value ? Convert.ToDecimal(row["Debit"]) : 0m;
                        var credit = row["Credit"] != DBNull.Value ? Convert.ToDecimal(row["Credit"]) : 0m;
                        var balance = row["Balance"] != DBNull.Value ? Convert.ToDecimal(row["Balance"]) : 0m;

                        if (debit > 0) { debitSum += debit; debitCount++; }
                        if (credit > 0) { creditSum += credit; creditCount++; }

                        data.Transactions.Add(new StatementRecord
                        {
                            TransactionDate = row["TransactionDate"]?.ToString() ?? "",
                            TransactionValueDate = row["TransactionValueDate"]?.ToString() ?? "",
                            Debit = debit,
                            Credit = credit,
                            Balance = balance,
                            Remarks = row["Remarks"]?.ToString()?.Trim() ?? ""
                        });
                    }

                    data.TotalDebit = debitSum;
                    data.TotalCredit = creditSum;
                    data.DebitCount = debitCount;
                    data.CreditCount = creditCount;
                }

                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statement data for account: {AccountNumber}", accountNumber);
                return null;
            }
        }
        private async Task<string> GetCustomerEmailAsync(string cifId)
        {
            try
            {
                var accounts = await _middlewareClient.GetCustomerCIFAccountsAsync(cifId);
                if (accounts != null && accounts.Any())
                {
                    var firstAccount = accounts.FirstOrDefault(a => a != null);
                    if (firstAccount != null && !string.IsNullOrEmpty(firstAccount.AccountIdentification))
                    {
                        var details = await _statementClient.GetCustomerAccountDetailsAsync(firstAccount.AccountIdentification);
                        return details?.Email;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching email for CIF: {CIF_ID}", cifId);
                return null;
            }
        }
        private (string accountNumber, string accountName, string product) GetPrioritizedAccount(List<AccountInfo> accounts)
        {
            if (accounts == null || accounts.Count == 0)
                return ("", "", "");
            var prioritized = accounts.FirstOrDefault(a => !string.IsNullOrEmpty(a.AccountNumber));
            return (prioritized?.AccountNumber ?? "", prioritized?.AccountName ?? "", prioritized?.ProductCode ?? "");
        }
        #endregion
    }
}