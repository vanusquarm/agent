using Acccount.API.Entities.DTOs;
using Refit;

namespace Accounts.API.Services
{
    public interface IAccountClient
    {
        [Get("/accounts/{accountId}/balances")] // example CustomerId 35000000055
        Task<GetAccountBalanceResponse> GetAccountBalance(string accountId);

        [Get("/accounts/{transactionId}/transactions")] 
        Task<List<TransactionDetailsDto>> GetAccountTrasactions(string transactionId);
    }
}
