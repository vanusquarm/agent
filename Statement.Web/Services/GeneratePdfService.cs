using iTextSharp.text;
using iTextSharp.text.pdf;
using Oracle.ManagedDataAccess.Client;
using Statement.Web.Helpers;
using Statement.Web.Interfaces;
using Statement.Web.Models;
using System.Data;
using System.Globalization;
namespace Statement.Web.Services
{
    public class GeneratePdfService : IGeneratePdfService
    {
        private readonly ILogger<GeneratePdfService> _logger;
        private readonly string _connectionString;
        private readonly IMiddlewareClient _finacleApiClient;
        private readonly IStatementClient _statementClient;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emaiilService;
        private readonly PdfStatementHelper _pdfHelper;

        private readonly string _SenderEmail;
        private readonly string _NotifierCopyEmail;
        private readonly string _NotifierToEmail;
        public GeneratePdfService(ILogger<GeneratePdfService> logger, IMiddlewareClient finacleApiClient, IConfiguration configuration, IWebHostEnvironment environment, IStatementClient statementClient, IEmailService emaiilService)
        {
            _logger = logger ?? throw new ArgumentNullException();
            _finacleApiClient = finacleApiClient ?? throw new ArgumentNullException(nameof(finacleApiClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _connectionString = configuration.GetConnectionString("FinacleConn") ?? throw new InvalidOperationException("Database connection string is not configured.");
            _environment = environment;
            _statementClient = statementClient;

            _pdfHelper = new PdfStatementHelper(logger, statementClient);
            _SenderEmail = configuration["SenderEmail"];
            _NotifierCopyEmail = configuration["NotifierCopyEmail"];
            _NotifierToEmail = configuration["NotifierToEmail"];
            _emaiilService = emaiilService;
        }

        public async Task<PDFResponse> GenerateStatementsPdf(PDFRequest request, string outputFile = null)
        {
            string acct_name = string.Empty;
            string email = string.Empty;
            bool noTransactions = false;
            string branchCode = request.Accounts.FirstOrDefault()?.BranchCode.ToString() ?? "201";

            try
            {
                if (request.Accounts == null || request.Accounts.Count == 0)
                {
                    _logger.LogWarning($"No accounts found for CIF_ID: {request.CIF_ID}");
                    return new PDFResponse();
                }

                // If no outputFile provided, generate one
                if (string.IsNullOrEmpty(outputFile))
                {
                    var tempPath = Path.Combine(Path.GetTempPath(), "GTStatements", "Temp");
                    Directory.CreateDirectory(tempPath);

                    string month = request.EndDate.ToString("MMM", CultureInfo.CreateSpecificCulture("en-GB"));
                    string fileName = $"GTBANK_{month}_{request.Accounts.First().AccountNumber}_Stmt_{Guid.NewGuid()}.pdf";
                    outputFile = Path.Combine(tempPath, fileName);
                }

                // Generate the PDF document
                Document document = new Document(PageSize.A4, 10, 10, 10, 40);
                using (FileStream fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);
                    writer.PageEvent = new CustomFooterEvent();
                    document.Open();

                    // Add logo
                    string imageFilePath = Path.Combine(_environment.ContentRootPath, "Resources", "glogo.jpg");
                    if (File.Exists(imageFilePath))
                    {
                        Image jpg = Image.GetInstance(imageFilePath);
                        jpg.ScaleToFit(50f, 50f);
                        jpg.Alignment = Element.ALIGN_RIGHT;
                        jpg.IndentationRight = 30;
                        document.Add(jpg);
                    }

                    bool hasActualData = false;
                    var semaphore = new SemaphoreSlim(2);
                    int accountIndex = 0;

                    foreach (var account in request.Accounts)
                    {
                        await semaphore.WaitAsync();
                        try
                        {
                            string accountIdentification = account.AccountNumber;
                            string accountCurrency = account.Currency;
                            acct_name = account.AccountName;

                            // Add new page for subsequent accounts
                            if (accountIndex > 0)
                            {
                                document.NewPage();
                                if (File.Exists(imageFilePath))
                                {
                                    Image jpg = Image.GetInstance(imageFilePath);
                                    jpg.ScaleToFit(50f, 50f);
                                    jpg.SpacingAfter = 1f;
                                    jpg.Alignment = Element.ALIGN_RIGHT;
                                    jpg.IndentationRight = 30;
                                    document.Add(jpg);
                                }
                            }

                            if (string.IsNullOrEmpty(accountIdentification))
                            {
                                _logger.LogWarning($"Invalid accountIdentification for CIF_ID: {request.CIF_ID}");
                                continue;
                            }

                            // Get account details
                            var details = await _statementClient.GetCustomerAccountDetailsAsync(accountIdentification);

                            // Build transaction table using the helper
                            var (datatable, hasTrans, openingBalance, closingBalance, debitSum, creditSum, debitCount, creditCount)
                                = await _pdfHelper.BuildTransactionTableAsync(accountIdentification, request.StartDate, request.EndDate);

                            noTransactions = !hasTrans;
                            if (hasTrans) hasActualData = true;

                            // Build the header table
                            var headerRow = _pdfHelper.BuildAccountHeaderTable(
                                accountIdentification, accountCurrency, openingBalance,
                                closingBalance, details, branchCode);

                            // Add elements to document
                            AddStatementElements(document, acct_name, accountIdentification,
                                request.StartDate, request.EndDate, headerRow, datatable);

                            accountIndex++;
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }

                    document.Close();
                }

                // Prepare response
                var response = new PDFResponse
                {
                    FilePath = outputFile,
                    CustomerName = acct_name,
                    EmailAddress = email,
                    HasTransactions = !noTransactions,
                    Success = true
                };

                // ============ TYPE OF SENDING HANDLING ============

                // For PREVIEW - just count pages, calculate estimate, no actual charges
                if (request.TypeOfSending == "preview")
                {
                    int pageCount = GetPdfPageCount(outputFile);
                    response.PageCount = pageCount;

                    // Calculate estimated charge (without calling Finacle API)
                    decimal chargePerPage = _configuration.GetValue<decimal>("StatementCharge:PerPage", 2.50m);
                    decimal minimumCharge = _configuration.GetValue<decimal>("StatementCharge:Minimum", 5.00m);
                    decimal maxCharge = _configuration.GetValue<decimal>("StatementCharge:Maximum", 100.00m);

                    decimal estimatedCharge = Math.Max(pageCount * chargePerPage, minimumCharge);
                    estimatedCharge = Math.Min(estimatedCharge, maxCharge);

                    response.Amount = estimatedCharge.ToString("N2");
                    response.Message = $"Preview: Estimated charge {estimatedCharge:N2} for {pageCount} page(s)";

                    // Process preview (no actual charging)
                    return await ProcessDownloadAsync(response, request, branchCode, isPreview: true);
                }

                // For DOWNLOAD - actual charge processing
                if (request.TypeOfSending.Equals("download", StringComparison.OrdinalIgnoreCase))
                {
                    // Get page count
                    int pageCount = GetPdfPageCount(outputFile);
                    response.PageCount = pageCount;

                    // Process actual charges
                    return await ProcessDownloadAsync(response, request, branchCode, isPreview: false);
                }

                // For EMAIL - no charges
                if (request.TypeOfSending.Equals("email", StringComparison.OrdinalIgnoreCase))
                {
                    response.Amount = "0.00";
                    response.Message = "PDF generated successfully for email.";
                    return response;
                }

                // Default fallback - no charges
                response.Amount = "0.00";
                response.Message = "PDF generated successfully.";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating PDF for CIF_ID: {request.CIF_ID}: {ex.Message}", ex);
                if (!string.IsNullOrEmpty(outputFile) && File.Exists(outputFile))
                {
                    try { File.Delete(outputFile); } catch { }
                }
                _emaiilService.SendMessage(
                    _NotifierToEmail,
                    _NotifierCopyEmail,
                    _SenderEmail,
                    $"Error Generating Statement for: {request.CIF_ID}",
                    $"An Error Occurred: {ex.Message}");
                return new PDFResponse();
            }
        }
        public async Task<PdfChargePreview> GeneratePreviewPdf(PDFRequest request)
        {
            string tempFilePath = null;
            try
            {
                if (request.Accounts == null || !request.Accounts.Any())
                    return new PdfChargePreview { ChargeAmount = 0, PageCount = 0, HasTransactions = false };

                // Create temp directory for previews
                var tempPath = Path.Combine(Path.GetTempPath(), "GTStatements", "Preview");
                Directory.CreateDirectory(tempPath);

                string month = request.EndDate.ToString("MMM", CultureInfo.CreateSpecificCulture("en-GB"));
                string fileName = $"PREVIEW_{month}_{request.Accounts.First().AccountNumber}_Stmt_{Guid.NewGuid()}.pdf";
                tempFilePath = Path.Combine(tempPath, fileName);

                // Set TypeOfSending to "preview" to generate preview
                request.TypeOfSending = "preview";

                // Generate the PDF (this will NOT call ProcessDownloadAsync for charging)
                var pdfResponse = await GenerateStatementsPdf(request, tempFilePath);

                if (!File.Exists(tempFilePath))
                {
                    return new PdfChargePreview
                    {
                        ChargeAmount = 0,
                        PageCount = 0,
                        HasTransactions = false,
                        TempFilePath = tempFilePath
                    };
                }

                // Get the actual page count from the generated PDF
                int pageCount = 0;
                using (var reader = new PdfReader(tempFilePath))
                {
                    pageCount = reader.NumberOfPages;
                }

                // Calculate estimated charge based on actual page count
                decimal chargePerPage = _configuration.GetValue<decimal>("StatementCharge:PerPage", 5.00m);
                decimal minimumCharge = _configuration.GetValue<decimal>("StatementCharge:Minimum", 5.00m);
                decimal maxCharge = _configuration.GetValue<decimal>("StatementCharge:Maximum", 100.00m);

                decimal estimatedCharge = Math.Max(pageCount * chargePerPage, minimumCharge);
                estimatedCharge = Math.Min(estimatedCharge, maxCharge);

                return new PdfChargePreview
                {
                    ChargeAmount = estimatedCharge,
                    PageCount = pageCount,
                    HasTransactions = pdfResponse.HasTransactions,
                    TempFilePath = tempFilePath,
                    Message = $"Estimated charge: {estimatedCharge:N2} cedis for {pageCount} page(s)"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF preview");

                // Clean up temp file if it exists
                if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                {
                    try { File.Delete(tempFilePath); } catch { }
                }

                throw;
            }
        }
        public async Task CleanupTempFile(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to delete temp file: {filePath}");
            }
        }
        /// <summary>
        /// Adds all the statement elements to the PDF document
        /// </summary>
        private void AddStatementElements(Document document, string accountName, string accountNumber,DateTime startDate, DateTime endDate, PdfPTable headerRow, PdfPTable dataTable)
        {
            // Statement period
            Paragraph periodParagraph = new Paragraph(
                $"Statement Period: {startDate:dd-MMM-yyyy} to {endDate:dd-MMM-yyyy}",
                FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))
            {
                IndentationLeft = 45,
                SpacingAfter = 5f
            };

            // Customer statement title
            Paragraph titleParagraph = new Paragraph("CUSTOMER STATEMENT",
                FontFactory.GetFont(FontFactory.HELVETICA, 12, Font.BOLD))
            {
                Alignment = Element.ALIGN_RIGHT,
                IndentationRight = 45
            };

            // Account name
            Paragraph nameParagraph = new Paragraph(accountName.ToUpper(),
                FontFactory.GetFont(FontFactory.HELVETICA, 14, Font.BOLD, new BaseColor(255, 69, 0)))
            {
                Alignment = Element.ALIGN_RIGHT,
                IndentationRight = 45,
                SpacingAfter = 10f
            };

            // Add all elements to document
            document.Add(periodParagraph);
            document.Add(headerRow);
            document.Add(titleParagraph);
            document.Add(nameParagraph);
            document.Add(dataTable);

            // Divider line
            Paragraph divider = new Paragraph(new string('-', 100),
                FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingBefore = 1,
                SpacingAfter = 1
            };
            document.Add(divider);
        }
        private async Task<PDFResponse> ProcessDownloadAsync(PDFResponse response, PDFRequest request, string branchCode, bool isPreview = false)
        {
            try
            {
                // If preview, just return the response without any charging logic
                if (isPreview)
                {
                    _logger.LogInformation($"Preview generated for CIF: {request.CIF_ID}, Pages: {response.PageCount}, Amount: {response.Amount}");
                    response.Message = $"Preview: {response.Amount} for {response.PageCount} page(s)";
                    return response;
                }

                // ============ ACTUAL DOWNLOAD CHARGING LOGIC ============

                // Check if compliance account - no charges
                if (request.isCompliance)
                {
                    response.Amount = "0.00";
                    response.Message = "Compliance account. Charges not applied.";
                    _logger.LogInformation($"Compliance account for CIF: {request.CIF_ID} - No charges applied");
                    return response;
                }

                // Check if staff account - no charges
                try
                {
                    if (!await _finacleApiClient.IsChargeable(request.CIF_ID))
                    {
                        response.Amount = "0.00";
                        response.Message = "Staff account. Charges not applied.";
                        _logger.LogInformation($"Staff account for CIF: {request.CIF_ID} - No charges applied");
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Failed to check if account is chargeable for CIF: {request.CIF_ID}. Defaulting to chargeable.");
                    // Continue with charging if check fails (default to chargeable)
                }

                // Check if there are transactions - no charges if no transactions
                if (!response.HasTransactions)
                {
                    response.Amount = "0.00";
                    response.Message = "No transactions found. Charges not applied.";
                    _logger.LogInformation($"No transactions for CIF: {request.CIF_ID} - No charges applied");
                    return response;
                }

                // Get the branch charge account
                var destination = await _finacleApiClient.GetBranchAccountForStatementCharges(branchCode);

                if (destination == null || !destination.Success)
                {
                    response.Amount = "0.00";
                    response.Message = "Branch charge account not found. Please charge customer at branch.";
                    _logger.LogWarning($"Branch charge account not found for branch: {branchCode}, CIF: {request.CIF_ID}");
                    return response;
                }

                // Get the actual page count from the PDF
                int pageCount = GetPdfPageCount(response.FilePath);
                response.PageCount = pageCount;

                // Calculate the final charge
                string charge = await _finacleApiClient.FinallyCalculatedStatementCharges(
                    destination.DestinationAccountNumber,
                    request.AccountToDebit,
                    pageCount,
                    "", // Additional parameters if needed
                    request.DebitCurrency);

                // Parse the charge amount
                if (decimal.TryParse(charge, out decimal chargeAmount))
                {
                    response.Amount = chargeAmount.ToString("N2");

                    if (chargeAmount > 0)
                    {
                        response.Message = $"Customer charged successfully. Amount: {chargeAmount:N2} for {pageCount} page(s).";
                        _logger.LogInformation($"Charged {chargeAmount:N2} for CIF: {request.CIF_ID}, Account: {request.AccountToDebit}, Pages: {pageCount}");
                    }
                    else
                    {
                        response.Message = "No charges applied.";
                        _logger.LogInformation($"No charges applied for CIF: {request.CIF_ID}");
                    }
                }
                else
                {
                    // If charge couldn't be parsed, default to 0
                    response.Amount = "0.00";
                    response.Message = "Unable to calculate charge amount. Please contact support.";
                    _logger.LogWarning($"Invalid charge amount returned: {charge} for CIF: {request.CIF_ID}");
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing download/charge for CIF: {request.CIF_ID}");

                // Return a safe response
                response.Amount = "0.00";
                response.Message = $"Error processing charges: {ex.Message}. Please contact support.";
                response.Success = false;

                return response;
            }
        }
        private int GetPdfPageCount(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                    return 0;

                using (var reader = new PdfReader(filePath))
                {
                    return reader.NumberOfPages;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error counting pages in PDF: {filePath}");
                return 0;
            }
        }
    }
}
