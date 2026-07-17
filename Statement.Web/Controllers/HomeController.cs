using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Statement.Web.Helpers;
using Statement.Web.Interfaces;
using Statement.Web.Models;
using System.Data;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Statement.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IBasisFallbackService _basisFallbackService;
        private readonly IGeneratePdfService _pdfService;
        //private readonly IGenerateExcelService _excelService;
        private readonly IStatementClient _statementClient;
        private readonly IMiddlewareClient _middlewareClient;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IBasisFallbackService basisFallbackService, IMiddlewareClient middlewareClient,
            IGeneratePdfService pdfService, IEmailService emailService, IStatementClient statementClient)
        {
            _logger = logger;
            _configuration = configuration;
            _basisFallbackService = basisFallbackService;
            _pdfService = pdfService;
            //_excelService = excelService;
            _emailService = emailService;
            _middlewareClient = middlewareClient;
            _statementClient = statementClient;
        }

        [HttpGet]
        public IActionResult Index()
        {

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

            var result = await SearchCustomerInfoAsync(dto);

            if (result.Count == 0)
                return Json(new { success = false, message = "No accounts found for customer" });

            return Json(new { success = true, data = result });
        }
        public async Task<List<CustomerInfo>> SearchCustomerInfoAsync(SearchDto dto)
        {
            try
            {
                string cif_column = null;
                string accNum_column = null;

                var customers = new List<CustomerInfo>();

                switch (dto.SearchParameter.ToLower())
                {
                    case "cif_id":
                        cif_column = "CIF_ID";
                        break;
                    case "accountnumber":
                        accNum_column = "AccountNumber";
                        break;
                    default:
                        _logger.LogWarning("Invalid search parameter: {SearchParameter}", dto.SearchParameter);
                        return customers;
                }
                if (cif_column != null)
                {
                    var apiAccounts = await _middlewareClient.GetCustomerCIFAccountsAsync(dto.SearchValue);
                    if (apiAccounts != null)
                    {
                        var details = await _statementClient.GetCustomerAccountDetailsAsync(apiAccounts.FirstOrDefault().AccountIdentification);
                        customers = apiAccounts.Select(a => new CustomerInfo
                        {
                            CIF_ID = a.CustomerReference,
                            AccountNumber = a.AccountIdentification,
                            Balance = a.EffectiveAvailableBalance,
                            DisplayName = a.AccountName,
                            AccountCurrency = a.AccountCurrency,
                            BranchCode = int.Parse(a.BranchIdentifier),
                            ProductType = a.ProductType,
                            ProductCode = a.ProductCode,
                            Email = details.Email
                        }).ToList();
                    }
                }
                if (accNum_column != null)
                {
                    if (!Regex.IsMatch(dto.SearchValue, @"^[1-3]\d{3}00[1-5]\d{6,}$"))
                    {
                        dto.SearchValue = await _basisFallbackService.GetFinacleAccountFromBasisAccount(dto.SearchValue);
                    }
                    var customerAccounts = await _middlewareClient.GetAccountsAndCustomersAsync(dto.SearchValue);
                    var acc = customerAccounts.AccountDetails[0];
                    var cust = customerAccounts.CustomerDetails[0];
                    var customerAccount = await _statementClient.GetCustomerAccountDetailsAsync(dto.SearchValue);
  
                    customers.Add(new CustomerInfo
                    {
                        AccountCurrency = acc.AccountCurrency,
                        Balance = double.Parse(acc.AvailableBalanceAmount),
                        AccountNumber = acc.AccountNumber,
                        DisplayName = cust.CustomerName,
                        CIF_ID = cust.CustomerReference,
                        BranchCode = int.Parse(acc.AcctSolId),
                        ProductType = customerAccount.AccountType,
                        ProductCode = acc.Schmcode,
                        Email = customerAccount.Email,
                    });
                }
                return customers;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> GenerateExcelFromData(string StartDate, string EndDate, string CIF_ID, string AccountsJson)
        {
            try
            {
                var accounts = System.Text.Json.JsonSerializer.Deserialize<List<Account>>(AccountsJson);

                if (accounts == null || !accounts.Any())
                    return Json(new { success = false, message = "No accounts provided" });

                var start = DateTime.Parse(StartDate);
                var end = DateTime.Parse(EndDate);

                //var filePath = await _excelService.GenerateStatementsExcel(accounts, CIF_ID, start, end);
                string filePath = null;
                if (!System.IO.File.Exists(filePath))
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
                var accounts = System.Text.Json.JsonSerializer.Deserialize<List<AccountInfo>>(AccountsJson);

                if (accounts == null || !accounts.Any())
                    return Json(new { success = false, message = "No accounts provided" });

                var request = new PDFRequest()
                {
                    Accounts = accounts,
                    StartDate = DateTime.Parse(StartDate),
                    EndDate = DateTime.Parse(EndDate),
                    isCompliance = false,
                    TypeOfSending = "email"
                };
                if (request.StartDate > request.EndDate)
                    return Json(new { success = false, message = "Start date cannot be later than end date" });

                var  pdfResponse = await _pdfService.GenerateStatementsPdf(request);

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

                return emailResult
                    ? Json(new { success = true, message = "Statement emailed successfully" })
                    : Json(new { success = false, message = "Failed to send email" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error emailing statement for CIF {CIF_ID}", CIF_ID);
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> GeneratePdfFromData(string requestJson, bool isAjax = false)
        {
            try
            {
                if (string.IsNullOrEmpty(requestJson))
                    return Json(new { success = false, message = "Invalid request data" });

                var generationRequest = System.Text.Json.JsonSerializer.Deserialize<PDFRequest>(requestJson);

                if (generationRequest.Accounts == null || !generationRequest.Accounts.Any())
                    return Json(new { success = false, message = "Invalid request data", noTransactions = true });

                //var startDate = DateTime.Parse(generationRequest.StartDate);
                //var endDate = DateTime.Parse(generationRequest.EndDate);
                var startDate = generationRequest.StartDate;
                var endDate = generationRequest.EndDate;
                var result = await _pdfService.GenerateStatementsPdf(generationRequest);

                if (!System.IO.File.Exists(result.FilePath))
                    return Json(new { success = false, noTransactions = true, message = result.Message ?? "No transactions found" });

                var fileBytes = await System.IO.File.ReadAllBytesAsync(result.FilePath);
                System.IO.File.Delete(result.FilePath);

                return File(fileBytes, "application/pdf",
                    $"Statement_{generationRequest.CIF_ID}_{endDate:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF");
                return Json(new { success = false, message = "Error generating PDF" });
            }
        }
    }
}