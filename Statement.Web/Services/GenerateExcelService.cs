using ClosedXML.Excel;
using Oracle.ManagedDataAccess.Client;
using Statement.Web.Interfaces;
using Statement.Web.Models;
using System.Data;
using System.Globalization;

namespace Statement.Web.Services
{
    public class GenerateExcelService : IGenerateExcelService
    {
        private readonly ILogger<GenerateExcelService> _logger;
        private readonly IHostEnvironment _environment;
        private readonly IMiddlewareClient _finacleApiClient;
        private readonly IStatementClient _statementClient;
        private readonly string _connectionString;
        private const string GDPC_NOTICE = "Guaranty Trust Bank (Ghana) Ltd is a member of the Ghana Deposit Protection Scheme. " +
                                           "For further information about the Ghana Deposit Protection Scheme, refer to GDPC's website: www.gdpc.gov.gh.";

        public GenerateExcelService(ILogger<GenerateExcelService> logger,IHostEnvironment environment, IMiddlewareClient finacleApiClient,  IConfiguration configuration, IStatementClient statementClient)
        {
            _logger = logger;
            _environment = environment;
            _finacleApiClient = finacleApiClient;
            _connectionString = configuration.GetConnectionString("FinacleConn") ?? throw new InvalidOperationException("Database connection string is not configured.");
            _statementClient = statementClient;
        }

        public async Task<string> GenerateStatementsExcelAsync(List<AccountInfo> accounts,string cifId,DateTime startDate,DateTime endDate,string email = null)
        {
            try
            {
                // Create output directory
                string outputDirectory = CreateOutputDirectory();
                string outputFile = GenerateOutputFilePath(outputDirectory, cifId, endDate);

                using var workbook = new XLWorkbook();

                // Initialize worksheet
                InitializeWorkbook(workbook);

                bool hasValidStatement = false;
                int sheetIndex = 0;

                foreach (var account in accounts)
                {
                    var sheetData = await BuildSheetDataAsync(account, cifId, startDate, endDate, sheetIndex);

                    if (sheetData == null)
                        continue;

                    var worksheet = GetOrCreateWorksheet(workbook, sheetData.SheetName, sheetIndex);

                    PopulateWorksheet(worksheet, sheetData, startDate, endDate);

                    if (sheetData.HasTransactions)
                        hasValidStatement = true;

                    sheetIndex++;
                }

                // Save workbook
                workbook.SaveAs(outputFile, true);

                _logger.LogInformation("Excel file generated successfully: {FilePath}", outputFile);
                return outputFile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Excel for CIF_ID: {CIF_ID}", cifId);
                return string.Empty;
            }
        }

        private string CreateOutputDirectory()
        {
            string downloadsPath = Path.Combine(_environment.ContentRootPath, "Downloads", "Individual");
            Directory.CreateDirectory(downloadsPath);
            return downloadsPath;
        }

        private string GenerateOutputFilePath(string directory, string cifId, DateTime endDate)
        {
            string month = endDate.ToString("MMM", CultureInfo.CreateSpecificCulture("en-GB"));
            string uniqueId = DateTime.UtcNow.Ticks.ToString();
            return Path.Combine(directory, $"{cifId}_{uniqueId}_{month}_Stmt.xlsx");
        }

        private void InitializeWorkbook(XLWorkbook workbook)
        {
            // Remove default sheet
            var defaultSheet = workbook.Worksheets.FirstOrDefault(ws => ws.Name == "Sheet1");
            if (defaultSheet != null)
                workbook.Worksheets.Delete(defaultSheet.Name);

            // Add placeholder sheet
            workbook.Worksheets.Add("BlankSheet");
        }

        private IXLWorksheet GetOrCreateWorksheet(XLWorkbook workbook, string sheetName, int sheetIndex)
        {
            if (sheetIndex == 0 && workbook.Worksheets.Any(ws => ws.Name == "BlankSheet"))
            {
                var worksheet = workbook.Worksheets.First(ws => ws.Name == "BlankSheet");
                worksheet.Name = sheetName;
                return worksheet;
            }

            return workbook.Worksheets.Add(sheetName);
        }

