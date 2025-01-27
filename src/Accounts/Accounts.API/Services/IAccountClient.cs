using Acccounts.API.Entities.DTOs;
using Refit;

namespace Acccounts.API.Services
{
    public interface IAccountClient
    {
        [Get("/v2/accounts/{accountId}/balances")] // example CustomerId 35000000055
        Task<AccountBalanceDto> GetAccountBalance(string accountId);

        [Get("/v2/accounts/{accountId}/transactions")]
        Task<List<TransactionDetailsDto>> GetAccountTrasactions(string accountId);

        [Post("/v2/current-accounts")]
        Task<CreateCurrentAcctResponse> CreateCurrentAccount(CreateCurrentAcctRequest createAcctRequest);

        [Post("/v2/savings-accounts")]
        Task<CreateCurrentAcctResponse> CreateSavingsAccount(CreateSavingsAcctRequest createAcctRequest);

        [Get("/v2/savings-accounts/{accountId}/account-details")]
        Task<AccountDto> GetSavingsAccount(string accountId);

        [Get("/v2/current-accounts/{accountId}/account-details")]
        Task<AccountDto> GetCurrentAccount(string accountId);

        // Corporate Entities only have Current Accounts

        [Get("/v2/corporate-current-accounts/{accountId}/account-details")]
        Task<AccountDto> GetCorpCurrentAccount(string accountId);
    }
}
