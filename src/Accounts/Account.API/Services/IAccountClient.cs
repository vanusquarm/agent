using Acccount.API.Entities.DTOs;
using Refit;

namespace Acccount.API.Services
{
    public interface IAccountClient
    {
        [Get("/accounts/{accountId}/balances")] // example CustomerId 35000000055
        Task<AccountBalanceDto> GetAccountBalance(string accountId);

        [Get("/accounts/{accountId}/transactions")]
        Task<List<TransactionDetailsDto>> GetAccountTrasactions(string accountId);
    }
}
