using Finacle.Core.Domain;
using Loans.API.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Finacle.Core
{
    public partial class FinacleClient
    {
        public CreateUnsecuredLoanResponse CreateUnsecuredLoan(CreateUnsecuredLoanRequest unsecuredLoanRequest)
        {
            if (unsecuredLoanRequest == null)
                throw new ArgumentNullException(nameof(unsecuredLoanRequest));

            if (unsecuredLoanRequest.LoanAmount <= 0)
                throw new ArgumentException("Loan amount must be greater than zero.", nameof(unsecuredLoanRequest.LoanAmount));

            if (string.IsNullOrWhiteSpace(unsecuredLoanRequest.OperAccountId) ||
                string.IsNullOrWhiteSpace(unsecuredLoanRequest.CustomerReference)) 
            {
                throw new ArgumentException("Invalid OperAccountId || CustomerReference", nameof(unsecuredLoanRequest));
            }
            if (string.IsNullOrWhiteSpace(unsecuredLoanRequest.AccountCurrency))
            {
                throw new ArgumentException("Invalid AccountCurrency ", nameof(unsecuredLoanRequest));
            }
           

            var requestUri = "api/v1/Loans"; //
            var jsonContent = new StringContent(JsonSerializer.Serialize(unsecuredLoanRequest), Encoding.UTF8, "application/json");

            var response = _httpClient.PostAsync(requestUri, jsonContent).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<CreateUnsecuredLoanResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public UpdateUnsecuredLoanResponse UpdateUnsecuredLoan(UpdateLoanFeesRequestDto updateLoanFeesRequestDto)
        {
            if (updateLoanFeesRequestDto == null)
                throw new ArgumentNullException(nameof(updateLoanFeesRequestDto));

            var requestUri = "api/v1/Loans"; //
            var jsonContent = new StringContent(JsonSerializer.Serialize(updateLoanFeesRequestDto), Encoding.UTF8, "application/json");

            var response = _httpClient.PutAsync(requestUri, jsonContent).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<UpdateUnsecuredLoanResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public DisbursementResponseDto Disburse(DisbursementRequestDTO disbursementRequestDTO)
        {
            if (disbursementRequestDTO == null)
                throw new ArgumentNullException(nameof(disbursementRequestDTO));

            var requestUri = "api/v1/Loans/Disbursement/{accountId}"; //
            var jsonContent = new StringContent(JsonSerializer.Serialize(disbursementRequestDTO), Encoding.UTF8, "application/json");

            var response = _httpClient.PostAsync(requestUri, jsonContent).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<DisbursementResponseDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public PayOffResponseDto PayOff(string accountId, PayoffRequestDTO payoffRequestDTO)
        {
            if (payoffRequestDTO == null)
                throw new ArgumentNullException(nameof(payoffRequestDTO));

            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            var requestUri = "api/v1/Loans/PayOff/{accountId}"; //
            var jsonContent = new StringContent(JsonSerializer.Serialize(payoffRequestDTO), Encoding.UTF8, "application/json");

            var response = _httpClient.PostAsync(requestUri, jsonContent).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<PayOffResponseDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public OutstandingInquiryResponseDTO GetLoanOutstandingInquiry(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            var requestUri = "api/v1/Loans/GetOutstandingInquiry/{accountId}"; //GetOutstandingInquiry/{accountId}

            var response = _httpClient.GetAsync(requestUri).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<OutstandingInquiryResponseDTO>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public DisbursementResponseDto schedulePayment(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));
            var requestUri = "api/v1/Loans/SchedulePayment/{accountId}"; //GetOutstandingInquiry/{accountId}

            var response = _httpClient.GetAsync(requestUri).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<DisbursementResponseDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public  ModifyAccountResponseDto ModifyLoanInterestRate(string accountId,UnsecuredLoanInterestRateDto unsecuredLoanInterestRateDto)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            var requestUri = "api/v1/Loans/InterestDetails/{accountId}"; //GetOutstandingInquiry/{accountId}
            var jsonContent = new StringContent(JsonSerializer.Serialize(unsecuredLoanInterestRateDto), Encoding.UTF8, "application/json");


            var response =  _httpClient.PutAsync(requestUri,jsonContent).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<ModifyAccountResponseDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        public GetInterestDetailsResponse GetInterestDetails(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));
            var requestUri = "api/v1/Loans/InterestDetails/{accountId}"; //GetOutstandingInquiry/{accountId}

            var response = _httpClient.GetAsync(requestUri).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<GetInterestDetailsResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public GetInterestDetailsResponse GetLoanDetails(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));
            var requestUri = "api/v1/Loans/{accountId}"; //GetOutstandingInquiry/{accountId}

            var response = _httpClient.GetAsync(requestUri).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<GetInterestDetailsResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }


    }
}
