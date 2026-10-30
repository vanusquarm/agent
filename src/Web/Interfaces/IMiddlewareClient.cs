using Statement.Web.Models;

namespace Statement.Web.Interfaces
{
    public interface IMiddlewareClient
    {
        Task<List<Account>> GetCustomerCIFAccountsAsync(string customerId, string requestId = null, CancellationToken cancellationToken = default);
        Task<AccountCustomerResponse> GetAccountsAndCustomersAsync(string accountId, string requestId = null, CancellationToken cancellationToken = default);
        Task<TransactionLogprocedureResponse> LogTransferTransaction(
        string sourceAccount,
        string destAccount,
        decimal amount,
        string remarks,
        string currency,
        string channelReference,
        IntraBankTransferResponse transferResponse,
        string requestId,
        string? initiatingUserId);

        Task<string> FinallyCalculatedStatementCharges(string destinationAccount, string accountNumber, int NoOfPages, string tranAmt, string DebitAccountCurrency);
        Task<bool> IsChargeable(string cif);
        Task<BranchAccountProcedureResponse> GetBranchAccountForStatementCharges(string branchCode);
    }
}
