using Refit;
using Loans.API.Entities.Models;
using System.Threading.Tasks;
using Loans.API.Entities.DTOs;

namespace Loans.API.Services
{
    public interface ILoanClient
    {
        // UnSecured Loan (example: quick loans, personal loans)
        // Diburse loan by crediting customer's account
        [Headers("requesttype: SRV","delivery_channel:API")]
        [Post("/v2/unsecured-loan-accounts/{accountId}/DISBURSEMENT")]         
        Task<dynamic> DiburseLoan(string accountId, DisbursementRequestDTO disburseRequestDTO);

        // 
        [Get("/v2/unsecured-loan-accounts/{accountIdentification}/interest-details  ")]
        Task<dynamic> GetUnsecuredLoanInterest(string accountIdentification);

        [Get("/v2/unsecured-loan-accounts/{accountIdentification}/interest-details  ")]
        Task<dynamic> ModifyUnsecuredLoanInterest(string accountIdentification, UnsecuredLoanInterestRateDto unsecuredLoanInterestRateDto);

        [Get("/v2/unsecured-loan-accounts/{accountIdentification}/basic")]
        Task<FullAccountResponseDetailDto> GetLoanDetails(string accountIdentification);


        [Post("/v2/LoanAccountInquiryDtls/getLoanAccountInquiryDtls")]
        Task<AccountInquiryDetailsResponseDto> GetOutStandingIniquiry([Body] Dictionary<string, string> request);

        [Get("/v2/unsecured-loan-accounts/{accountIdentification}/repayment-history")] 
        Task<dynamic> GetLoans(string accountIdentification);

        [Headers("requesttype: SRV", "delivery_channel:API")]
        [Post("/v2/unsecured-loan-accounts/{accountIdentification}:payoff")]
        Task<PayOffResponseDto> PayOffLoan(string accountIdentification,PayoffRequestDTO payoffRequestDTO);

        [Post("/v2/unsecured-loan-accounts/{accountIdentification}:schedulePayment")]
        Task<dynamic> SchedulePayment(string accountIdentification, SchedulePaymentDTO schedulePaymentDTO);

        [Put("/v2/unsecured-loan-accounts/{accountIdentification}/interest-details")]
        Task<dynamic> UpdateLoan(string accountIdentification);

        // Secured Loan (Backed by Collateral)
        [Post("/v2/unsecured-loan-accounts/{accountId}/DISBURSE")]
        Task<dynamic> DiburseSecuredLoan(string accountId);

        [Post("/v2/unsecured-loan-accounts")]
        Task<object> CreateUnsecureLoan(CreateUnsecuredLoanRequest createUnsecuredLoanRequest);

        [Post("/v2/secured-loan-accounts")]
        Task<object> CreateSecureLoan(CreateUnsecuredLoanRequest createUnsecuredLoanRequest);

        [Put("/v2/unsecured-loan-accounts/{accountId}")]
        Task<dynamic> UpdateUnsecureLoan(string accountId, UpdateLoanFeesRequestDto updateLoanFeesRequestDto);

        [Get("/v2/unsecured-loan-accounts/{accountIdentification}")]
        Task<dynamic> GetSecuredLoanDetails(string accountIdentification);

        [Get("/v2/unsecured-loan-accounts/{accountIdentification}/repayment-history")]
        Task<dynamic> GetSecuredLoans(string accountIdentification);

        [Post("/v2/unsecured-loan-accounts/{accountIdentification}:close")]
        Task<dynamic> PayOffSecuredLoan(string accountIdentification);


    }
}
