using Acccounts.API.Entities.DTOs;
using Refit;

namespace Acccounts.API.Services
{
    public interface IAccountClient
    {
        // Get Account Balance - Example of a Savings Account is 1205001000000471
        [Get("/v2/accounts/{accountId}/balances")] 
        Task<AccountBalanceDto> GetAccountBalance(string accountId);

        [Get("/v2/accounts/{accountId}/transactions")]
        Task<List<TransactionDetailsDto>> GetAccountTrasactions(string accountId);

        // Create Account - Corporate Entities only have Current Accounts

        [Post("/v2/current-accounts")]
        Task<CreateCurrentAcctResponse> CreateCurrentAccount(CreateCurrentAcctRequest createAcctRequest);

        [Post("/v2/savings-accounts")]
        Task<CreateCurrentAcctResponse> CreateSavingsAccount(CreateSavingsAcctRequest createAcctRequest);

        [Post("/v2/corporate-current-accounts")]
        Task<object?> CreateCorpCurrentAccount(CreateCorpCurrentAcctRequest createAcctRequest);

        // Get Account Details - Example of a Savings Account is 1205001000000471 

        [Post("/v2/accntdetails/getaccntdetails")]
        Task<object> GetAccountDetails(object accountId);

        [Get("/v2/savings-accounts/{accountId}/account-details")]
        Task<object> GetSavingsAccount(string accountId);

        [Get("/v2/current-accounts/{accountId}/account-details")]
        Task<object> GetCurrentAccount(string accountId);

        [Get("/v2/corporate-current-accounts/{accountId}/account-details")]
        Task<object> GetCorpCurrentAccount(string accountId);

        // Get Customer Details of an Account
        [Post("/v2/accountCustomerDtls/getaccountCustomerDtls")]
        Task<CustomerDto> GetCustomerDetailsByAccountIdAsync(object accountId);


        // Account Restriction
        [Post("/v2/CustomerAccountfreezeDtls/createCustomerAccountfreezeDtls")]
        Task<object> AddAccountRestriction(FreezeAccountRequest accountId);

        [Post("/v2/CustomerAccountUnfreezeDtls/createCustomerAccountUnfreezeDtls")]
        Task<object> RemoveAccountRestriction(UnFreezeAccountRequest request);

        // Account Statements
        [Post("/v2/acctstmnt/statementDetails")]
        Task<object> GetAccountStatements(AccountStatementRequest request);

        [Post("/v2/TerminalOfficeAcct/getTerminalOfficeAcct")]
        Task<object> GetTerminalOfficeAcct(TerminalOfficeAcctRequest request);

    }
}
