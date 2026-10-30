using iTextSharp.text;
using iTextSharp.text.pdf;
using Statement.Web.Interfaces;
using Statement.Web.Models;
using System.Data;
using System.Globalization;

namespace Statement.Web.Helpers
{
    public class PdfStatementHelper
    {
        private readonly ILogger _logger;
        private readonly IStatementClient _statementClient;

        public PdfStatementHelper(ILogger logger, IStatementClient statementClient)
        {
            _logger = logger;
            _statementClient = statementClient;
        }

        /// <summary>
        /// Builds the transaction data table for a single account
        /// </summary>
        public async Task<(PdfPTable dataTable, bool hasTransactions, decimal openingBalance, decimal closingBalance,
            decimal debitSum, decimal creditSum, int debitCount, int creditCount)>
            BuildTransactionTableAsync(string accountIdentification, DateTime startDate, DateTime endDate)
        {
            var (statement, openingBalance, closingBalance) = await _statementClient
                .GetStatementFromFinacleDbAsync(accountIdentification, startDate, endDate);

            bool noTransactions = statement == null || statement.Rows.Count == 0;

            // Create the data table with 6 columns
            PdfPTable datatable = new PdfPTable(6)
            {
                WidthPercentage = 100,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            float[] headerwidths = { 12f, 12f, 10f, 10f, 10f, 16f };
            datatable.SetWidths(headerwidths);

            // Add headers
            string[] headers = { "Trans. Date", "Value Date", "Debits", "Credits", "Balance", "Remarks" };
            Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8, BaseColor.WHITE);
            foreach (var header in headers)
            {
                PdfPCell headerCell = new PdfPCell(new Phrase(header, headerFont));
                headerCell.BackgroundColor = BaseColor.BLACK;
                headerCell.Border = Rectangle.NO_BORDER;
                datatable.AddCell(headerCell);
            }

            decimal debitSum = 0m, creditSum = 0m;
            int debitCount = 0, creditCount = 0;

            if (noTransactions)
            {
                datatable.AddCell(new PdfPCell(new Phrase("No transactions found",
                    FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.NORMAL)))
                { Colspan = 6, HorizontalAlignment = Element.ALIGN_CENTER });

                return (datatable, false, openingBalance, closingBalance, 0, 0, 0, 0);
            }

            // Process each transaction row
            foreach (DataRow rowStmt in statement.Rows)
            {
                string tradate = FormatDate(rowStmt["TransactionDate"]);
                string valdate = FormatDate(rowStmt["TransactionValueDate"]);

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

            // Add totals row
            AddTotalsRow(datatable, debitSum, creditSum, closingBalanceValue, debitCount, creditCount);

            return (datatable, true, openingBalance, closingBalance, debitSum, creditSum, debitCount, creditCount);
        }

        /// <summary>
        /// Builds the header information table for an account
        /// </summary>
        public PdfPTable BuildAccountHeaderTable(string accountIdentification, string accountCurrency,
            decimal openingBalance, decimal closingBalance, CustomerAccountDetails details, string branch)
        {
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
            Headerrow.AddCell(new Phrase($"{details?.Branch?.ToUpper() ?? branch.ToUpper()}", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
            Headerrow.AddCell(new Phrase("Account No.", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
            Headerrow.AddCell(new Phrase($"{accountIdentification}", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
            Headerrow.AddCell(new Phrase("Address", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
            Headerrow.AddCell(new Phrase($"{details?.Address?.ToUpper() ?? ""}", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
            Headerrow.AddCell(new Phrase("Account Type", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
            Headerrow.AddCell(new Phrase(details?.AccountType?.ToUpper() ?? "", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
            Headerrow.AddCell(new Phrase("Currency", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
            Headerrow.AddCell(new Phrase($"{accountCurrency.ToUpper()}", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
            Headerrow.AddCell(new Phrase("Opening Balance", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
            Headerrow.AddCell(new Phrase(openingBalance.ToString("N2"), FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
            Headerrow.AddCell(new Phrase("Closing Balance", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));
            Headerrow.AddCell(new Phrase(closingBalance.ToString("N2"), FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD)));

            return Headerrow;
        }

        /// <summary>
        /// Adds the totals summary row to the transaction table
        /// </summary>
        private void AddTotalsRow(PdfPTable datatable, decimal debitSum, decimal creditSum,
            string closingBalance, int debitCount, int creditCount)
        {
            datatable.AddCell(new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });
            datatable.AddCell(new PdfPCell(new Phrase("TOTAL:", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });
            datatable.AddCell(new PdfPCell(new Phrase(debitSum.ToString("N2"), FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });
            datatable.AddCell(new PdfPCell(new Phrase(creditSum.ToString("N2"), FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });
            datatable.AddCell(new PdfPCell(new Phrase(closingBalance, FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });
            datatable.AddCell(new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });

            datatable.AddCell(new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });
            datatable.AddCell(new PdfPCell(new Phrase("Total", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });
            datatable.AddCell(new PdfPCell(new Phrase($"{debitCount} debits", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });
            datatable.AddCell(new PdfPCell(new Phrase($"{creditCount} credits", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });
            datatable.AddCell(new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });
            datatable.AddCell(new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD))) { Border = Rectangle.NO_BORDER });
        }

        /// <summary>
        /// Formats a database date value to a consistent string format
        /// </summary>
        private string FormatDate(object dateValue)
        {
            if (dateValue == DBNull.Value)
                return "";

            if (dateValue is DateTime date)
                return date.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture).ToUpper();

            return dateValue.ToString().Trim();
        }
    }
}