using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Statement.Web.Interfaces;
using Statement.Web.Models;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Statement.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly IBasisFallbackService _basisFallbackService;
        private readonly IStatementClient _statementClient;
        private readonly IMiddlewareClient _middlewareClient;
        private readonly IConfiguration _configuration;

        public ProfileController(ILogger<ProfileController> logger, IBasisFallbackService basisFallbackService, IStatementClient statementClient, IMiddlewareClient middlewareClient, IConfiguration configuration)
        {
            _logger = logger;
            _basisFallbackService = basisFallbackService;
            _statementClient = statementClient;
            _middlewareClient = middlewareClient;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> NewCustomer(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber))
            {
                return PartialView("_NewCustomer", new NewCustomerDto());
            }

            var customerData = await DetermineCustomerAccount(accountNumber);
            if (customerData == null)
            {
                ViewBag.Error = "Account not found.";
                return PartialView("_NewCustomer", new NewCustomerDto());
            }

            var newCustomerNo = customerData.Accounts?.FirstOrDefault()?.AccountIdentification;
            if (!string.IsNullOrEmpty(newCustomerNo))
            {
                customerData.CustomerNumber = GetLastSixCharForCustomerNumber(newCustomerNo);
            }

            return PartialView("_NewCustomer", customerData);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveNewCustomer(NewCustomerDto model)
        {
            try
            {
                var currentUser = HttpContext.Session.GetString("user_name") ??
                                  User.Identity?.Name ??
                                  "System";

                model.Createdby = currentUser;
                model.lastupdatedby = currentUser;
                model.ReportFile = "GenReport";
                model.ExportFile = FormDisplayNameForExportFile(model.CustomerName);
                model.ExportFormat = 4;

                if (model.Frequency == null)
                {
                    model.Frequency = new Frequency();
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .Where(m => !string.IsNullOrEmpty(m))
                        .ToList();

                    if (errors.Any())
                    {
                        return Json(new { success = false, errors });
                    }
                }

                if (string.IsNullOrEmpty(model.CIF_ID))
                {
                    return Json(new { success = false, message = "CIF ID is required. Please search for a customer first." });
                }

                if (string.IsNullOrEmpty(model.CustomerNumber))
                {
                    return Json(new { success = false, message = "Customer number is required." });
                }

                var branchCode = await GetBranchCodeAsync(model.Branch);
                var currentDateTime = DateTime.UtcNow;

                var connectionString = _configuration.GetConnectionString("GTMailConn");
                if (string.IsNullOrEmpty(connectionString))
                {
                    _logger.LogError("Database connection string 'GTMailConn' is not configured");
                    return Json(new { success = false, message = "Database configuration error" });
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var existingCustomer = await CheckIfCustomerExists(connection, model.CIF_ID);

                    if (existingCustomer != null)
                    {
                        _logger.LogInformation("Customer with CIF_ID {CIF_ID} already exists. Updating instead.", model.CIF_ID);

                        model.ID = existingCustomer.ID;
                        model.CustomerNumber = existingCustomer.CustomerNumber;
                        model.Branch = existingCustomer.Branch;

                        return await UpdateExistingCustomer(connection, model, currentDateTime, branchCode);
                    }
                    else
                    {
                        return await InsertNewCustomer(connection, model, currentDateTime, branchCode);
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                _logger.LogError(ex, "Duplicate customer number: {CustomerNumber}", model.CustomerNumber);
                return Json(new { success = false, message = "A customer with this number already exists" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving customer: {Message}", ex.Message);
                return Json(new { success = false, message = "An unexpected error occurred. Please try again" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCustomer(string CIF_ID)
        {
            if (string.IsNullOrEmpty(CIF_ID))
            {
                return View(new NewCustomerDto());
            }

            try
            {
                var accounts = await _middlewareClient.GetCustomerCIFAccountsAsync(CIF_ID);
                var connectionString = _configuration.GetConnectionString("GTMailConn");

                if (string.IsNullOrEmpty(connectionString))
                {
                    _logger.LogError("Database connection string 'GTMailConn' is not configured");
                    ViewBag.Error = "Database configuration error";
                    return View(new NewCustomerDto());
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(
                        @"SELECT BranchCode, CustomerNo, DisplayName, ExportFile, ReportFile, Email, ExportFormat, DefaultExt, Status,
                  UseExportFile, Allaccounts, Daily, DailyOpt, Weekly, WeeklyOpt, Monthly, MonthlyOpt,
                  LastsentDaily, LastsentWeekly, LastsentMonthly, Createdby, datecreated, lastupdatedby, lastupdateddate,
                  RegenerateDailyStmt, NoofDays, ContinuousPerMonth, ContinuousOpt, Continuous, CIF_ID, ID
                  FROM Profile WHERE CIF_ID = @CIF_ID", connection))
                    {
                        command.Parameters.AddWithValue("@CIF_ID", CIF_ID);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var model = new NewCustomerDto
                                {
                                    ID = reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID")),
                                    Branch = reader["BranchCode"].ToString(),
                                    CustomerNumber = reader["CustomerNo"].ToString(),
                                    CustomerName = reader["DisplayName"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    DefaultExt = reader["DefaultExt"].ToString(),
                                    Status = reader["Status"].ToString(),
                                    UseExportFile = reader.IsDBNull(reader.GetOrdinal("UseExportFile")) ? false : reader.GetBoolean(reader.GetOrdinal("UseExportFile")),
                                    Allaccounts = reader.IsDBNull(reader.GetOrdinal("Allaccounts")) ? false : reader.GetBoolean(reader.GetOrdinal("Allaccounts")),
                                    Frequency = new Frequency
                                    {
                                        Daily = reader.IsDBNull(reader.GetOrdinal("Daily")) ? false : reader.GetBoolean(reader.GetOrdinal("Daily")),
                                        Weekly = reader.IsDBNull(reader.GetOrdinal("Weekly")) ? false : reader.GetBoolean(reader.GetOrdinal("Weekly")),
                                        Monthly = reader.IsDBNull(reader.GetOrdinal("Monthly")) ? false : reader.GetBoolean(reader.GetOrdinal("Monthly")),
                                        // Fix: Use GetInt16 for smallint columns, convert to int?
                                        DailyOpt = reader.IsDBNull(reader.GetOrdinal("DailyOpt")) ? (int?)-1 : (int)reader.GetInt16(reader.GetOrdinal("DailyOpt")),
                                        WeeklyOpt = reader.IsDBNull(reader.GetOrdinal("WeeklyOpt")) ? (int?)-1 : (int)reader.GetInt16(reader.GetOrdinal("WeeklyOpt")),
                                        MonthlyOpt = reader.IsDBNull(reader.GetOrdinal("MonthlyOpt")) ? (int?)-1 : (int)reader.GetInt16(reader.GetOrdinal("MonthlyOpt")),
                                        ContinuousPerMonth = reader.IsDBNull(reader.GetOrdinal("ContinuousPerMonth")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ContinuousPerMonth")),
                                        ContinuousOpt = reader.IsDBNull(reader.GetOrdinal("ContinuousOpt")) ? (int?)-1 : (int)reader.GetInt16(reader.GetOrdinal("ContinuousOpt")),
                                        Continuous = reader.IsDBNull(reader.GetOrdinal("Continuous")) ? false : reader.GetBoolean(reader.GetOrdinal("Continuous")),
                                    },
                                    RegenerateDailyStmt = reader.IsDBNull(reader.GetOrdinal("RegenerateDailyStmt")) ? 0 : reader.GetInt32(reader.GetOrdinal("RegenerateDailyStmt")),
                                    CIF_ID = reader["CIF_ID"].ToString(),
                                    Createdby = reader["Createdby"].ToString(),
                                    datecreated = reader.IsDBNull(reader.GetOrdinal("datecreated")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("datecreated")),
                                    lastupdatedby = reader["lastupdatedby"].ToString(),
                                    lastupdateddate = reader.IsDBNull(reader.GetOrdinal("lastupdateddate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("lastupdateddate")),
                                    Accounts = accounts
                                };

                                return View(model);
                            }
                        }
                    }
                }

                ViewBag.Error = "Customer not found";
                return View(new NewCustomerDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching customer: {Message}", ex.Message);
                ViewBag.Error = "Error searching customer";
                return View(new NewCustomerDto());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCustomer(NewCustomerDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new { success = false, errors });
            }

            try
            {
                model.lastupdatedby = "System";
                model.lastupdateddate = DateTime.UtcNow;

                var connectionString = _configuration.GetConnectionString("GTMailConn");
                if (string.IsNullOrEmpty(connectionString))
                {
                    _logger.LogError("Database connection string 'GTMailConn' is not configured");
                    return Json(new { success = false, message = "Database configuration error" });
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(
                        @"UPDATE Profile SET
                    Email = @Email,
                    DefaultExt = @DefaultExt,
                    RegenerateDailyStmt = @RegenerateDailyStmt,
                    Continuous = @Continuous,
                    Daily = @Daily,
                    Weekly = @Weekly,
                    Monthly = @Monthly,
                    DailyOpt = @DailyOpt,
                    WeeklyOpt = @WeeklyOpt,
                    MonthlyOpt = @MonthlyOpt,
                    ContinuousOpt = @ContinuousOpt,
                    Allaccounts = @Allaccounts,
                    lastupdatedby = @lastupdatedby,
                    lastupdateddate = @lastupdateddate
                  WHERE ID = @ID AND CustomerNo = @CustomerNumber", connection))
                    {
                        command.Parameters.AddWithValue("@Email", (object)model.Email ?? DBNull.Value);
                        command.Parameters.AddWithValue("@DefaultExt", (object)model.DefaultExt ?? DBNull.Value);
                        command.Parameters.AddWithValue("@RegenerateDailyStmt", model.RegenerateDailyStmt);
                        command.Parameters.AddWithValue("@Continuous", model.Frequency.Continuous);
                        command.Parameters.AddWithValue("@Daily", model.Frequency.Daily);
                        command.Parameters.AddWithValue("@Weekly", model.Frequency.Weekly);
                        command.Parameters.AddWithValue("@Monthly", model.Frequency.Monthly);
                        // Fix: Convert int? to short? for smallint columns
                        command.Parameters.AddWithValue("@DailyOpt", (short)(model.Frequency.DailyOpt ?? -1));
                        command.Parameters.AddWithValue("@WeeklyOpt", (short)(model.Frequency.WeeklyOpt ?? -1));
                        command.Parameters.AddWithValue("@MonthlyOpt", (short)(model.Frequency.MonthlyOpt ?? -1));
                        command.Parameters.AddWithValue("@ContinuousOpt", (short)(model.Frequency.ContinuousOpt ?? -1));
                        command.Parameters.AddWithValue("@Allaccounts", model.Allaccounts);
                        command.Parameters.AddWithValue("@lastupdatedby", (object)model.lastupdatedby ?? DBNull.Value);
                        command.Parameters.AddWithValue("@lastupdateddate", (object)model.lastupdateddate ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CustomerNumber", (object)model.CustomerNumber ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ID", model.ID);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Json(new { success = true, message = "Customer updated successfully" });
                        }

                        _logger.LogError("No rows affected while updating customer {CustomerNumber}", model.CustomerNumber);
                        return Json(new { success = false, message = "Failed to update customer data" });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer: {Message}", ex.Message);
                return Json(new { success = false, message = "Error updating customer data" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetCustomerDetails(string cifId)
        {
            try
            {
                if (string.IsNullOrEmpty(cifId))
                {
                    return Json(new { success = false, message = "CIF ID is required" });
                }

                var connectionString = _configuration.GetConnectionString("GTMailConn");
                if (string.IsNullOrEmpty(connectionString))
                {
                    return Json(new { success = false, message = "Database configuration error" });
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(
                        @"SELECT 
                    ID, CIF_ID, CustomerNo, DisplayName, Email, BranchCode, 
                    DefaultExt, Status, UseExportFile, Allaccounts,
                    Daily, DailyOpt, Weekly, WeeklyOpt, Monthly, MonthlyOpt,
                    Continuous, ContinuousOpt, ContinuousPerMonth,
                    RegenerateDailyStmt,
                    Createdby, datecreated, lastupdatedby, lastupdateddate
                  FROM Profile WHERE CIF_ID = @CIF_ID", connection))
                    {
                        command.Parameters.AddWithValue("@CIF_ID", cifId);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var accountCount = await GetCustomerAccountCountAsync(cifId);

                                var data = new
                                {
                                    cifId = reader["CIF_ID"].ToString(),
                                    customerNumber = reader["CustomerNo"].ToString(),
                                    displayName = reader["DisplayName"].ToString(),
                                    email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader["Email"].ToString(),
                                    branchCode = reader["BranchCode"].ToString(),
                                    defaultExt = reader.IsDBNull(reader.GetOrdinal("DefaultExt")) ? null : reader["DefaultExt"].ToString(),
                                    status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader["Status"].ToString(),
                                    useExportFile = reader.IsDBNull(reader.GetOrdinal("UseExportFile")) ? false : reader.GetBoolean(reader.GetOrdinal("UseExportFile")),
                                    allAccounts = reader.IsDBNull(reader.GetOrdinal("Allaccounts")) ? false : reader.GetBoolean(reader.GetOrdinal("Allaccounts")),
                                    daily = reader.IsDBNull(reader.GetOrdinal("Daily")) ? false : reader.GetBoolean(reader.GetOrdinal("Daily")),
                                    // FIX: Use GetInt16 for smallint columns, then convert to int
                                    dailyOpt = reader.IsDBNull(reader.GetOrdinal("DailyOpt")) ? (int?)null : (int)reader.GetInt16(reader.GetOrdinal("DailyOpt")),
                                    weekly = reader.IsDBNull(reader.GetOrdinal("Weekly")) ? false : reader.GetBoolean(reader.GetOrdinal("Weekly")),
                                    weeklyOpt = reader.IsDBNull(reader.GetOrdinal("WeeklyOpt")) ? (int?)null : (int)reader.GetInt16(reader.GetOrdinal("WeeklyOpt")),
                                    monthly = reader.IsDBNull(reader.GetOrdinal("Monthly")) ? false : reader.GetBoolean(reader.GetOrdinal("Monthly")),
                                    monthlyOpt = reader.IsDBNull(reader.GetOrdinal("MonthlyOpt")) ? (int?)null : (int)reader.GetInt16(reader.GetOrdinal("MonthlyOpt")),
                                    continuous = reader.IsDBNull(reader.GetOrdinal("Continuous")) ? false : reader.GetBoolean(reader.GetOrdinal("Continuous")),
                                    continuousOpt = reader.IsDBNull(reader.GetOrdinal("ContinuousOpt")) ? (int?)null : (int)reader.GetInt16(reader.GetOrdinal("ContinuousOpt")),
                                    continuousPerMonth = reader.IsDBNull(reader.GetOrdinal("ContinuousPerMonth")) ? 0 : reader.GetInt32(reader.GetOrdinal("ContinuousPerMonth")),
                                    regenerateDailyStmt = reader.IsDBNull(reader.GetOrdinal("RegenerateDailyStmt")) ? 0 : reader.GetInt32(reader.GetOrdinal("RegenerateDailyStmt")),
                                    createdBy = reader["Createdby"].ToString(),
                                    dateCreated = reader.IsDBNull(reader.GetOrdinal("datecreated")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("datecreated")),
                                    lastUpdatedBy = reader.IsDBNull(reader.GetOrdinal("lastupdatedby")) ? null : reader["lastupdatedby"].ToString(),
                                    lastUpdatedDate = reader.IsDBNull(reader.GetOrdinal("lastupdateddate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("lastupdateddate")),
                                    accountCount = accountCount
                                };

                                return Json(new { success = true, data });
                            }
                        }
                    }
                }

                return Json(new { success = false, message = "Customer not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer details for CIF: {CIF_ID}", cifId);
                return Json(new { success = false, message = "An error occurred while retrieving customer details" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> SearchCustomers(string parameter = null, string value = null)
        {
            var model = new SearchCustomerViewModel
            {
                SearchParameter = parameter ?? "cif_id",
                SearchValue = value ?? "",
                Customers = new List<CustomerProfileDto>()
            };

            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    model.Customers = await SearchCustomersByMailDbWithDetails(parameter, value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error searching customers");
                    ViewBag.Error = "An error occurred while searching. Please try again.";
                }
            }

            return View(model);
        }


        private async Task<int?> GetBranchCodeAsync(string branchName)
        {
            int? branchCode = null;

            try
            {
                var connectionString = _configuration.GetConnectionString("EOneConn");
                if (string.IsNullOrEmpty(connectionString))
                {
                    _logger.LogWarning("Database connection string 'EOneConn' is not configured");
                    return null;
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(
                        "SELECT branch_Code FROM Branches WHERE Branch_Name LIKE @BranchName", connection))
                    {
                        command.Parameters.AddWithValue("@BranchName", $"%{branchName}%");
                        var result = await command.ExecuteScalarAsync();
                        if (result is int value)
                        {
                            branchCode = value;
                        }
                    }
                }

                if (!branchCode.HasValue)
                {
                    _logger.LogWarning($"No branch code found for BranchName: {branchName}");
                }

                return branchCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching branch code for BranchName: {branchName}");
                throw;
            }
        }
        public async Task<NewCustomerDto> DetermineCustomerAccount(string accountNumber)
        {
            try
            {
                if (!Regex.IsMatch(accountNumber, @"^[1-3]\d{3}00[1-5]\d{6,}$"))
                {
                    accountNumber = await _basisFallbackService.GetFinacleAccountFromBasisAccount(accountNumber);
                }

                var detail = await _middlewareClient.GetAccountsAndCustomersAsync(accountNumber);
                if (detail.AccountDetails == null || detail.CustomerDetails == null)
                {
                    return null;
                }
                var ad = detail.AccountDetails[0];
                var cd = detail.CustomerDetails[0];
                var allAccounts = await _middlewareClient.GetCustomerCIFAccountsAsync(cd.CustomerReference);

                return new NewCustomerDto
                {
                    Branch = ad.BranchName,
                    CIF_ID = cd.CustomerReference,
                    CustomerName = cd.CustomerName,
                    Accounts = allAccounts?.Select(row => new Account
                    {
                        AccountIdentification = row.AccountIdentification?.ToString(),
                        AccountName = row.AccountName?.ToString(),
                        AccountCurrency = row.AccountCurrency?.ToString()
                    }).ToList() ?? new List<Account>(),
                    Email = cd.Email,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while determining customer account. {ex.Message} -- {ex.StackTrace}");
                return null;
            }
        }
        private async Task<List<CustomerInfo>> SearchCustomersByMailDb(string parameter, string value)
        {
            var customers = new List<CustomerInfo>();
            var connectionString = _configuration.GetConnectionString("GTMailConn");

            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("Database connection string 'GTMailConn' is not configured");
                return customers;
            }

            string column;
            switch (parameter?.ToLower())
            {
                case "accountname":
                    column = "DisplayName";
                    break;
                case "email":
                    column = "Email";
                    break;
                case "cif_id":
                    column = "CIF_ID";
                    break;
                default:
                    _logger.LogWarning("Invalid search parameter: {SearchParameter}", parameter);
                    return customers;
            }

            string sqlCommand = $@"
                SELECT CIF_ID, BranchCode, CustomerNo, DisplayName, Email, ReportFile, ExportFormat, DefaultExt, Status, 
                       UseExportFile, AllAccounts, Daily, DailyOpt, Weekly, WeeklyOpt, MonthlyOpt, Monthly, lastupdatedby, 
                       RegenerateDailyStmt, lastupdateddate
                FROM Profile
                WHERE {column} LIKE @SearchValue";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(sqlCommand, connection))
                    {
                        command.Parameters.AddWithValue("@SearchValue", $"%{value}%");
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                customers.Add(new CustomerInfo
                                {
                                    CIF_ID = reader.GetString(reader.GetOrdinal("CIF_ID")),
                                    BranchCode = reader.GetInt32(reader.GetOrdinal("BranchCode")),
                                    CustomerNo = reader.GetInt32(reader.GetOrdinal("CustomerNo")),
                                    DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                                    Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                                    ReportFile = reader.IsDBNull(reader.GetOrdinal("ReportFile")) ? null : reader.GetString(reader.GetOrdinal("ReportFile")),
                                    ExportFormat = reader.GetInt16(reader.GetOrdinal("ExportFormat")),
                                    DefaultExt = reader.IsDBNull(reader.GetOrdinal("DefaultExt")) ? null : reader.GetString(reader.GetOrdinal("DefaultExt")),
                                    Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                                    UseExportFile = reader.GetBoolean(reader.GetOrdinal("UseExportFile")),
                                    AllAccounts = reader.GetBoolean(reader.GetOrdinal("AllAccounts")),
                                    Daily = reader.GetBoolean(reader.GetOrdinal("Daily")),
                                    DailyOpt = reader.GetInt16(reader.GetOrdinal("DailyOpt")),
                                    Weekly = reader.GetBoolean(reader.GetOrdinal("Weekly")),
                                    WeeklyOpt = reader.GetInt16(reader.GetOrdinal("WeeklyOpt")),
                                    Monthly = reader.GetBoolean(reader.GetOrdinal("Monthly")),
                                    MonthlyOpt = reader.GetInt16(reader.GetOrdinal("MonthlyOpt")),
                                    lastupdatedby = reader.GetString(reader.GetOrdinal("lastupdatedby")),
                                    lastupdateddate = reader.GetDateTime(reader.GetOrdinal("lastupdateddate")),
                                    RegenerateDailyStmt = reader.GetInt32(reader.GetOrdinal("RegenerateDailyStmt"))
                                });
                            }
                        }
                    }
                }

                _logger.LogInformation("Found {CustomerCount} customers for {SearchParameter}: {SearchValue}", customers.Count, parameter, value);
                return customers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying Profile table for {SearchParameter}: {SearchValue}", parameter, value);
                return customers;
            }
        }
        private string GetLastSixCharForCustomerNumber(string accountIdentification)
        {
            if (string.IsNullOrEmpty(accountIdentification) || accountIdentification.Length < 6)
            {
                return null;
            }
            return accountIdentification.Substring(accountIdentification.Length - 6, 6);
        }
        private async Task<int> GetCustomerAccountCountAsync(string cif)
        {
            try
            {
                var accounts = await _middlewareClient.GetCustomerCIFAccountsAsync(cif);
                return accounts?.Count ?? 0;
            }
            catch
            {
                return 0;
            }
        }
        private async Task<List<CustomerProfileDto>> SearchCustomersByMailDbWithDetails(string parameter, string value)
        {
            var customers = new List<CustomerProfileDto>();
            var connectionString = _configuration.GetConnectionString("GTMailConn");

            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("Database connection string 'GTMailConn' is not configured");
                return customers;
            }

            string column;
            switch (parameter?.ToLower())
            {
                case "accountname":
                    column = "DisplayName";
                    break;
                case "email":
                    column = "Email";
                    break;
                case "cif_id":
                    column = "CIF_ID";
                    break;
                default:
                    _logger.LogWarning("Invalid search parameter: {SearchParameter}", parameter);
                    return customers;
            }

            string sqlCommand = $@"
        SELECT 
            ID, CIF_ID, CustomerNo, DisplayName, Email, BranchCode, 
            DefaultExt, Status, UseExportFile, Allaccounts,
            Daily, DailyOpt, Weekly, WeeklyOpt, Monthly, MonthlyOpt,
            Continuous, ContinuousOpt, ContinuousPerMonth,
            RegenerateDailyStmt,
            Createdby, datecreated, lastupdatedby, lastupdateddate
        FROM Profile
        WHERE {column} LIKE @SearchValue
        ORDER BY DisplayName";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(sqlCommand, connection))
                    {
                        command.Parameters.AddWithValue("@SearchValue", $"%{value}%");
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var customer = new CustomerProfileDto
                                {
                                    ID = reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID")),
                                    CIF_ID = reader["CIF_ID"].ToString(),
                                    CustomerNumber = reader["CustomerNo"].ToString(),
                                    DisplayName = reader["DisplayName"].ToString(),
                                    Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader["Email"].ToString(),
                                    BranchCode = reader["BranchCode"].ToString(),
                                    DefaultExt = reader.IsDBNull(reader.GetOrdinal("DefaultExt")) ? null : reader["DefaultExt"].ToString(),
                                    Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader["Status"].ToString(),
                                    UseExportFile = reader.IsDBNull(reader.GetOrdinal("UseExportFile")) ? false : reader.GetBoolean(reader.GetOrdinal("UseExportFile")),
                                    AllAccounts = reader.IsDBNull(reader.GetOrdinal("Allaccounts")) ? false : reader.GetBoolean(reader.GetOrdinal("Allaccounts")),
                                    Daily = reader.IsDBNull(reader.GetOrdinal("Daily")) ? false : reader.GetBoolean(reader.GetOrdinal("Daily")),
                                    // Fix: Convert smallint to int? using GetInt16
                                    DailyOpt = reader.IsDBNull(reader.GetOrdinal("DailyOpt")) ? (int?)null : (int)reader.GetInt16(reader.GetOrdinal("DailyOpt")),
                                    Weekly = reader.IsDBNull(reader.GetOrdinal("Weekly")) ? false : reader.GetBoolean(reader.GetOrdinal("Weekly")),
                                    WeeklyOpt = reader.IsDBNull(reader.GetOrdinal("WeeklyOpt")) ? (int?)null : (int)reader.GetInt16(reader.GetOrdinal("WeeklyOpt")),
                                    Monthly = reader.IsDBNull(reader.GetOrdinal("Monthly")) ? false : reader.GetBoolean(reader.GetOrdinal("Monthly")),
                                    MonthlyOpt = reader.IsDBNull(reader.GetOrdinal("MonthlyOpt")) ? (int?)null : (int)reader.GetInt16(reader.GetOrdinal("MonthlyOpt")),
                                    Continuous = reader.IsDBNull(reader.GetOrdinal("Continuous")) ? false : reader.GetBoolean(reader.GetOrdinal("Continuous")),
                                    ContinuousOpt = reader.IsDBNull(reader.GetOrdinal("ContinuousOpt")) ? (int?)null : (int)reader.GetInt16(reader.GetOrdinal("ContinuousOpt")),
                                    ContinuousPerMonth = reader.IsDBNull(reader.GetOrdinal("ContinuousPerMonth")) ? 0 : reader.GetInt32(reader.GetOrdinal("ContinuousPerMonth")),
                                    RegenerateDailyStmt = reader.IsDBNull(reader.GetOrdinal("RegenerateDailyStmt")) ? 0 : reader.GetInt32(reader.GetOrdinal("RegenerateDailyStmt")),
                                    CreatedBy = reader["Createdby"].ToString(),
                                    DateCreated = reader.IsDBNull(reader.GetOrdinal("datecreated")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("datecreated")),
                                    LastUpdatedBy = reader.IsDBNull(reader.GetOrdinal("lastupdatedby")) ? null : reader["lastupdatedby"].ToString(),
                                    LastUpdatedDate = reader.IsDBNull(reader.GetOrdinal("lastupdateddate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("lastupdateddate"))
                                };

                                customer.AccountCount = await GetCustomerAccountCountAsync(customer.CIF_ID);
                                customers.Add(customer);
                            }
                        }
                    }
                }

                _logger.LogInformation("Found {CustomerCount} customers for {SearchParameter}: {SearchValue}",
                    customers.Count, parameter, value);
                return customers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying Profile table for {SearchParameter}: {SearchValue}", parameter, value);
                throw;
            }
        }
        private string FormDisplayNameForExportFile(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length < 6)
            {
                return null;
            }
            return name.Split(' ')[0] + " " + "STMT";
        }
        private async Task<ExistingCustomerInfo> CheckIfCustomerExists(SqlConnection connection, string cifId)
        {
            try
            {
                using (var command = new SqlCommand(
                    @"SELECT ID, CustomerNo, BranchCode, DisplayName, Email, Status 
              FROM Profile 
              WHERE CIF_ID = @CIF_ID", connection))
                {
                    command.Parameters.AddWithValue("@CIF_ID", cifId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new ExistingCustomerInfo
                            {
                                ID = reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID")),
                                CustomerNumber = reader["CustomerNo"].ToString(),
                                Branch = reader["BranchCode"].ToString(),
                                DisplayName = reader["DisplayName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Status = reader["Status"].ToString()
                            };
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if customer exists for CIF: {CIF_ID}", cifId);
                return null;
            }
        }
        private async Task<IActionResult> InsertNewCustomer(SqlConnection connection, NewCustomerDto model, DateTime currentDateTime, int? branchCode)
        {
            const string sql = @"
        INSERT INTO Profile (
            BranchCode, CustomerNo, DisplayName, ExportFile, ReportFile, Email,
            ExportFormat, DefaultExt, Status, UseExportFile, Allaccounts,
            Daily, DailyOpt, Weekly, WeeklyOpt, Monthly, MonthlyOpt,
            LastsentDaily, LastsentWeekly, LastsentMonthly,
            Createdby, datecreated, lastupdatedby, lastupdateddate,
            RegenerateDailyStmt, NoofDays, ContinuousPerMonth, ContinuousOpt, Continuous, CIF_ID
        ) VALUES (
            @BranchCode, @CustomerNumber, @CustomerName, @ExportFile, @ReportFile, @Email,
            @ExportFormat, @DefaultExt, @Status, @UseExportFile, @Allaccounts,
            @Daily, @DailyOpt, @Weekly, @WeeklyOpt, @Monthly, @MonthlyOpt,
            NULL, NULL, NULL,
            @Createdby, @datecreated, @lastupdatedby, @lastupdateddate,
            @RegenerateDailyStmt, NULL, @ContinuousPerMonth, @ContinuousOpt, @Continuous, @CIF_ID
        );
        SELECT SCOPE_IDENTITY();";

            using (var command = new SqlCommand(sql, connection))
            {
                AddParameters(command, model, currentDateTime, branchCode);

                var newId = await command.ExecuteScalarAsync();
                model.ID = Convert.ToInt32(newId);

                _logger.LogInformation("New customer inserted with ID: {ID} and CIF: {CIF_ID}", model.ID, model.CIF_ID);
                return Json(new { success = true, message = "Customer saved successfully" });
            }
        }
        private async Task<IActionResult> UpdateExistingCustomer(SqlConnection connection, NewCustomerDto model, DateTime currentDateTime, int? branchCode)
        {
            const string sql = @"
        UPDATE Profile SET
            DisplayName = @CustomerName,
            Email = @Email,
            DefaultExt = @DefaultExt,
            Status = @Status,
            UseExportFile = @UseExportFile,
            Allaccounts = @Allaccounts,
            Daily = @Daily,
            DailyOpt = @DailyOpt,
            Weekly = @Weekly,
            WeeklyOpt = @WeeklyOpt,
            Monthly = @Monthly,
            MonthlyOpt = @MonthlyOpt,
            Continuous = @Continuous,
            ContinuousOpt = @ContinuousOpt,
            ContinuousPerMonth = @ContinuousPerMonth,
            RegenerateDailyStmt = @RegenerateDailyStmt,
            lastupdatedby = @lastupdatedby,
            lastupdateddate = @lastupdateddate,
            BranchCode = @BranchCode
        WHERE CIF_ID = @CIF_ID";

            using (var command = new SqlCommand(sql, connection))
            {
                // Add all parameters including CIF_ID for WHERE clause
                command.Parameters.AddWithValue("@CustomerName", (object)model.CustomerName ?? DBNull.Value);
                command.Parameters.AddWithValue("@Email", (object)model.Email ?? DBNull.Value);
                command.Parameters.AddWithValue("@DefaultExt", (object)model.DefaultExt ?? DBNull.Value);
                command.Parameters.AddWithValue("@Status", (object)model.Status ?? DBNull.Value);
                command.Parameters.AddWithValue("@UseExportFile", model.UseExportFile);
                command.Parameters.AddWithValue("@Allaccounts", model.Allaccounts);
                command.Parameters.AddWithValue("@Daily", model.Frequency.Daily);
                command.Parameters.AddWithValue("@Weekly", model.Frequency.Weekly);
                command.Parameters.AddWithValue("@Monthly", model.Frequency.Monthly);
                command.Parameters.AddWithValue("@Continuous", model.Frequency.Continuous);
                command.Parameters.AddWithValue("@DailyOpt", (short)(model.Frequency.DailyOpt ?? -1));
                command.Parameters.AddWithValue("@WeeklyOpt", (short)(model.Frequency.WeeklyOpt ?? -1));
                command.Parameters.AddWithValue("@MonthlyOpt", (short)(model.Frequency.MonthlyOpt ?? -1));
                command.Parameters.AddWithValue("@ContinuousOpt", (short)(model.Frequency.ContinuousOpt ?? -1));
                command.Parameters.AddWithValue("@ContinuousPerMonth", model.Frequency.ContinuousPerMonth);
                command.Parameters.AddWithValue("@RegenerateDailyStmt", model.RegenerateDailyStmt);
                command.Parameters.AddWithValue("@lastupdatedby", (object)model.lastupdatedby ?? DBNull.Value);
                command.Parameters.AddWithValue("@lastupdateddate", currentDateTime);
                command.Parameters.AddWithValue("@BranchCode", (object)branchCode ?? DBNull.Value);
                command.Parameters.AddWithValue("@CIF_ID", (object)model.CIF_ID ?? DBNull.Value);

                int rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Customer updated for CIF: {CIF_ID}", model.CIF_ID);
                    return Json(new { success = true, message = "Customer updated successfully" });
                }
                else
                {
                    _logger.LogWarning("No rows updated for CIF: {CIF_ID}", model.CIF_ID);
                    return Json(new { success = false, message = "Failed to update customer data" });
                }
            }
        }
        private void AddParameters(SqlCommand command, NewCustomerDto model, DateTime currentDateTime, int? branchCode)
        {
            // Basic fields
            command.Parameters.AddWithValue("@BranchCode", (object)branchCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@CustomerNumber", (object)model.CustomerNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@CustomerName", (object)model.CustomerName ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExportFile", (object)model.ExportFile ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReportFile", (object)model.ReportFile ?? DBNull.Value);
            command.Parameters.AddWithValue("@Email", (object)model.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExportFormat", model.ExportFormat);
            command.Parameters.AddWithValue("@DefaultExt", (object)model.DefaultExt ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", (object)model.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@Createdby", (object)model.Createdby ?? DBNull.Value);
            command.Parameters.AddWithValue("@lastupdatedby", (object)model.lastupdatedby ?? DBNull.Value);
            command.Parameters.AddWithValue("@datecreated", currentDateTime);
            command.Parameters.AddWithValue("@lastupdateddate", currentDateTime);
            command.Parameters.AddWithValue("@CIF_ID", (object)model.CIF_ID ?? DBNull.Value);

            // BIT fields
            command.Parameters.AddWithValue("@UseExportFile", model.UseExportFile);
            command.Parameters.AddWithValue("@Allaccounts", model.Allaccounts);
            command.Parameters.AddWithValue("@Daily", model.Frequency.Daily);
            command.Parameters.AddWithValue("@Weekly", model.Frequency.Weekly);
            command.Parameters.AddWithValue("@Monthly", model.Frequency.Monthly);
            command.Parameters.AddWithValue("@Continuous", model.Frequency.Continuous);

            // INT fields
            command.Parameters.AddWithValue("@RegenerateDailyStmt", model.RegenerateDailyStmt);
            command.Parameters.AddWithValue("@ContinuousPerMonth", model.Frequency.ContinuousPerMonth);

            // SMALLINT fields
            command.Parameters.AddWithValue("@DailyOpt", (short)(model.Frequency.DailyOpt ?? -1));
            command.Parameters.AddWithValue("@WeeklyOpt", (short)(model.Frequency.WeeklyOpt ?? -1));
            command.Parameters.AddWithValue("@MonthlyOpt", (short)(model.Frequency.MonthlyOpt ?? -1));
            command.Parameters.AddWithValue("@ContinuousOpt", (short)(model.Frequency.ContinuousOpt ?? -1));
        }
    }
}