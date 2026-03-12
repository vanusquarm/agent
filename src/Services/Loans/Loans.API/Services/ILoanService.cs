using Loans.API.Entities.DTOs;

namespace Loans.API.Services
{
    public interface ILoanService
    {
        Task<EligibilityResponse> GetEligibilityAsync(int braCode, string cusNum);
        Task<ManualQuickCreditResponse> ProcessManualQuickCreditAsync(ManualQuickCreditRequest request);
        Task<QuickCreditHRUndertakingResponse> ProcessHRUndertakingAsync(QuickCreditHRUndertakingRequest request);
    }
}
