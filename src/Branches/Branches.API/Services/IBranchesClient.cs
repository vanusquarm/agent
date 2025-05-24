using Branches.API.Entities.DTOs;

namespace Branches.API.Services;
using Refit;

public interface IBranchesClient
{
    [Get("/v2/branches-details")]
    Task<BranchDetailsResponseDto> GetBranchDetails();
    [Get("/v2/branches")]
    Task<BranchDetailsDto> GetBranchDetailsByCode([Query] string branchCode, [Query] string bankCode);
    [Get("/v2/customers/{customerId}/accounts")]
    Task<BranchListResponseDto> GetCustomerAccounts(
        string customerId,
        [Query] string includeClosedAccounts,
        [Query] int setNumber,
        [Query] int maximumRecords
    );

    [Post("/v2/getChq/getHolidayStatusDtls")]
    Task<HolidayStatusResponse> GetHoliday(object date);
}