using Finacle.Core.Domain;
using Loans.API.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Finacle.Core
{
    public partial class FinacleClient
    {
        public CreateUnsecuredLoanResponse CreateUnsecuredLoan(CreateUnsecuredLoanRequest request, string requestId = null)
        {
            var url = "/api/v1/Loans?isSecured=false";  // Append the query string

            try
            {
                var jsonContent = JsonSerializer.Serialize(request);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = _httpClient.PostAsync(url, httpContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    return JsonSerializer.Deserialize<CreateUnsecuredLoanResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    var errorContent = response.Content.ReadAsStringAsync().Result;
                    throw new HttpRequestException($"Error creating LOAN. Status: {response.StatusCode}, Response: {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"HTTP Request error: {ex.Message}");
            }
            catch (JsonException ex)
            {
                throw new Exception($"JSON Serialization/Deserialization error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred: {ex.Message}");
            }

         
        }

        public async Task<CreateUnsecuredLoanResponse> CreateUnsecuredLoanAsync(CreateUnsecuredLoanRequest loanRequest, string requestId = null)
        {
            var url = $"/api/v1/Loans?isSecured=false";  // Append the query string

            // Serialize the request object to JSON using System.Text.Json
            var content = new StringContent(JsonSerializer.Serialize(loanRequest), Encoding.UTF8, "application/json");

            // Send the POST request
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                // Parse the response and return it
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<CreateUnsecuredLoanResponse>(responseData);  // Deserialize the response body
            }
            else
            {
                // Handle error response
                throw new HttpRequestException($"Error occurred while creating loan: {response.ReasonPhrase}");
            }
        }

        public UpdateUnsecuredLoanResponse UpdateUnsecuredLoan(UpdateLoanFeesRequestDto updateLoanFeesRequestDto, string requestId = null)
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

        public DisbursementResponseDto Disburse(string accountId, DisbursementRequestDTO disbursementRequestDTO, string requestId = null)
        {
            if (disbursementRequestDTO == null)
                throw new ArgumentNullException(nameof(disbursementRequestDTO));

            var requestUri = $"api/v1/Loans/Disbursement/{accountId}"; //
            var jsonContent = new StringContent(JsonSerializer.Serialize(disbursementRequestDTO), Encoding.UTF8, "application/json");

            var response = _httpClient.PostAsync(requestUri, jsonContent).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<DisbursementResponseDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public PayOffResponseDto PayOff(string accountId, PayoffRequestDTO payoffRequestDTO, string requestId = null)
        {
            if (payoffRequestDTO == null)
                throw new ArgumentNullException(nameof(payoffRequestDTO));

            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            var requestUri = $"api/v1/Loans/PayOff/{accountId}"; //
            var jsonContent = new StringContent(JsonSerializer.Serialize(payoffRequestDTO), Encoding.UTF8, "application/json");

            var response = _httpClient.PostAsync(requestUri, jsonContent).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<PayOffResponseDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public AccountInquiryDetailsResponseDto GetLoanOutstandingInquiry(string customerId, string requestId = null)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ArgumentNullException(nameof(customerId));

            var requestUri = $"api/v1/Loans/GetOutstandingInquiry/{customerId}"; //GetOutstandingInquiry/{accountId}

            var response = _httpClient.GetAsync(requestUri).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<AccountInquiryDetailsResponseDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public DisbursementResponseDto schedulePayment(string accountId, string requestId = null)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));
            var requestUri = $"api/v1/Loans/SchedulePayment/{accountId}"; //GetOutstandingInquiry/{accountId}

            var response = _httpClient.GetAsync(requestUri).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<DisbursementResponseDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public  ModifyAccountResponseDto ModifyLoanInterestRate(string accountId,UnsecuredLoanInterestRateDto unsecuredLoanInterestRateDto, string requestId = null)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            var requestUri = $"api/v1/Loans/InterestDetails/{accountId}"; //GetOutstandingInquiry/{accountId}
            var jsonContent = new StringContent(JsonSerializer.Serialize(unsecuredLoanInterestRateDto), Encoding.UTF8, "application/json");


            var response =  _httpClient.PutAsync(requestUri,jsonContent).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<ModifyAccountResponseDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        public GetInterestDetailsResponse GetInterestDetails(string accountId, string requestId = null)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));
            var requestUri = $"api/v1/Loans/InterestDetails/{accountId}"; //GetOutstandingInquiry/{accountId}

            var response = _httpClient.GetAsync(requestUri).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<GetInterestDetailsResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public FullAccountResponseDetailDto GetLoanDetails(string loanId, string requestId = null)
        {
            if (string.IsNullOrEmpty(loanId))
                throw new ArgumentNullException(nameof(loanId));
            var requestUri = $"api/v1/Loans/unsecuredLoan/{loanId}"; //GetOutstandingInquiry/{accountId}

            var response = _httpClient.GetAsync(requestUri).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<FullAccountResponseDetailDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }


    }
}
