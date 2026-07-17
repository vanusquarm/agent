using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Serilog;
using Statement.Web.Helpers;
using Statement.Web.Interfaces;
using Statement.Web.Models;
using System.Configuration;
using System.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Statement.Web.Services
{
    public class MiddlewareClient : IMiddlewareClient
    {
        private readonly ILogger<MiddlewareClient> _logger;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        private readonly string _channelId;
        private readonly string _staffEmpCode;
        private readonly string _chargeFixedAmount;
        private readonly string _StatementTransactionsProcedureName;
        public MiddlewareClient(ILogger<MiddlewareClient> logger, HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;

            _channelId = configuration["FinacleSettings:ChannelId"]!;
            _staffEmpCode = configuration["FinacleSettings:StaffEmpCode"]!;
            _chargeFixedAmount = configuration["FinacleSettings:ChargeFixedAmount"]!;
            _StatementTransactionsProcedureName =
                configuration["FinacleSettings:StatementTransactionsProcedureName"]
                ?? "[dbo].[proc_Statement_Transactions]";

            _connectionString = configuration.GetConnectionString("GTMailConn")!;
        }
        public async Task<List<Account>> GetCustomerCIFAccountsAsync(string customerId, string requestId = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching accounts for {CustomerId} [CorrelationID: {RequestId}]", customerId, requestId);
            try
            {
                //var accessToken = await GetAccessToken();
                //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.AccessToken);

                var allAccounts = new List<Account>();
                int setNumber = 1;
                string isLastSet = "N";


                var queryParams = new { requestId, setNumber };
                var queryString = $"?{ObjectToQueryString(queryParams)}";
                var endpoint = $"api/v1/customers/{customerId}/accounts";

                var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
                request.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

                var response = await _httpClient.SendAsync(request, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("API returned {StatusCode}: {ErrorContent}", response.StatusCode, errorContent);
                    throw new HttpRequestException($"API request failed with status {response.StatusCode}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Raw API response (set {SetNumber}): {JsonResponse}", setNumber, jsonResponse);

                var accountResponse = JsonConvert.DeserializeObject<AccountResponse>(jsonResponse);
                if (accountResponse?.BasicAccountDetails != null)
                {
                    allAccounts.AddRange(accountResponse.BasicAccountDetails);
                }

                var accounts = allAccounts.Count > 0 ? allAccounts : new List<Account>();
                return accounts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch accounts for {CustomerId} [CorrelationID: {RequestId}]", customerId, requestId);
                throw;
            }
        }
        public async Task<AccountCustomerResponse> GetAccountsAndCustomersAsync(string accountId, string requestId = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching account customers for {AccountId} [RequestID: {RequestId}]", accountId, requestId);
            try
            {
                //var accessToken = await GetAccessToken();
                //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.AccessToken);

                var queryParams = new { requestId };

                var queryString = string.IsNullOrEmpty(requestId) ? "" : $"?{ObjectToQueryString(queryParams)}";
                var endpoint = $"api/v1/accounts/{accountId}/customers{queryString}";

                var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
                request.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

                var response = await _httpClient.SendAsync(request, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("API returned {StatusCode}: {ErrorContent}", response.StatusCode, errorContent);
                    throw new HttpRequestException($"API request failed with status {response.StatusCode}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Raw API response: {JsonResponse}", jsonResponse);

                var responseObject = JsonConvert.DeserializeObject<AccountCustomerResponse>(jsonResponse);
                return responseObject;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch account customers for {AccountId} [RequestID: {RequestId}]", accountId, requestId);
                throw;
            }
        }
        public async Task<IntraBankTransferResponse> TransferFunds(string destAccount, string sourceAccount, decimal amount, string remarks, string currency)
        {
            try
            {
                var initiatingUserId = _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?
                    .Value;

                var transferRequest = new IntraBankTransferRequest
                {
                    SourceAccountId = sourceAccount,
                    DestinationAccountId = destAccount,
                    Amount = amount,
                    Currency = currency,
                    Remarks = remarks,
                    ChannelReference = $"{GenerateEightDigitString()}{GetAccountReference(sourceAccount, destAccount)}",
                    ChannelId = _channelId,
                    DocNum = null,
                    UserPartTransactionCode = null,
                    ChargeEventId = null
                };

                var requestId = Guid.NewGuid().ToString();

                using var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    "api/v1/Transactions/transfer");

                request.Headers.Add("RequestId", requestId);
                request.Headers.Add("ChannelId", _channelId);

                request.Content = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(transferRequest),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.SendAsync(request);

                IntraBankTransferResponse? transferResponse = null;

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    transferResponse = System.Text.Json.JsonSerializer.Deserialize<IntraBankTransferResponse>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                }

                var logResponse = await LogTransferTransaction(
                    sourceAccount,
                    destAccount,
                    amount,
                    remarks,
                    currency,
                    transferRequest.ChannelReference,
                    transferResponse,
                    requestId,
                    initiatingUserId);

                if (!logResponse.Success)
                {
                    _logger.LogWarning(
                        "Transfer succeeded but logging failed: {Message}",
                        logResponse.Message);
                }

                return transferResponse ??
                       new IntraBankTransferResponse
                       {
                           TranStatus = "Failure"
                       };
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Transfer request timed out.");

                return new IntraBankTransferResponse
                {
                    TranStatus = "Failure"
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed during transfer.");

                return new IntraBankTransferResponse
                {
                    TranStatus = "Failure"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while transferring funds.");

                return new IntraBankTransferResponse
                {
                    TranStatus = "Failure"
                };
            }
        }
        public async Task<GetRetailCustomerResponse> GetCustomerAsync(string customerId, string correlationId = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching customer {CustomerId} [CorrelationID: {CorrelationId}]", customerId, correlationId);
            try
            {
                //var accessToken = await GetAccessToken();
                //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.AccessToken);

                var queryParams = new { correlationId };
                var queryString = string.IsNullOrEmpty(correlationId) ? "" : $"?{ObjectToQueryString(queryParams)}";
                var endpoint = $"api/v1/customers/{customerId}{queryString}";

                var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
                request.Headers.Add("RequestId", correlationId ?? Guid.NewGuid().ToString());

                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<GetRetailCustomerResponse>(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch customer for {CustomerId} [CorrelationID: {CorrelationId}]", customerId, correlationId);
                throw;
            }
        }
        public async Task<string> FinallyCalculatedStatementCharges(string destinationAccount, string accountNumber, int NoOfPages, string tranAmt, string DebitAccountCurrency)
        {
            //var getChargesRate = await GetEventIdCharge(accountNumber, tranAmt);
            var getChargesRate = _chargeFixedAmount;
            //if (getChargesRate != null && getChargesRate.errorFlg == "0" && getChargesRate.chrgAmt != null)
            {
                //decimal.TryParse(getChargesRate.chrgAmt, out decimal chargeAmount);
                decimal.TryParse(getChargesRate, out decimal chargeAmount);

                var totalCharge = NoOfPages * chargeAmount;
                var chargeCustomer = await TransferFunds(destinationAccount, accountNumber, totalCharge, "Statement Charges", DebitAccountCurrency);
                //return chargeAmount.ToString("F2");
                if (chargeCustomer.TransactionReference != null)
                {
                    return totalCharge.ToString("F2");
                }
                else
                {
                    return "0.00";
                }
            }
        }
        public async Task<bool> IsChargeable(string cif)
        {
            var cust = await GetCustomerAsync(cif);
            var staffEmpCode = _staffEmpCode;

            var empCustomer = cust.Segment == null ? "" : cust.Segment.ToString();

            if (empCustomer != null)
                if (staffEmpCode.Split(',').Contains(empCustomer))
                {
                    return false;
                }
            return true;
        }
        private static string ObjectToQueryString(object obj)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            foreach (var prop in obj.GetType().GetProperties())
            {
                var value = prop.GetValue(obj)?.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    queryString[prop.Name.ToLower()] = value;
                }
            }
            return queryString.ToString();
        }
        private string GenerateEightDigitString()
        {
            var random = new Random();
            var builder = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                builder.Append(random.Next(0, 10));
            }
            return builder.ToString();
        }
        private string GetAccountReference(string sourceAccount, string destinationAccount)
        {
            string sourceStart = sourceAccount.Substring(0, 5);
            string sourceEnd = sourceAccount.Substring(sourceAccount.Length - 5, 5);
            string destinationStart = destinationAccount.Substring(0, 5);
            string destinationEnd = destinationAccount.Substring(destinationAccount.Length - 5, 5);
            return $"{sourceStart}{sourceEnd}{destinationStart}{destinationEnd}";
        }
        public async Task<TransactionLogprocedureResponse> LogTransferTransaction(
          string sourceAccount,string destAccount, decimal amount, string remarks, string currency,
          string channelReference, IntraBankTransferResponse transferResponse,string requestId, string? initiatingUserId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sourceAccount) ||
                    string.IsNullOrWhiteSpace(destAccount) ||
                    amount <= 0 ||
                    string.IsNullOrWhiteSpace(currency))
                {
                    return new TransactionLogprocedureResponse
                    {
                        Success = false,
                        Message = "Source account, destination account, amount and currency are required."
                    };
                }

                var transferRequest = new
                {
                    SourceAccountId = sourceAccount,
                    DestinationAccountId = destAccount,
                    Amount = amount,
                    Currency = currency,
                    Remarks = remarks,
                    ChannelReference = channelReference,
                    RequestId = requestId,
                    InitiatingUserId = initiatingUserId
                };

                var transactionRequestJson =
                    System.Text.Json.JsonSerializer.Serialize(transferRequest);

                var status = transferResponse?.TranStatus ?? "Failure";

                var transactionResponse =
                    transferResponse?.TransactionReference ??
                    "No response received";

                var procedureName =
                    _configuration["DatabaseSettings:TransactionLogProcedureName"]
                    ?? "[dbo].[proc_LogTransaction]";

                var connectionString =
                    _configuration.GetConnectionString("GTMailConn");

                await using var connection =
                    new SqlConnection(connectionString);

                await connection.OpenAsync();

                await using var command =
                    new SqlCommand(procedureName, connection);

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Transaction_Mode", "Insert");
                command.Parameters.AddWithValue("@Information_Type", "Insert-Transaction-Logs");
                command.Parameters.AddWithValue("@TransType", "Making Transfer");
                command.Parameters.AddWithValue("@TransactionRequest",
                    transactionRequestJson ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@TransactionResponse",
                    transactionResponse ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Status", status);
                command.Parameters.AddWithValue("@TransAmount", amount);
                command.Parameters.AddWithValue("@RequestID",
                    requestId ?? (object)DBNull.Value);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    return new TransactionLogprocedureResponse
                    {
                        Success = true,
                        Message = "Transaction logged successfully"
                    };
                }

                const string errorMessage = "Unable to insert transaction log.";

                _logger.LogWarning(
                    "Failed to log transaction. SourceAccount: {SourceAccount}, DestAccount: {DestAccount}, Amount: {Amount}",
                    sourceAccount,
                    destAccount,
                    amount);

                return new TransactionLogprocedureResponse
                {
                    Success = false,
                    Message = errorMessage
                };
            }
            catch (SqlException ex)
            {
                _logger.LogError(
                    ex,
                    "Database error logging transaction. SourceAccount: {SourceAccount}, DestAccount: {DestAccount}, Amount: {Amount}",
                    sourceAccount,
                    destAccount,
                    amount);

                return new TransactionLogprocedureResponse
                {
                    Success = false,
                    Message = "Database error occurred while logging transaction."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error logging transaction. SourceAccount: {SourceAccount}, DestAccount: {DestAccount}, Amount: {Amount}",
                    sourceAccount,
                    destAccount,
                    amount);

                return new TransactionLogprocedureResponse
                {
                    Success = false,
                    Message = "An unexpected error occurred while logging transaction."
                };
            }
        }

        public async Task<BranchAccountProcedureResponse> GetBranchAccountForStatementCharges(string branchCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(branchCode))
                {
                    return new BranchAccountProcedureResponse
                    {
                        Success = false,
                        Message = "Branch code is required."
                    };
                }

                await using var connection = new SqlConnection(_connectionString);

                await connection.OpenAsync();

                await using var command = new SqlCommand(_StatementTransactionsProcedureName, connection);

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Transaction_Mode", "Select");
                command.Parameters.AddWithValue("@Information_Type", "Get-Branch-Account");
                command.Parameters.AddWithValue("@Transaction_String", branchCode);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var accountNumber = reader["AccountNumber"]?.ToString();

                    return new BranchAccountProcedureResponse
                    {
                        Success = true,
                        DestinationAccountNumber = accountNumber,
                        Message = string.IsNullOrWhiteSpace(accountNumber)
                            ? "No account number returned for the specified branch."
                            : "Account retrieved successfully."
                    };
                }

                _logger.LogWarning(
                    "No destination account found for branch {BranchCode}.",
                    branchCode);

                return new BranchAccountProcedureResponse
                {
                    Success = false,
                    Message = "No account found for the specified branch."
                };
            }
            catch (SqlException ex)
            {
                _logger.LogError(
                    ex,
                    "Database error retrieving destination account for branch {BranchCode}.",
                    branchCode);

                return new BranchAccountProcedureResponse
                {
                    Success = false,
                    Message = "Database error occurred while retrieving branch account."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error retrieving destination account for branch {BranchCode}.",
                    branchCode);

                return new BranchAccountProcedureResponse
                {
                    Success = false,
                    Message = "An unexpected error occurred."
                };
            }
        }

    }
}
