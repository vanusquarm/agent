using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Statement.Web.Helpers;
using Statement.Web.Interfaces;
using Statement.Web.Models;
using System.Data;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Statement.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IBasisFallbackService _basisFallbackService;
        private readonly IGeneratePdfService _pdfService;
        private readonly IGenerateExcelService _excelService;
        private readonly IStatementClient _statementClient;
        private readonly IMiddlewareClient _middlewareClient;
        private string _pendingTempFilePath;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IBasisFallbackService basisFallbackService, IMiddlewareClient middlewareClient,
            IGeneratePdfService pdfService, IGenerateExcelService excelService, IEmailService emailService, IStatementClient statementClient)
        {
            _logger = logger;
            _configuration = configuration;
            _basisFallbackService = basisFallbackService;
            _pdfService = pdfService;
            _excelService = excelService;
            _emailService = emailService;
            _middlewareClient = middlewareClient;
            _statementClient = statementClient;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var welcomeMessage = User.FindFirstValue("WelcomeMessage");

            if (string.IsNullOrEmpty(welcomeMessage))
            {
                welcomeMessage = HttpContext.Session.GetString("WelcomeMessage");
            }

            if (string.IsNullOrEmpty(welcomeMessage))
            {
                var userName = User.Identity?.Name ?? HttpContext.Session.GetString("user_name");
                if (!string.IsNullOrEmpty(userName))
                {
                    welcomeMessage = $"Welcome back, {userName}!";
                }
                else
                {
                    welcomeMessage = "Welcome to GT Bank Statement Portal!";
                }

                HttpContext.Session.SetString("WelcomeMessage", welcomeMessage);
            }

            ViewBag.WelcomeMessage = welcomeMessage;
            ViewBag.UserName = User.Identity?.Name ?? HttpContext.Session.GetString("user_name");

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Search(SearchDto dto)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid search parameters" });

            if (dto.StartDate > dto.EndDate)
                return Json(new { success = false, message = "Start date cannot be later than end date" });

            try
            {
                var result = await SearchCustomerInfoAsync(dto);

                if (result == null || result.Count == 0)
                    return Json(new { success = false, message = "No accounts found for customer" });

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during search execution for value: {SearchValue}", dto?.SearchValue);
                return Json(new { success = false, message = "An internal server error occurred while retrieving details." });
            }
        }

        public async Task<List<CustomerInfo>> SearchCustomerInfoAsync(SearchDto dto)
        {
            var customers = new List<CustomerInfo>();

            if (dto == null || string.IsNullOrWhiteSpace(dto.SearchParameter) || string.IsNullOrWhiteSpace(dto.SearchValue))
            {
                _logger.LogWarning("Search attempted with null or empty DTO parameters.");
                return customers;
            }

            try
            {
                string parameter = dto.SearchParameter.ToLower().Trim();

                // --- CIF ID SEARCH PATHWAY ---
                if (parameter == "cif_id")
                {
                    var apiAccounts = await _middlewareClient.GetCustomerCIFAccountsAsync(dto.SearchValue);

                    if (apiAccounts != null && apiAccounts.Any())
                    {
                        var firstAccount = apiAccounts.FirstOrDefault(a => a != null);
                        string emailAddress = "—";

                        if (firstAccount != null && !string.IsNullOrEmpty(firstAccount.AccountIdentification))
                        {
                            var details = await _statementClient.GetCustomerAccountDetailsAsync(firstAccount.AccountIdentification);
                            emailAddress = details?.Email ?? "—";
                        }

                        customers = apiAccounts
                            .Where(a => a != null)
                            .Select(a => new CustomerInfo
                            {
                                CIF_ID = a.CustomerReference,
                                AccountNumber = a.AccountIdentification,
                                Balance = a.EffectiveAvailableBalance,
                                DisplayName = a.AccountName,
                                AccountCurrency = a.AccountCurrency,
                                BranchCode = int.TryParse(a.BranchIdentifier, out int bCode) ? bCode : 0,
                                ProductType = a.ProductType,
                                ProductCode = a.ProductCode,
                                Email = emailAddress
                            }).ToList();
                    }
                }
                // --- ACCOUNT NUMBER SEARCH PATHWAY ---
                else if (parameter == "accountnumber")
                {
                    if (!Regex.IsMatch(dto.SearchValue, @"^[1-3]\d{3}00[1-5]\d{6,}$"))
                    {
                        dto.SearchValue = await _basisFallbackService.GetFinacleAccountFromBasisAccount(dto.SearchValue);
                    }

                    var customerAccounts = await _middlewareClient.GetAccountsAndCustomersAsync(dto.SearchValue);

                    if (customerAccounts?.AccountDetails != null && customerAccounts.AccountDetails.Any() &&
                        customerAccounts.CustomerDetails != null && customerAccounts.CustomerDetails.Any())
                    {
                        var acc = customerAccounts.AccountDetails[0];
                        var cust = customerAccounts.CustomerDetails[0];

                        var customerAccount = await _statementClient.GetCustomerAccountDetailsAsync(dto.SearchValue);

                        customers.Add(new CustomerInfo
                        {
                            AccountCurrency = acc?.AccountCurrency,
                            Balance = double.TryParse(acc?.AvailableBalanceAmount, out double balValue) ? balValue : 0.0,
                            AccountNumber = acc?.AccountNumber,
                            DisplayName = cust?.CustomerName,
                            CIF_ID = cust?.CustomerReference,
                            BranchCode = int.TryParse(acc?.AcctSolId, out int solId) ? solId : 0,
                            ProductType = customerAccount?.AccountType,
                            ProductCode = acc?.Schmcode,
                            Email = customerAccount?.Email ?? "—",
                        });
                    }
                    else
                    {
                        _logger.LogWarning("Middleware returned incomplete data structures for account search value: {SearchValue}", dto.SearchValue);
                    }
                }
                else
                {
                    _logger.LogWarning("Invalid search parameter: {SearchParameter}", dto.SearchParameter);
                }

                return customers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception caught inside SearchCustomerInfoAsync processing.");
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> GenerateExcelFromData(string StartDate, string EndDate, string CIF_ID, string AccountsJson)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };

                var accounts = System.Text.Json.JsonSerializer.Deserialize<List<AccountInfo>>(AccountsJson, options);

                if (accounts == null || !accounts.Any())
                    return Json(new { success = false, message = "No accounts provided" });

                var start = DateTime.Parse(StartDate);
                var end = DateTime.Parse(EndDate);

                var filePath = await _excelService.GenerateStatementsExcelAsync(accounts, CIF_ID, start, end);

                if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                    return Json(new { success = false, message = "Failed to generate Excel file" });

                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                System.IO.File.Delete(filePath);

                return File(fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Statement_{CIF_ID}_{end:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Excel for CIF {CIF_ID}", CIF_ID);
                return StatusCode(500, new { success = false, message = "Internal error generating Excel" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> EmailStatement(string StartDate, string EndDate, string CIF_ID, string Email, string AccountsJson)
        {
            try
            {
                // Allow reading numbers from strings
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };

                var accounts = System.Text.Json.JsonSerializer.Deserialize<List<AccountInfo>>(AccountsJson, options);

                if (accounts == null || !accounts.Any())
                    return Json(new { success = false, message = "No accounts provided" });

                var request = new PDFRequest()
                {
                    Accounts = accounts,
                    StartDate = DateTime.Parse(StartDate),
                    EndDate = DateTime.Parse(EndDate),
                    isCompliance = false,
                    TypeOfSending = "email",
                    CIF_ID = CIF_ID
                };

                if (request.StartDate > request.EndDate)
                    return Json(new { success = false, message = "Start date cannot be later than end date" });

                var pdfResponse = await _pdfService.GenerateStatementsPdf(request);

                if (!System.IO.File.Exists(pdfResponse.FilePath))
                    return Json(new { success = false, message = "Failed to generate PDF for email" });

                var senderEmail = _configuration["SenderEmail"] ?? "gh.statement@gtbank.com";

                var (accountNumber, accountName, product) = Utility.GetPrioritizedAccountWithDescription(accounts);

                var emailResult = _emailService.SendIntroHtmlMessage(
                    sendTo: Email,
                    sendFrom: senderEmail,
                    sendSubject: $"Account Statement for {pdfResponse.CustomerName} ({request.EndDate:MMM yyyy})",
                    customerName: pdfResponse.CustomerName ?? "Valued Customer",
                    statementPeriod: $"{request.StartDate:dd MMM yyyy} to {request.EndDate:dd MMM yyyy}",
                    attachmentPath: pdfResponse.FilePath,
                    accountNumToEmail: accountNumber,
                    productToEmail: product);

                // Clean up the temp file after email
                if (System.IO.File.Exists(pdfResponse.FilePath))
                {
                    try { System.IO.File.Delete(pdfResponse.FilePath); } catch { }
                }

                return emailResult
                    ? Json(new { success = true, message = "Statement emailed successfully" })
                    : Json(new { success = false, message = "Failed to send email" });
            }
            catch (System.Text.Json.JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON deserialization error in EmailStatement: {AccountsJson}", AccountsJson);
                return Json(new { success = false, message = "Invalid account data format: " + jsonEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error emailing statement for CIF {CIF_ID}", CIF_ID);
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> GeneratePdfFromData(string requestJson, bool isAjax = false, string tempFilePath = null)
        {
            try
            {
                if (string.IsNullOrEmpty(requestJson))
                    return Json(new { success = false, message = "Invalid request data" });

                // Use custom converter for AccountInfo
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new AccountInfoConverter() }
                };

                var generationRequest = System.Text.Json.JsonSerializer.Deserialize<PDFRequest>(requestJson, options);

                if (generationRequest?.Accounts == null || !generationRequest.Accounts.Any())
                    return Json(new { success = false, message = "Invalid request data", noTransactions = true });

                // ============ CRITICAL: Clean up preview temp file if it exists ============
                if (!string.IsNullOrEmpty(tempFilePath) && System.IO.File.Exists(tempFilePath))
                {
                    try
                    {
                        // Delete the preview temp file - we won't use it
                        System.IO.File.Delete(tempFilePath);
                        _logger.LogInformation($"Deleted preview temp file: {tempFilePath}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Failed to delete preview temp file: {tempFilePath}");
                    }
                }

                generationRequest.TypeOfSending = "download";
                var result = await _pdfService.GenerateStatementsPdf(generationRequest);

                if (!System.IO.File.Exists(result.FilePath))
                {
                    return Json(new
                    {
                        success = false,
                        noTransactions = true,
                        message = result.Message ?? "No transactions found"
                    });
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(result.FilePath);

                int pageCount = 0;
                using (var reader = new PdfReader(result.FilePath))
                {
                    pageCount = reader.NumberOfPages;
                }

                System.IO.File.Delete(result.FilePath);

                if (isAjax)
                {
                    Response.Headers.Add("X-Pdf-Message", result.Message ?? "PDF statement generated successfully.");
                    Response.Headers.Add("X-Pdf-Amount", result.Amount ?? "0.00");
                    Response.Headers.Add("X-Pdf-Pages", pageCount.ToString());
                    Response.Headers.Add("X-Pdf-Accounts", generationRequest.Accounts.Count.ToString());
                }

                string fileName = $"Statement_{generationRequest.CIF_ID}_{generationRequest.EndDate:yyyyMMdd}.pdf";
                return File(fileBytes, "application/pdf", fileName);
            }
            catch (System.Text.Json.JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON deserialization error: {RequestJson}", requestJson);
                return Json(new { success = false, message = "Invalid data format: " + jsonEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF");
                return Json(new { success = false, message = "Error generating PDF: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> PreviewPdfCharge([FromBody] PreviewChargeRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Request))
                    return Json(new { success = false, message = "Invalid request data" });

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new AccountInfoConverter() }
                };

                var pdfRequest = System.Text.Json.JsonSerializer.Deserialize<PDFRequest>(request.Request, options);

                if (pdfRequest?.Accounts == null || !pdfRequest.Accounts.Any())
                    return Json(new { success = false, message = "No accounts provided" });

                pdfRequest.TypeOfSending = "preview";
                var previewResult = await _pdfService.GeneratePreviewPdf(pdfRequest);

                return Json(new
                {
                    success = true,
                    chargeAmount = previewResult.ChargeAmount.ToString("N2"),
                    pageCount = previewResult.PageCount,
                    accountCount = pdfRequest.Accounts.Count,
                    period = $"{pdfRequest.StartDate:dd MMM yyyy} to {pdfRequest.EndDate:dd MMM yyyy}",
                    tempFilePath = previewResult.TempFilePath, // Store for later use if user proceeds
                    hasTransactions = previewResult.HasTransactions
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing PDF charge");
                return Json(new { success = false, message = "Error calculating charges: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> CleanupTempFile([FromForm] string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                {
                    await Task.Run(() => System.IO.File.Delete(filePath));
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "File not found" });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to delete temp file: {filePath}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}