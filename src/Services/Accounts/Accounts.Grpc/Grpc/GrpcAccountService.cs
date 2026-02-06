//using Accounts.API.Entities;
//using Accounts.API.Entities.DTOs;
//using Accounts.API.Services;
//using Accounts.API.Validators;
//using Finacle.Infrastructure.Logging;
//using FluentValidation;
//using Grpc.Core;
//using Microsoft.AspNetCore.Mvc;
//using Refit;
//using Serilog;
//using System.Text.Json;


//namespace Accounts.API.Grpc
//{
//    public class GrpcAccountService(IAccountClient accountClient, AccountValidator accountValidator, ILogger<GrpcAccountService> logger)
//    {
//        /// <summary>
//        /// Async Method to Get Account
//        /// </summary>
//        /// <param name="accountId"></param>
//        /// <returns></returns>

//        public override async Task<GetAccountResponse> GetAccount(GetAccountRequest request, ServerCallContext context)
//        {
//            logger.LogInformation("gRPC GetAccount: {AccountId}", request.AccountId);

//            var validationResult = await _accountValidator.ValidateAsync(request.AccountId);
//            if (!validationResult.IsValid)
//            {
//                return new GetAccountResponse { Success = false, Error = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)) };
//            }

//            try
//            {
//                var details = await accountClient.GetAccountDetails(new { accountId = request.AccountId });
//                // Map details to AccountDetails message
//                return new GetAccountResponse
//                {
//                    Success = true,
//                    Details = new AccountDetails
//                    {
//                        AccountId = request.AccountId,
//                        // Map other fields as needed
//                    }
//                };
//            }
//            catch (Exception ex)
//            {
//                logger.LogError(ex, "Error in GetAccount");
//                return new GetAccountResponse { Success = false, Error = "Internal error" };
//            }
//        }


//        /// <summary>
//        /// Async Method to Get Account Statements
//        /// </summary>
//        /// <param name="accountId"></param>
//        /// <param name="request"></param>
//        /// <returns></returns>
//        public async Task<IApiResponse> GetAccountStatements([FromRoute] string accountId, [FromQuery] AccountStatementRequest request)
//        {
//            logger.LogInformation("Recieved Request at GetAccountStatementsAsync: {@Request}", request);

//            var validationResult = await accountValidator.ValidateAsync(request);
//            if (!validationResult.IsValid)
//            {
//                logger.LogInformation("Account statement request payload is invalid: {@Request}", request);
//                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
//                logger.LogError("Back from getting account statements with response: {@Response}", ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
//                throw new ValidationException(string.Join("; ", errors));
//            }

//            try
//            {
//                var json = JsonSerializer.Serialize(request);
//                var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
//                dict["accountId"] = accountId;

//                logger.LogInformation("About to get account statements with request: {@Request}", dict);
//                var accountDetails = await accountClient.GetAccountStatements(dict);
//                logger.LogInformation("Back from getting account statements with response: {@Response}", accountDetails);

//                if (accountDetails == null)
//                {
//                    logger.LogInformation("Account statements could not be retrived for: {Account}", accountId);
//                    return new ResponseManager.ApiResponse("No Account Statement found", "", "", "", "");
//                }
//                return Ok(accountDetails);
//            }
//            catch (RpcException ex)
//            {
//                logger.LogError("Back from getting account statements with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
//                return ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Trailers);
//            }
//            catch (Exception ex)
//            {
//                logger.LogError("An exception has occured trying to get Account Statements with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
//                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
//            }
//        }

//        /// <summary>
//        /// Async Method to Get Account Statement
//        /// </summary>
//        /// <param name="accountId"></param>
//        /// <param name="request"></param>
//        /// <returns></returns>
//        public async Task<IApiResponse> GetAccountStatement([FromRoute] string accountId, [FromBody] AccountStatementRequest request)
//        {
//            logger.LogInformation("Recieved Request at GetAccountStatementAsync: {@Request}", request);

//            var validationResult = await accountValidator.ValidateAsync(request);
//            if (!validationResult.IsValid)
//            {
//                logger.LogInformation("Account statement request payload is invalid: {Request}", request);
//                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
//                logger.LogError("Back from getting account statements with response: {@Response}", ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
//                return new ApiResponse<object>(
//                    new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
//                    {
//                        Content = new StringContent(string.Join("; ", errors))
//                    },
//                    null,
//                    false
//                );
//            }
//            try
//            {
//                logger.LogInformation("About to get account statements with request: {@Request}", new { accountId, fromDate = request.FromDate, toDate = request.ToDate });
//                var accountDetails = await accountClient.GetAccountStatement(new { accountId, fromDate = request.FromDate, toDate = request.ToDate });
//                logger.LogInformation("Back from getting account statements with response: {@Response}", accountDetails);

