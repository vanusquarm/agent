using GTBStatementService.Models;
using iText.Commons.Actions;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
//using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Event;
using iText.Layout;
using iText.Layout.Borders;
//using iText.Layout.Canvas;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Globalization;
using System.Reflection;
using Path = System.IO.Path;

public class StatementPdfGenerator
{
    private readonly PdfFont font;
    private readonly PdfFont bold;
    private readonly string logoPath = "GTBStatementService.Assets.Images.glogo.jpg";

    public StatementPdfGenerator()
    {
        font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
        bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
    }

    public async Task<(byte[] PdfBytes, string FilePath)> GenerateStatementAsync(
         IEnumerable<BankStatement> accounts,
        DateTime startDate,
        DateTime endDate,
        string cifId)
    {
        string folder = Path.Combine(Path.GetTempPath(), "GTBStatements");
        Directory.CreateDirectory(folder);

        string filePath = Path.Combine(folder,
            $"AC_{endDate:MMM}_{cifId}_{Guid.NewGuid()}.pdf");

        using MemoryStream memoryStream = new();

        using (PdfWriter writer = new(memoryStream))
        using (PdfDocument pdf = new(writer))
        {
            //pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new FooterHandler(font));

            Document doc = new(pdf, PageSize.A4.Rotate()); // Rotate to landscape if needed: PageSize.A4.Rotate()
            doc.SetMargins(40, 40, 70, 40);

            foreach (var statement in accounts)
            {
                AddTopHeader(doc, startDate, endDate);
                AddAccountSection(doc, statement);
                AddTransactions(doc, statement);
                doc.Add(new AreaBreak());
            }

            doc.Close();
        }

        byte[] bytes = memoryStream.ToArray();
        await File.WriteAllBytesAsync(filePath, bytes);

        return (bytes, filePath);
    }

    // =============================
    // TOP HEADER
    // =============================

    private void AddTopHeader(Document doc, DateTime start, DateTime end)
    {
        Table header = new Table(new float[] { 3, 1 }).UseAllAvailableWidth();

        header.AddCell(new Cell()
            .Add(new Paragraph($"Statement Period  :{start:dd-MMM-yyyy} to {end:dd-MMM-yyyy}")
            .SetFont(bold)
            .SetFontSize(10))
            .SetBorder(Border.NO_BORDER)
            .SetVerticalAlignment(VerticalAlignment.MIDDLE));

        header.AddCell(new Cell()
            .Add(GetLogo())
            .SetBorder(Border.NO_BORDER)
            .SetTextAlignment(TextAlignment.RIGHT));

        doc.Add(header);
        doc.Add(new Paragraph("\n"));
    }

    // =============================
    // ACCOUNT + CUSTOMER HEADER
    // =============================

    private void AddAccountSection(Document doc, BankStatement ctx)
    {
        Table container = new Table(new float[] { 1, 1 }).UseAllAvailableWidth().SetBorder(Border.NO_BORDER);

        container.AddCell(AccountInfoTable(ctx));
        container.AddCell(CustomerTitle(ctx));

        doc.Add(container);
        doc.Add(new Paragraph("\n"));
    }

    // =============================
    // ACCOUNT INFO BOX
    // =============================

    private Table AccountInfoTable(BankStatement ctx)
    {
        Table table = new Table(2)
            .SetWidth(250);

        AddRow(table, "Print. Date", DateTime.UtcNow.ToString("dd-MMM-yyyy"));
        AddRow(table, "Branch Name", ctx.BranchName);
        AddRow(table, "Account No", ctx.AccountNumber);
        AddRow(table, "Internal Reference", "***");
        AddRow(table, "Account Type", ctx.AccountType);
        AddRow(table, "Currency", ctx.Currency);
        AddRow(table, "Opening Balance", ctx.OpeningBalance.ToString("N2"));

        return table;
    }

    private void AddRow(Table table, string label, string value)
    {
        table.AddCell(new Cell()
            .Add(new Paragraph(label).SetFont(bold).SetFontSize(9))
            .SetBackgroundColor(new DeviceRgb(230, 230, 230)));

        table.AddCell(new Cell()
            .Add(new Paragraph(value ?? "***").SetFont(font).SetFontSize(9)));
    }

    // =============================
    // CUSTOMER STATEMENT TITLE
    // =============================

