using Statement.Web.Models;
using System.Data;

namespace Statement.Web.Interfaces
{
    public interface IStatementClient
    {
        Task<CustomerAccountDetails> GetCustomerAccountDetailsAsync(string accountNumber);
        Task<(DataTable, decimal, decimal)> GetStatementFromFinacleDbAsync(string accountNumber, DateTime fromDate, DateTime toDate, string correlationId = null);
    }
}