//                if (accountDetails == null)
//                {
//                    logger.LogInformation("Account statements could not be retrived for: {Account}", accountId);
//                    return Ok("No Account Statement found");
//                }
//                return accountDetails;
//            }
//            catch (RpcException ex)
//            {
//                logger.LogError("An exception has occured trying to get Account Statements with message: {Message} and stack trace: {StackTrace}", ex.Message, ex.StackTrace);
//                return ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Trailers);
//            }
//        }

//        /// <summary>
//        /// Async Method to Add Account Restriction
//        /// </summary>
//        /// <param name="request"></param>
//        /// <returns></returns>
//        public async Task<IApiResponse> AddAccountRestriction(FreezeAccountRequest request)
//        {
//            logger.LogInformation("Recieved Request at AddAccountRestrictionAsync: {@Request}", request);

//            var validationResult = await accountValidator.ValidateAsync(request);
//            if (!validationResult.IsValid)
//            {
//                logger.LogInformation("Account restriction request payload is invalid: {@Request}", request);
//                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
//                logger.LogError("Back from Adding Account Restriction with response: {@Response}", ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
//                throw new ValidationException(string.Join("; ", errors));
//            }
//            try
//            {
//                logger.LogInformation("Recieved Request: {@Request}", request);
//                logger.LogInformation("About to Add Account Restriction with request: {@Request}", request);
//                var accountDetails = await accountClient.AddAccountRestriction(request);
//                logger.LogInformation("Back from Adding Account Restriction with response: {@Response}", accountDetails);

//                if (accountDetails == null)
//                {
//                    logger.LogInformation("Account could not be restricted: {Account}", request.AccountNumber);
//                    return Ok("Account Restriction Not Successful");
//                }
//                return accountDetails;
//            }
//            catch (RpcException ex)
//            {
//                logger.LogError("Back from Adding Account Restriction with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
//                return ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Trailers);
//            }
//        }

//        /// <summary>
//        /// Async Method to Remove Account Restriction
//        /// </summary>
//        /// <param name="request"></param>
//        /// <returns></returns>
//        public async Task<IApiResponse> RemoveAccountRestriction(UnFreezeAccountRequest request)
//        {
//            logger.LogInformation("Recieved Request at RemoveAccountRestrictionAsync: {@Request}", request);

//            var validationResult = await accountValidator.ValidateAsync(request);
//            if (!validationResult.IsValid)
//            {
//                logger.LogInformation("Account restriction removal request payload is invalid: {@Request}", request);
//                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
//                logger.LogError("Back from Removing Account Restriction with response: {@Response}", ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
//                return ResponseManager.ReturnFailedValidations(string.Join("; ", errors));
//            }
//            try
//            {
//                logger.LogInformation("Recieved Request: {@Request}", request);
//                logger.LogInformation("About to Remove Account Restriction with request: {@Request}", request);
//                var accountDetails = await accountClient.RemoveAccountRestriction(request);
//                logger.LogInformation("Back from Removing Account Restriction with response: {@Response}", accountDetails);

//                if (accountDetails == null)
//                {
//                    logger.LogInformation("Removing Account Restriction failed: {Account}", request.CustomerId);
//                    return Ok("Account Restriction Removal Not Successful");
//                }
//                return accountDetails;
//            }
//            catch (RpcException ex)
//            {
//                logger.LogError("Back from Removing Account Restriction with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
//                return ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Trailers));
//            }
//        }

//        /// <summary>
//        /// Async Method Get Customer Details By AccountId
//        /// </summary>
//        /// <param name="accountId"></param>
//        /// <returns></returns>
//        public async Task<IApiResponse> GetAccountCustomers(string accountId)
//        {
//            logger.LogInformation("Recieved Request at GetCustomerDetailsByAccountIdAsync: {Request}", accountId);

//            var validationResult = await accountValidator.ValidateAsync(accountId);
//            if (!validationResult.IsValid)
//            {
//                logger.LogInformation("Account number is invalid: {Request}", accountId);
//                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
//                logger.LogError("Back from Getting Customer Details By AccountId  with response: {@Response}", ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
//                return ResponseManager.ReturnFailedValidations(string.Join("; ", errors));
//            }
//            try
//            {
//                logger.LogInformation("Recieved Request: {Request}", accountId);
//                logger.LogInformation("About to Get Customer Details By AccountId with request: {Request}", accountId);
//                var customerDetails = await accountClient.GetCustomerDetailsByAccountIdAsync(new { accountNumber = accountId });
//                logger.LogInformation("Back from Getting Customer Details By AccountId  with response: {@Response}", customerDetails);

