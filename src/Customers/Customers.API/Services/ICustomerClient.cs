using Customers.API.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace Customers.API.Services
{
    public interface ICustomerClient
    {
        // Corporate Customer
        [Post("/corporate-customers")] 
        Task<CreateCorpCustomerResponse> CreateCorporateCustomer(CreateCorpCustomerRequest createCCRequest);

        [Get("/corporate-customers/{customerId}")] 
        Task<GetCorpCustomerResponse> GetCorporateCustomer(string customerId);

        [Get("/corporate-customers/{customerId}/{prop}")]
        Task<dynamic> GetCorporateCustomer(string customerId, string prop);

        // Retail Customer
        [Post("/retail-customers")] 
        Task<CreateRetailCustomerResponse> CreateRetailCustomer(CreateRetailCustomerRequest createRCRequest);

        // Retail Customer
        [Post("/retail-customers")]
        Task<CreateRetailCustomerResponseDto2> CreateRetailCustomer2(CreateRetailCustomerRequestDto2 createRCRequest);

        [Get("/retail-customers/{customerId}")]
        Task<GetRetailCustomerResponse> GetRetailCustomer(string customerId);

        [Get("/retail-customers/{customerId}/{prop}")]
        Task<dynamic> GetRetailCustomer(string customerId, string prop);


        // All Customers
        [Post("/v2/CustomerDtls/getCustomerDtls")]
        Task<CustomerDetailsResponse> GetCustomerDetailsByPhoneAsync([Body] Dictionary<string, string> request);

        [Get("/v2/customers/{customerId}/accounts?includeClosedAccounts={includeClosedAccounts}")]
        Task<GetCustomerAcctsResponse> GetCustomerAccounts(string customerId, string includeClosedAccounts);

        [Get("/customers/{customerId}/accounts?includeClosedAccounts={includeClosedAccounts}")]
        Task<object> GetCustomerAccountsStatements(string customerId, string includeClosedAccounts);

        [Get("/v2/customers/{customerId}/account-balances?includeClosedAccounts=Y")]
        Task<object> GetCustomerAccountsBalances(string customerId);

        [Get("/accounts/{accountId}/transactions")]
        Task<object> GetCustomerAccountTransactions(string accountId);
        [Post("/v2/StandingInstructionInquiryDtls/getStandingInstructionInquiryDtls")]
        Task<GetStandingIstructionsResponse> GetCustomerStandingInstructions(GetStandingInstructionsRequest request);
        [Post("/v2/standing-instructions/Fixed")]
        Task<PostCustomerStandingInstructionsResponse> PostCustomerStandingInstructions(PostCustomerStandingInstructionsRequest request);

        [Post("/v2/deleteSI/getDeleteSIDtls")]
        Task<CancelCustomerStandingInstructionResponse> DeleteCustomerStandingInstruction(CancelCustomerStandingInstructionRequest request);

        // Loan Account Inquiry
        [Get("/v2/LoanAccountInquiryDtls/getLoanAccountInquiryDtls")]
        Task<object> GetCustomerLoan([Body] Dictionary<string, string> request);

        // Teller Transactions
        [Post("/v2/TellerTranDtls/getTellerTranDtls")]
        Task<object> GetTellerTransactionDetails(object request);

        [Post("/v2/tellerAccountDtls/gettellerAccountDtls")]
        Task<TellerAccountDetails> GetTellerAccountDetails(object request);
    }
}
