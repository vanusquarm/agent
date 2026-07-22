using Statement.Web.Models;

namespace Statement.Web.Interfaces
{
    public interface IGenerateExcelService
    {
        Task<string> GenerateStatementsExcelAsync(List<AccountInfo> accounts, string cifId, DateTime startDate, DateTime endDate, string email = null);
    }
}