    private Cell CustomerTitle(BankStatement ctx)
    {
        Paragraph title = new Paragraph("CUSTOMER STATEMENT")
            .SetFont(bold)
            .SetFontSize(14)
            .SetTextAlignment(TextAlignment.RIGHT);

        Paragraph name = new Paragraph(ctx.AccountName.ToUpper())
            .SetFont(bold)
            .SetFontSize(14)
            .SetFontColor(new DeviceRgb(230, 74, 25))
            .SetTextAlignment(TextAlignment.RIGHT);

        return new Cell()
            .Add(title)
            .Add(name)
            .SetBorder(Border.NO_BORDER)
            .SetVerticalAlignment(VerticalAlignment.BOTTOM);
    }

    // =============================
    // TRANSACTION TABLE
    // =============================

    private void AddTransactions(Document doc, BankStatement ctx)
    {
        Table table = new Table(new float[]
        {
            2,2,2,2,2,2,3,4
        }).UseAllAvailableWidth();

        AddHeader(table, "Trans. Date");
        AddHeader(table, "Value. Date");
        AddHeader(table, "Reference");
        AddHeader(table, "Debits");
        AddHeader(table, "Credits");
        AddHeader(table, "Balance");
        AddHeader(table, "Originating Branch");
        AddHeader(table, "Remarks");

        bool alt = false;

        foreach (var t in ctx.Transactions)
        {
            Color bg = alt ? new DeviceRgb(245, 245, 245) : ColorConstants.WHITE;

            table.AddCell(CellValue(t.TransactionDate.ToString("dd-MMM-yyyy"), bg));
            table.AddCell(CellValue(t.TransactionValueDate.ToString("dd-MMM-yyyy"), bg));
            table.AddCell(CellValue("" ?? "", bg));
            table.AddCell(CellValue(t.Debit.ToString("N2"), bg, TextAlignment.RIGHT));
            table.AddCell(CellValue(t.Credit.ToString("N2"), bg, TextAlignment.RIGHT));
            table.AddCell(CellValue(t.Balance.ToString("N2"), bg, TextAlignment.RIGHT));
            table.AddCell(CellValue(t.OriginatingBranch ?? "", bg));
            table.AddCell(CellValue(t.Remarks ?? "", bg));

            alt = !alt;
        }

        doc.Add(table);
    }

    // =============================
    // TABLE HEADER STYLE
    // =============================

    private void AddHeader(Table table, string text)
    {
        table.AddHeaderCell(
            new Cell()
            .Add(new Paragraph(text)
            .SetFont(bold)
            .SetFontColor(ColorConstants.WHITE)
            .SetFontSize(9))
            .SetBackgroundColor(ColorConstants.BLACK)
            .SetTextAlignment(TextAlignment.CENTER));
    }

    private Cell CellValue(string text, Color bg,
        TextAlignment align = TextAlignment.LEFT)
    {
        return new Cell()
            .Add(new Paragraph(text)
            .SetFont(font)
            .SetFontSize(9))
            .SetBackgroundColor(bg)
            .SetTextAlignment(align);
    }

    // =============================
    // LOGO
    // =============================

    private Image GetLogo()
    {
        var assembly = Assembly.GetExecutingAssembly();

        using Stream stream = assembly.GetManifestResourceStream(logoPath);
        using MemoryStream ms = new();
        stream.CopyTo(ms);

        ImageData img = ImageDataFactory.Create(ms.ToArray());

        return new Image(img).ScaleToFit(60, 60);
    }

    // =============================
    // FOOTER
    // =============================

    private class FooterHandler : IEventHandler
    {
        private readonly PdfFont font;

        public FooterHandler(PdfFont font)
        {
            this.font = font;
        }

        public void OnEvent(IEvent e)
        {
            PdfDocumentEvent ev = (PdfDocumentEvent)e;
            PdfDocument pdf = ev.GetDocument();
            PdfPage page = ev.GetPage();

            PdfCanvas canvas = new(page.NewContentStreamAfter(),
                page.GetResources(), pdf);

            Canvas doc = new(canvas, page.GetPageSize());

            doc.SetFont(font);

            doc.ShowTextAligned(
                "This is a computer generated Email. Please address all enquiries to Guaranty Trust Bank Liberia Systems and Control Unit Or the Customer Information Unit of your local branch.",
                page.GetPageSize().GetWidth() / 2,
                25,
                TextAlignment.CENTER);

            doc.ShowTextAligned(
                $"Page {pdf.GetPageNumber(page)}",
                page.GetPageSize().GetWidth() - 40,
                25,
                TextAlignment.RIGHT);

            doc.Close();
        }
    }
}