        private async Task<ExcelSheetData> BuildSheetDataAsync(AccountInfo account,string cifId,DateTime startDate,DateTime endDate,int sheetIndex)
        {
            try
            {
                string accountNumber = account.AccountNumber;
                string branchCode = account.BranchCode.ToString();
                string accountCurrency = account.Currency;
                string availableBalance = account.Balance.ToString();
                string accountName = account.AccountName;

                var details = await _statementClient.GetCustomerAccountDetailsAsync(accountNumber);
                var (statement, openingBalance, closingBalance) = await _statementClient.GetStatementFromFinacleDbAsync( accountNumber, startDate, endDate);

                // Generate unique sheet name
                string sheetName = $"{cifId}_{sheetIndex}";
                int suffix = 1;

                return new ExcelSheetData
                {
                    AccountNumber = accountNumber,
                    AccountName = accountName,
                    AccountCurrency = accountCurrency,
                    BranchName = details.Branch,
                    Address = details.Address,
                    ProductType = details.AccountType,
                    Statement = statement,
                    OpeningBalance = openingBalance,
                    ClosingBalance = closingBalance,
                    SheetName = sheetName,
                    HasTransactions = statement != null && statement.Rows.Count > 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to build sheet data for account: {AccountNumber}", account.AccountNumber);
                return null;
            }
        }

        private void PopulateWorksheet(IXLWorksheet worksheet, ExcelSheetData data, DateTime startDate, DateTime endDate)
        {
            string imageFilePath = Path.Combine(_environment.ContentRootPath, "Resources", "glogo.jpg");
            string bankDate = DateTime.UtcNow.ToString("dd-MMM-yyyy", CultureInfo.CreateSpecificCulture("en-GB"));
            string startDateFormatted = startDate.ToString("dd-MMM-yyyy", CultureInfo.CreateSpecificCulture("en-GB"));
            string endDateFormatted = endDate.ToString("dd-MMM-yyyy", CultureInfo.CreateSpecificCulture("en-GB"));

            // Add logo
            AddLogo(worksheet, imageFilePath);

            // Add header information
            AddHeaderInformation(worksheet, data, bankDate);

            // Add title
            AddTitle(worksheet, data.AccountName, startDateFormatted, endDateFormatted);

            // Add opening/closing balances
            AddBalanceInformation(worksheet, data.OpeningBalance, data.ClosingBalance);

            // Add transaction table
            int lastRow = AddTransactionTable(worksheet, data.Statement, startDate, endDate);

            // Add GDPC notice
            AddGdpcNotice(worksheet, lastRow);

            // Format columns
            worksheet.Columns().AdjustToContents();
        }

        private void AddLogo(IXLWorksheet worksheet, string imageFilePath)
        {
            try
            {
                if (File.Exists(imageFilePath))
                {
                    worksheet.AddPicture(imageFilePath)
                             .MoveTo(worksheet.Cell(1, 8))
                             .WithSize(100, 120);
                }
                else
                {
                    _logger.LogWarning("Logo file not found: {Path}", imageFilePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add logo to Excel");
            }
        }

        private void AddHeaderInformation(IXLWorksheet worksheet, ExcelSheetData data, string bankDate)
        {
            var headerData = new Dictionary<string, string>
            {
                ["Print. Date"] = bankDate,
                ["Branch Name"] = data.BranchName,
                ["Account No."] = data.AccountNumber,
                ["Address"] = data.Address,
                ["Account Type"] = data.ProductType,
                ["Currency"] = data.AccountCurrency
            };

            int row = 3;
            foreach (var item in headerData)
            {
                var labelCell = worksheet.Cell(row, 1);
                labelCell.Value = item.Key;
                labelCell.Style.Font.Bold = true;
                labelCell.Style.Font.FontName = "Arial";

                var valueCell = worksheet.Cell(row, 2);
                valueCell.Value = item.Value ?? "";
                valueCell.Style.Font.FontName = "Arial";

                row++;
            }
        }

        private void AddTitle(IXLWorksheet worksheet, string accountName, string startDate, string endDate)
        {
            // Title
            var titleCell = worksheet.Cell(8, 8);
            titleCell.Value = "CUSTOMER STATEMENT";
            titleCell.Style.Font.Bold = true;
            titleCell.Style.Font.FontName = "Arial";
            titleCell.Style.Font.FontSize = 12;

            // Account name
            var nameCell = worksheet.Cell(9, 8);
            nameCell.Value = accountName;
            nameCell.Style.Font.Bold = true;
            nameCell.Style.Font.FontName = "Arial";
            nameCell.Style.Font.FontSize = 14;
            nameCell.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 140, 0);

            // Period
            var periodCell = worksheet.Cell(10, 8);
            periodCell.Value = $"Statement Period: {startDate} to {endDate}";
            periodCell.Style.Font.Bold = true;
            periodCell.Style.Font.FontName = "Arial";
        }

        private void AddBalanceInformation(IXLWorksheet worksheet, decimal openingBalance, decimal closingBalance)
        {
            worksheet.Cell(9, 1).Value = "Opening Balance";
            worksheet.Cell(9, 1).Style.Font.Bold = true;
            worksheet.Cell(9, 1).Style.Font.FontName = "Arial";

            worksheet.Cell(9, 2).Value = openingBalance;
            worksheet.Cell(9, 2).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(9, 2).Style.Font.Bold = true;
            worksheet.Cell(9, 2).Style.Font.FontName = "Arial";

            worksheet.Cell(10, 1).Value = "Closing Balance";
            worksheet.Cell(10, 1).Style.Font.Bold = true;
            worksheet.Cell(10, 1).Style.Font.FontName = "Arial";

            worksheet.Cell(10, 2).Value = closingBalance;
            worksheet.Cell(10, 2).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(10, 2).Style.Font.Bold = true;
            worksheet.Cell(10, 2).Style.Font.FontName = "Arial";
        }

        private int AddTransactionTable(IXLWorksheet worksheet, DataTable statement, DateTime startDate, DateTime endDate)
        {
            int startRow = 11;
            int startCol = 1;
            string[] headers = { "Trans. Date", "Value Date", "Debits", "Credit", "Balance", "Remarks" };

            // Add headers
            for (int col = 0; col < headers.Length; col++)
                worksheet.Cell(startRow, startCol + col).Value = headers[col];

            var headerRowRange = worksheet.Range($"A{startRow}:F{startRow}");
            headerRowRange.Style.Font.Bold = true;
            headerRowRange.Style.Font.FontName = "Arial";
            headerRowRange.Style.Fill.BackgroundColor = XLColor.Black;
            headerRowRange.Style.Font.FontColor = XLColor.White;

            int lastRow = startRow;

            if (statement == null)
            {
                worksheet.Range(startRow + 1, 1, startRow + 1, 6).Merge();
                var noStmtCell = worksheet.Cell(startRow + 1, 1);
                noStmtCell.Value = "No Statement Found";
                noStmtCell.Style.Font.Bold = true;
                noStmtCell.Style.Font.FontName = "Arial";
                noStmtCell.Style.Font.FontSize = 12;
                noStmtCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                lastRow = startRow + 1;
            }
            else if (statement.Rows.Count == 0 ||
                     (statement.Rows.Count == 1 && statement.Rows[0]["Remarks"]?.ToString() == "No transactions found"))
            {
                worksheet.Range(startRow + 1, 1, startRow + 1, 6).Merge();
                var noTxCell = worksheet.Cell(startRow + 1, 1);
                noTxCell.Value = "No transactions found";
                noTxCell.Style.Font.FontName = "Arial";
                noTxCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                lastRow = startRow + 1;
            }
            else
            {
                int currentRow = startRow + 1;
                decimal debitSum = 0m, creditSum = 0m;
                int debitCount = 0, creditCount = 0;
                decimal lastBalance = 0m;

                foreach (DataRow rowStmt in statement.Rows)
                {
                    decimal debitAmount = rowStmt["Debit"] != DBNull.Value ? Convert.ToDecimal(rowStmt["Debit"]) : 0m;
                    decimal creditAmount = rowStmt["Credit"] != DBNull.Value ? Convert.ToDecimal(rowStmt["Credit"]) : 0m;

                    string debit = debitAmount > 0 ? debitAmount.ToString("N2") : "";
                    string credit = creditAmount > 0 ? creditAmount.ToString("N2") : "";
                    string transDate = rowStmt["TransactionDate"]?.ToString()?.Trim() ?? "";
                    string valueDate = rowStmt["TransactionValueDate"]?.ToString()?.Trim() ?? "";
                    string balance = rowStmt["Balance"] != DBNull.Value
                                         ? Convert.ToDecimal(rowStmt["Balance"]).ToString("N2")
                                         : "0.00";
                    string remark = rowStmt["Remarks"]?.ToString()?.Trim() ?? "";

                    if (debitAmount > 0) { debitSum += debitAmount; debitCount++; }
                    if (creditAmount > 0) { creditSum += creditAmount; creditCount++; }
                    lastBalance = Convert.ToDecimal(balance);

                    worksheet.Cell(currentRow, startCol).Value = transDate;
                    worksheet.Cell(currentRow, startCol + 1).Value = valueDate;
                    worksheet.Cell(currentRow, startCol + 2).Value = debit;
                    worksheet.Cell(currentRow, startCol + 3).Value = credit;
                    worksheet.Cell(currentRow, startCol + 4).Value = balance;
                    worksheet.Cell(currentRow, startCol + 5).Value = remark;

                    // Apply number formatting
                    if (!string.IsNullOrEmpty(debit))
                        worksheet.Cell(currentRow, startCol + 2).Style.NumberFormat.Format = "#,##0.00";
                    if (!string.IsNullOrEmpty(credit))
                        worksheet.Cell(currentRow, startCol + 3).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(currentRow, startCol + 4).Style.NumberFormat.Format = "#,##0.00";

                    currentRow++;
                }

                // Totals row
                worksheet.Cell(currentRow, startCol + 1).Value = "TOTAL:";
                worksheet.Cell(currentRow, startCol + 1).Style.Font.Bold = true;
                worksheet.Cell(currentRow, startCol + 1).Style.Font.FontName = "Arial";

                worksheet.Cell(currentRow, startCol + 2).Value = debitSum.ToString("N2");
                worksheet.Cell(currentRow, startCol + 2).Style.Font.Bold = true;
                worksheet.Cell(currentRow, startCol + 2).Style.Font.FontName = "Arial";
                worksheet.Cell(currentRow, startCol + 2).Style.NumberFormat.Format = "#,##0.00";

                worksheet.Cell(currentRow, startCol + 3).Value = creditSum.ToString("N2");
                worksheet.Cell(currentRow, startCol + 3).Style.Font.Bold = true;
                worksheet.Cell(currentRow, startCol + 3).Style.Font.FontName = "Arial";
                worksheet.Cell(currentRow, startCol + 3).Style.NumberFormat.Format = "#,##0.00";

                worksheet.Cell(currentRow, startCol + 4).Value = lastBalance.ToString("N2");
                worksheet.Cell(currentRow, startCol + 4).Style.Font.Bold = true;
                worksheet.Cell(currentRow, startCol + 4).Style.Font.FontName = "Arial";
                worksheet.Cell(currentRow, startCol + 4).Style.NumberFormat.Format = "#,##0.00";

                // Counts row
                currentRow++;
                worksheet.Cell(currentRow, startCol + 1).Value = "Total";
                worksheet.Cell(currentRow, startCol + 1).Style.Font.Bold = true;
                worksheet.Cell(currentRow, startCol + 1).Style.Font.FontName = "Arial";

                worksheet.Cell(currentRow, startCol + 2).Value = $"{debitCount} debits";
                worksheet.Cell(currentRow, startCol + 2).Style.Font.Bold = true;
                worksheet.Cell(currentRow, startCol + 2).Style.Font.FontName = "Arial";

                worksheet.Cell(currentRow, startCol + 3).Value = $"{creditCount} credits";
                worksheet.Cell(currentRow, startCol + 3).Style.Font.Bold = true;
                worksheet.Cell(currentRow, startCol + 3).Style.Font.FontName = "Arial";

                lastRow = currentRow;
            }

            return lastRow;
        }

        private void AddGdpcNotice(IXLWorksheet worksheet, int lastRow)
        {
            int noticeRow = lastRow + 2;
            worksheet.Range(noticeRow, 1, noticeRow, 6).Merge();
            var noticeCell = worksheet.Cell(noticeRow, 1);
            noticeCell.Value = GDPC_NOTICE;
            noticeCell.Style.Font.FontName = "Arial";
            noticeCell.Style.Font.FontSize = 9;
            noticeCell.Style.Font.Italic = true;
            noticeCell.Style.Font.FontColor = XLColor.FromArgb(70, 70, 70);
            noticeCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            noticeCell.Style.Alignment.WrapText = true;
            worksheet.Row(noticeRow).Height = 30;
        }
    }
}

