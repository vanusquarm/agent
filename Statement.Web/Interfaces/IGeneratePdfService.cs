using Statement.Web.Models;

namespace Statement.Web.Interfaces
{
    public interface IGeneratePdfService
    {
        Task<PDFResponse> GenerateStatementsPdf(PDFRequest pDFRequest);
    }
}
