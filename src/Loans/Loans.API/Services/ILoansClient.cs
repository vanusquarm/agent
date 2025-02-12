using Refit;
using Loans.API.Entities.Models;

namespace Loans.API.Services
{
    public interface ILoanClient
    {
        [Get("/api/fruits")] //specify the finacle resource endpoint
        Task<dynamic> GetLoans();
    }

    // Move to Entities/DTOs folder
}