//                if (customerDetails == null)
//                {
//                    logger.LogInformation("Get Customers Details By AccountId  failed: {Account}", accountId);
//                    return NotFound();
//                }
//                return Ok(customerDetails);
//            }
//            catch (RpcException ex)
//            {
//                logger.LogError("Back from Getting Customer Details By AccountId  with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
//                return ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Trailers));
//            }
//        }

//        /// <summary>
//        /// Method to Get Account Balances
//        /// </summary>
//        /// <param name="accountId"></param>
//        /// <param name="accountType"></param>
//        /// <returns></returns>
//        public async Task<IApiResponse> GetAccountBalances([FromRoute] string accountId, [FromQuery] string? accountType)
//        {
//            logger.LogInformation("Recieved Request at GetAccountBalances: {Request}", accountId);

//            var validationResult = await accountValidator.ValidateAsync(accountId);
//            if (!validationResult.IsValid)
//            {
//                logger.LogInformation("Account number is invalid: {Request}", accountId);
//                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
//                logger.LogError("Back from Getting AccountBalances with response: {@Response}", ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
//                return ResponseManager.ReturnFailedValidations(string.Join("; ", errors));
//            }
//            try
//            {
//                logger.LogInformation("Recieved Request: {Request}", accountId);
//                // Validate accountType using the enum
//                if (string.IsNullOrEmpty(accountType)) { accountType = null; }
//                else
//                {
//                    if (!Enum.IsDefined(typeof(AccountType), accountType))
//                    {
//                        logger.LogError("Back from Getting AccountBalances with response: {@Response}", ResponseManager.ReturnFailedValidations(
//                            $"Invalid account type: {accountType}. Please provide a valid account type."));
//                        return ResponseManager.ReturnFailedValidations($"Invalid account type: {accountType}. Please provide a valid account type.");
//                    }
//                }

//                logger.LogInformation("About to GetAccountBalances with request: {Request}", accountId);
//                var balances = await accountClient.GetAccountBalances(accountId);
//                logger.LogInformation("Back from Getting AccountBalances with response: {@Response}", balances);

//                // Check if the balances object is null or does not contain any balances
//                if (balances == null || balances.Balances == null || !balances.Balances.Any())
//                {
//                    logger.LogInformation("No balances found for the specified account for: {Account}", accountId);
//                    return NotFound("No balances found for the specified account.");
//                }

//                //filter for a specific account here
//                // Look for the specific balance matching the accountType
//                var balance = balances.Balances.FirstOrDefault(b => b.BalanceType == accountType);
//                if (balance != null) { return Ok(balance); }

//                // If no balance matches the accountType, return all balances
//                return Ok(balances);
//            }
//            catch (RpcException ex)
//            {
//                logger.LogError("Back from Getting AccountBalances with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
//                return ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Trailers);
//            }
//        }

//        /// <summary>
//        /// Method to Get Account Balance
//        /// </summary>
//        /// <param name="accountId"></param>
//        /// <param name="date"></param>
//        /// <returns></returns>
//        public async Task<IApiResponse> GetAccountBalance([FromRoute] string accountId, [FromQuery] string? date = null)
//        {
//            logger.LogInformation("Recieved Request at GetAccountBalance: {Request}", accountId);

//            var validationResult = await accountValidator.ValidateAsync(accountId);
//            if (!validationResult.IsValid)
//            {
//                logger.LogInformation("Account number is invalid: {Request}", accountId);
//                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
//                logger.LogError("Back from Getting AccountBalance with response: {@Response}", ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
//                return ResponseManager.ReturnFailedValidations(string.Join("; ", errors));
//            }
//            try
//            {

//                date ??= DateTime.UtcNow.ToString("yyyy-MM-dd");
//                var request = new { accountNumber = accountId, date };
//                logger.LogInformation("About to GetAccountBalance with request: {@Request}", request);
//                var balance = await accountClient.GetAccountBalance(request);
//                logger.LogInformation("Back from Getting AccountBalance with response: {@Response}", balance);

//                if (balance == null)
//                {
//                    logger.LogInformation("No balance found on {date} for the specified account", date);
//                    return NotFound($"No balance found on {date} for the specified account");
//                }
//                return Ok(balance);
//            }
//            catch (RpcException ex)
//            {
//                logger.LogError("Back from Getting AccountBalance with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
//                return ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Trailers);
//            }
//        }

