using Cheques.API.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Refit;


namespace Cheques.API.Services
{
    public interface IChequesClient
    {
        [Post("/corporate-current-accounts/{accountIdentification}/:issueChequeBook")]
        Task<CheckBookIssueResponseDto> IssueChequeBookAsync( ChequeBookIssueRequestDto accountIdentification);


        [Get("/corporate-current-accounts/{accountIdentification}/checkStatus?chequeNumber={chequeNumber}")]
        Task<ChequeStatusResponseDto> GetChequeStatusAsync([AliasAs("accountIdentification")] string accountIdentification, [Query] string chequeNumber);


        [Post("/corporate-current-accounts/{accountIdentification}/:stopCheque")]
        Task<StopChequeResponseDto> StopChequeBookAsync([AliasAs("accountIdentification")] string accountIdentification, [Body]StopChequeRequestDto requestDto);

        [Post("/corporate-current-accounts/{accountIdentification}/:revokeStopCheque")]
        Task<RevokeChequeResponseDto> RevokeStopChequeBookAsync([AliasAs("accountIdentification")] string accountIdentification, RevokeChequeRequestDto requestDto);

    }
}
