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

            _SenderEmail = configuration["SenderEmail"];
            _NotifierCopyEmail = configuration["NotifierCopyEmail"];
            _NotifierToEmail = configuration["NotifierToEmail"];
            _emaiilService = emaiilService;
        }

        public async Task<PDFResponse> GenerateStatementsPdf(PDFRequest request)
        {
            string outputFile = string.Empty;
            string acct_name = string.Empty;
            string email = string.Empty;
            bool noTransactions = false;
            string branchCode = request.Accounts.FirstOrDefault().BranchCode.ToString() ?? "201";
            string message = "Successfully generated statement";
            try
            {
                if (request.Accounts == null || request.Accounts.Count == 0)
                {
                    _logger.LogWarning($"No accounts found for CIF_ID: {request.CIF_ID}");
                    return (new PDFResponse());
                }

                if (string.IsNullOrEmpty(outputFile) || !File.Exists(outputFile))
                {
                    // Regenerate PDF if temp file is missing
                    //string StartUpPath = System.IO.Path.Combine(HostingEnvironment.MapPath("~/Downloads"), "Individual");
                    //Directory.CreateDirectory(StartUpPath);

                    //outputFile = Path.Combine(StartUpPath, $"Stmt_{EndDateTime:MMM}_{CIF_ID}_Stmt_{Guid.NewGuid().ToString()}.pdf");
                    var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "GTStatements", "Temp");
                    Directory.CreateDirectory(tempPath);

                    string month = request.EndDate.ToString("MMM", CultureInfo.CreateSpecificCulture("en-GB"));
                    string fileName = $"GTBANK_{month}_{request.Accounts.FirstOrDefault().AccountNumber}_Stmt_{Guid.NewGuid()}.pdf";
                    outputFile = System.IO.Path.Combine(tempPath, fileName);

                    Document document = new Document(PageSize.A4, 10, 10, 10, 40);
                    using (FileStream fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        PdfWriter writer = PdfWriter.GetInstance(document, fs);
                        writer.PageEvent = new CustomFooterEvent();
                        document.Open();

                        //string imageFilePath = System.IO.Path.Combine(HostingEnvironment.MapPath("~/Resources"), "glogo.jpg");
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
                                string bracode = account.BranchCode.ToString();
                                string accountIdentification = account.AccountNumber;
                                string accountCurrency = account.Currency;
                                string availableBalance = account.Balance.ToString();
                                string productType = "";
                                string address = "";
                                string branchName = "";
                                acct_name = account.AccountName;

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


                                var details = await _statementClient.GetCustomerAccountDetailsAsync(accountIdentification);

                                if (string.IsNullOrEmpty(accountIdentification))
                                {
                                    //_logger.LogWarning($"Invalid accountIdentification for CIF_ID: {request.re}");
                                    continue;
                                }

                                (var statement, decimal openingBalance, decimal closingBalance) = await _statementClient.GetStatementFromFinacleDbAsync(accountIdentification, request.StartDate, request.EndDate);

                                noTransactions = statement != null && statement.Rows.Count == 0;

                                Paragraph paragraph9 = new Paragraph(
                                    $"Statement Period: {request.StartDate:dd-MMM-yyyy} to {request.EndDate:dd-MMM-yyyy}",
                                    FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD));
                                paragraph9.IndentationLeft = 45;
                                paragraph9.SpacingAfter = 5f;

                                Paragraph paragraph1 = new Paragraph("CUSTOMER STATEMENT",
                                    FontFactory.GetFont(FontFactory.HELVETICA, 12, Font.BOLD));
                                paragraph1.Alignment = Element.ALIGN_RIGHT;

                                Paragraph paragraph2 = new Paragraph(acct_name.ToUpper(),
                                    FontFactory.GetFont(FontFactory.HELVETICA, 14, Font.BOLD, new BaseColor(255, 69, 0)));
                                paragraph2.Alignment = Element.ALIGN_RIGHT;
                                paragraph2.SpacingAfter = 10f;

                                PdfPTable Headerrow = new PdfPTable(2)
                                {
                                    WidthPercentage = 40,
                                    HorizontalAlignment = Element.ALIGN_LEFT
                                };
                                float[] widths = { 1f, 2f };
                                Headerrow.SetWidths(widths);

                                Headerrow.AddCell(new Phrase("Print. Date", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
                                Headerrow.AddCell(new Phrase(DateTime.UtcNow.ToString("dd-MMM-yyyy"), FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
                                Headerrow.AddCell(new Phrase("Branch Name", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
                                Headerrow.AddCell(new Phrase($"{branchName.ToUpper()}", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
                                Headerrow.AddCell(new Phrase("Account No.", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
                                Headerrow.AddCell(new Phrase($"{accountIdentification}", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
                                Headerrow.AddCell(new Phrase("Address", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
                                Headerrow.AddCell(new Phrase($"{address.ToUpper()}", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
                                Headerrow.AddCell(new Phrase("Account Type", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
                                Headerrow.AddCell(new Phrase(productType.ToUpper(), FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
                                Headerrow.AddCell(new Phrase("Currency", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
                                Headerrow.AddCell(new Phrase($"{accountCurrency.ToUpper()}", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
                                Headerrow.AddCell(new Phrase("Opening Balance", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
                                Headerrow.AddCell(new Phrase(openingBalance.ToString("N2"), FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
                                Headerrow.AddCell(new Phrase("Closing Balance", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
                                Headerrow.AddCell(new Phrase(closingBalance.ToString("N2"), FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));

                                PdfPTable datatable = new PdfPTable(6)
                                {
                                    WidthPercentage = 100,
                                    HorizontalAlignment = Element.ALIGN_CENTER
                                };
                                float[] headerwidths = { 12f, 12f, 10f, 10f, 10f, 16f };
                                datatable.SetWidths(headerwidths);
                                string[] headers = { "Trans. Date", "Value Date", "Debits", "Credits", "Balance", "Remarks" };
                                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8, BaseColor.WHITE);
                                foreach (var header in headers)
                                {
                                    PdfPCell headerCell = new PdfPCell(new Phrase(header, headerFont));
                                    headerCell.BackgroundColor = BaseColor.BLACK;
                                    headerCell.Border = Rectangle.NO_BORDER;
                                    datatable.AddCell(headerCell);
                                }

                                if (statement == null)
                                {
                                    datatable.AddCell(new PdfPCell(new Phrase("No Statement Found", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)))
                                    { Colspan = 6, HorizontalAlignment = Element.ALIGN_CENTER });
                                }
                                else if (noTransactions)
                                {
                                    datatable.AddCell(new PdfPCell(new Phrase("No transactions found", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.NORMAL)))
                                    { Colspan = 6, HorizontalAlignment = Element.ALIGN_CENTER });
                                }
                                else
                                {
                                    hasActualData = true;
                                    decimal debitSum = 0m, creditSum = 0m;
                                    int debitCount = 0, creditCount = 0;

                                    foreach (DataRow rowStmt in statement.Rows)
                                    {
                                        string tradate = rowStmt["TransactionDate"] != DBNull.Value
                                            ? (rowStmt["TransactionDate"] is DateTime date
                                                ? date.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture).ToUpper()
                                                : rowStmt["TransactionDate"].ToString().Trim())
                                            : "";
                                        string valdate = rowStmt["TransactionValueDate"] != DBNull.Value
                                            ? (rowStmt["TransactionValueDate"] is DateTime date1
                                                ? date1.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture).ToUpper()
                                                : rowStmt["TransactionValueDate"].ToString().Trim())
                                            : "";

                                        decimal debitAmount = rowStmt["Debit"] != DBNull.Value ? Convert.ToDecimal(rowStmt["Debit"]) : 0m;
                                        decimal creditAmount = rowStmt["Credit"] != DBNull.Value ? Convert.ToDecimal(rowStmt["Credit"]) : 0m;

                                        string debit = debitAmount > 0 ? debitAmount.ToString("N2") : "";
                                        string credit = creditAmount > 0 ? creditAmount.ToString("N2") : "";
                                        string balance = rowStmt["Balance"] != DBNull.Value ? Convert.ToDecimal(rowStmt["Balance"]).ToString("N2") : "0.00";
                                        string remark = rowStmt["Remarks"] != DBNull.Value ? rowStmt["Remarks"].ToString().Trim() : "";

                                        if (debitAmount > 0) { debitSum += debitAmount; debitCount++; }
                                        if (creditAmount > 0) { creditSum += creditAmount; creditCount++; }

                                        datatable.AddCell(new Phrase(tradate, FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.NORMAL)));
                                        datatable.AddCell(new Phrase(valdate, FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.NORMAL)));
                                        datatable.AddCell(new Phrase(debit, FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.NORMAL)));
                                        datatable.AddCell(new Phrase(credit, FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.NORMAL)));
                                        datatable.AddCell(new Phrase(balance, FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.NORMAL)));
                                        datatable.AddCell(new Phrase(remark, FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.NORMAL)));
                                    }

                                    string closingBalanceValue = statement.Rows.Count > 0
                                        ? Convert.ToDecimal(statement.Rows[statement.Rows.Count - 1]["Balance"]).ToString("N2")
                                        : closingBalance.ToString("N2");

                                    datatable.AddCell(new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });
                                    datatable.AddCell(new PdfPCell(new Phrase("TOTAL:", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });
                                    datatable.AddCell(new PdfPCell(new Phrase(debitSum.ToString("N2"), FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });
                                    datatable.AddCell(new PdfPCell(new Phrase(creditSum.ToString("N2"), FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });
                                    datatable.AddCell(new PdfPCell(new Phrase(closingBalanceValue, FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });
                                    datatable.AddCell(new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });

                                    datatable.AddCell(new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });
                                    datatable.AddCell(new PdfPCell(new Phrase("Total", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });
                                    datatable.AddCell(new PdfPCell(new Phrase($"{debitCount} debits", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });
                                    datatable.AddCell(new PdfPCell(new Phrase($"{creditCount} credits", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });
                                    datatable.AddCell(new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });
                                    datatable.AddCell(new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });
                                }

                                document.Add(paragraph9);
                                document.Add(Headerrow);
                                paragraph1.IndentationRight = 45;
                                paragraph2.IndentationRight = 45;
                                document.Add(paragraph1);
                                document.Add(paragraph2);
                                document.Add(datatable);

                                Paragraph paragraph18 = new Paragraph(new string('-', 100), FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))
                                {
                                    Alignment = Element.ALIGN_CENTER,
                                    SpacingBefore = 1,
                                    SpacingAfter = 1
                                };
                                document.Add(paragraph18);

                                accountIndex++;
                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        }

                        document.Close();
                    }
                }

                if (request.AccountToDebit == null)
                {
                    request.AccountToDebit = request.Accounts.FirstOrDefault()?.AccountNumber;
                }

                var response = new PDFResponse
                {
                    FilePath = outputFile,
                    CustomerName = acct_name,
                    EmailAddress = email,
                    HasTransactions = !noTransactions,
                    Success = true
                };

                if (request.TypeOfSending.Equals("download", StringComparison.OrdinalIgnoreCase))
                {
                    return await ProcessDownloadAsync(
                        response,
                        request,
                        branchCode);
                }

                // Email request.
                // Simply return the generated PDF information.
                // The controller will send the email.
                response.Amount = "0.00";
                response.Message = "PDF generated successfully.";

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating PDF for CIF_ID: {request.CIF_ID}: {ex.Message}", ex);
                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                }
                _emaiilService.SendMessage(
                    _NotifierToEmail,
                    _NotifierCopyEmail,
                    _SenderEmail,
                    $"Error Generating Statement for: {request.CIF_ID}",
                    $"An Error Occurred: {ex.Message}");
                return (new PDFResponse());
            }
        }

        private async Task<PDFResponse> ProcessDownloadAsync(PDFResponse response, PDFRequest request, string branchCode)
        {
            if (request.isCompliance)
            {
                response.Amount = "0.00";
                response.Message = "Compliance account. Charges not applied.";
                return response;
            }

            if (!await _finacleApiClient.IsChargeable(request.CIF_ID))
            {
                response.Amount = "0.00";
                response.Message = "Staff account. Charges not applied.";
                return response;
            }

            if (!response.HasTransactions)
            {
                response.Amount = "0.00";
                response.Message = "No transactions found. Charges not applied.";
                return response;
            }

            var destination = await _finacleApiClient.GetBranchAccountForStatementCharges(branchCode);

            if (destination == null || !destination.Success)
            {
                response.Amount = "0.00";
                response.Message =
                    "Branch charge account not found. Charge customer at branch.";

                return response;
            }

            var charge =
                await _finacleApiClient.FinallyCalculatedStatementCharges(
                    destination.DestinationAccountNumber,
                    request.AccountToDebit,
                    GetPdfPageCount(response.FilePath!),
                    "",
                    request.DebitCurrency);

            response.Amount = charge;

            response.Message = charge == "0.00"
                ? "No charges applied."
                : "Customer charged successfully.";

            return response;
        }
        private int GetPdfPageCount(string pdfFilePath)
        {
            using (var reader = new iTextSharp.text.pdf.PdfReader(pdfFilePath))
            {
                return reader.NumberOfPages;
            }
        }
    }

}
