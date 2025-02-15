using Refit;
using Loans.API.Entities.Models;

namespace Loans.API.Services
{
    public interface ILoanClient
    {
        // UnSecured Loan (example: quick loans, personal loans)
        // Diburse loan by crediting customer's account
        [Post("/v2/unsecured-loan-accounts/{accountId}/DISBURSEMENT")]         
        Task<dynamic> DiburseLoan(string accountId);

        [Get("/v2/unsecured-loan-accounts/{accountIdentification}")]
        Task<dynamic> GetLoanDetails(string loanId);

        [Get("/v2/unsecured-loan-accounts/{accountIdentification}/repayment-history")] 
        Task<dynamic> GetLoans();

        [Post("/v2/unsecured-loan-accounts/{accountIdentification}:close")]
        Task<dynamic> PayOffLoan(string loanId);

        [Put("/v2/unsecured-loan-accounts/{accountIdentification}/interest-details")]
        Task<dynamic> UpdateLoan(string loanId);


        // Secured Loan (Backed by Collateral)

        [Post("/v2/unsecured-loan-accounts/{accountId}/DISBURSEMENT")]
        Task<dynamic> DiburseSecuredLoan(string accountId);

        [Get("/v2/unsecured-loan-accounts/{accountIdentification}")]
        Task<dynamic> GetSecuredLoanDetails(string loanId);

        [Get("/v2/unsecured-loan-accounts/{accountIdentification}/repayment-history")]
        Task<dynamic> GetSecuredLoans();

        [Post("/v2/unsecured-loan-accounts/{accountIdentification}:close")]
        Task<dynamic> PayOffSecuredLoan(string loanId);

        [Put("/v2/unsecured-loan-accounts/{accountIdentification}/interest-details")]
        Task<dynamic> UpdateSecuredLoan(string loanId);
    }
}
