using Statement.Web.Models;

namespace Statement.Web.Interfaces
{
    public interface IGeneratePdfService
    {
        Task<PDFResponse> GenerateStatementsPdf(PDFRequest pDFRequest, string outputFile = null);
        Task<PdfChargePreview> GeneratePreviewPdf(PDFRequest request);
        Task CleanupTempFile(string filePath);
    }
}