//        /// <summary>
//        /// Method to Create Savings Account
//        /// </summary>
//        /// <param name="savingsAcct"></param>
//        /// <returns></returns>
//        public async Task<IApiResponse> CreateSavingsAccount([FromBody] CreateSavingsAcctRequest savingsAcct)
//        {
//            logger.LogInformation("Recieved Request at CreateSavingsAccount: {@Request}", savingsAcct);
//            try
//            {
//                logger.LogInformation("About to CreateSavingsAccount with request: {@Request}", savingsAcct);
//                var createSavingsAccountResponse = await accountClient.CreateSavingsAccount(savingsAcct);
//                logger.LogInformation("Back from Creating SavingsAccount with response: {@Response}", savingsAcct);
//                return Ok(createSavingsAccountResponse);
//            }
//            catch (RpcException ex)
//            {
//                logger.LogError("Back from Creating SavingsAccount with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
//                return ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Trailers);
//            }
//        }

//        /// <summary>
//        /// Method to Get Terminal Office Account
//        /// </summary>
//        /// <param name="TerminalId"></param>
//        /// <returns></returns>
//        public async Task<IApiResponse> GetTerminalOfficeAcct([FromBody] TerminalOfficeAcctRequest request)
//        {
//            logger.LogInformation("About to GetTerminalOfficeAcct with request: {@Request}", request);

//            var validationResult = await accountValidator.ValidateAsync(request);
//            if (!validationResult.IsValid)
//            {
//                logger.LogInformation("Terminal office account request payload is invalid: {@Request}", request);
//                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
//                logger.LogError("Back from Getting TerminalOfficeAcct with response: {@Response}", ResponseManager.ReturnFailedValidations(string.Join("; ", errors)));
//                return ResponseManager.ReturnFailedValidations(string.Join("; ", errors));
//            }
//            try
//            {
//                var GetTerminalOfficeAcctResponse = await accountClient.GetTerminalOfficeAccount(request);
//                logger.LogInformation("Back from Getting TerminalOfficeAcct with response: {@Response}", GetTerminalOfficeAcctResponse);
//                return Ok(GetTerminalOfficeAcctResponse);
//            }
//            catch (RpcException ex)
//            {
//                logger.LogError("Back from Getting TerminalOfficeAcct with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
//                return ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Trailers);
//            }

//        }

//        /// <summary>
//        /// Async method to Get Account Liens
//        /// </summary>
//        /// <param name="accountId"></param>
//        /// <param name="lienType"></param>
//        /// <returns></returns>
//        public async Task<LienResponseDto> GetAccountLiens([FromRoute] string accountId, [FromRoute] string lienType)
//        {
//            logger.LogInformation("Recieved Request at GetAccountLiensAsync: {@Request}", (accountId, lienType));
//            try
//            {
//                logger.LogInformation("About to GetAccountLiens with request: {Request}", accountId);
//                var accountDetails = await accountClient.GetAccountLiens(accountId, lienType);
//                logger.LogInformation("Back from Getting AccountLiens with response: {@Response}", accountDetails);

//                if (accountDetails == null)
//                {
//                    logger.LogInformation("No Account Lien found for:{Account}", accountId);
//                    return Ok("No Account Lien found");
//                }

//                return Ok(accountDetails);
//            }
//            catch (RpcException ex)
//            {
//                logger.LogError("Back from Getting AccountLiens with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
//                return ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Trailers);
//            }
//        }

//        /// <summary>
//        /// Async Method to Create Account Liens
//        /// </summary>
//        /// <param name="accountId"></param>
//        /// <param name="request"></param>
//        /// <returns></returns>
//        public async Task<IApiResponse> CreateAccountLiens([FromRoute] string accountId, [FromBody] LienRequestDto request)
//        {
//            logger.LogInformation("Recieved Request at CreateAccountLiensAsync: {@Request}", (accountId, request));
//            try
//            {
//                logger.LogInformation("About to CreateAccountLiens with request: {@Request}", request);
//                var accountDetails = await accountClient.CreateAccountLiens(accountId, request);
//                logger.LogInformation("Back from Creating AccountLiens with response: {@Response}", accountDetails);

//                if (accountDetails == null)
//                {
//                    logger.LogInformation("Account Lien creation failed:{Account}", accountDetails);
//                    return Ok("No Account Lien creation failed");
//                }

//                return Ok(accountDetails);
//            }
//            catch (RpcException ex)
//            {
//                logger.LogError("Back from Creating AccountLiens with response: {@Response}", ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Content));
//                return ResponseManager.ReturnFinacleCallUnsuccessful(exception: ex.Trailers);
//            }
//        }
//    }
//}
