using System.Data;
using Loans.API.Entities.DTOs;
using Oracle.ManagedDataAccess.Client;

namespace Loans.API.Services
{
    public class LoanService : ILoanService
    {
        private readonly string _connectionString;
        private readonly ILogger<LoanService> _logger;

        private const string SCHM = "CURNT";
        private const string LOAN_SCHM = "RLQCR";
        private const int TELL_ID = 9938;
        private const string CUR = "GHS";
        private const int VALIDATION_CODE = 2;

        public LoanService(IConfiguration configuration, ILogger<LoanService> logger)
        {
            _connectionString = configuration.GetConnectionString("OracleDb") ?? throw new ArgumentNullException("OracleDb ConnectionString is missing");
            _logger = logger;
        }

        public async Task<EligibilityResponse> GetEligibilityAsync(int braCode, string cusNum)
        {
            try
            {
                using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync();

                string query = @"
                    SELECT a.*, b.fullname cust_name 
                    FROM (
                        SELECT 
                            NVL2(b.sol_id, b.sol_id, NVL(a.sol_id, 0)) sol_id, 
                            NVL2(b.cif_id, b.cif_id, NVL(a.cif_id, 0)) cif_id, 
                            NVL2(b.average_salary, b.average_salary, NVL(a.average_salary, 0)) average_salary, 
                            NVL2(b.next_payment_amt, b.next_payment_amt, NVL(a.next_payment_amt, 0)) next_payment_amt, 
                            NVL2(b.sal_adveligible_amt, b.sal_adveligible_amt, NVL(a.sal_adveligible_amt, 0)) sal_adveligible_amt, 
                            NVL2(b.sol_id, 2, NVL2(a.sol_id, 1, 0)) existing_qualified 
                        FROM gtb_salary_advance_s a 
                        FULL OUTER JOIN temp_salary_adv_manual b 
                            ON a.sol_id = b.sol_id 
                            AND a.cif_id = b.cif_id 
                    ) a, stg.retail b 
                    WHERE a.cif_id = b.cifid  
                    AND a.cif_id = :cus_num
                    AND a.sol_id = :bra_code";

                using var command = new OracleCommand(query, connection);
                command.Parameters.Add(new OracleParameter("cus_num", cusNum));
                command.Parameters.Add(new OracleParameter("bra_code", braCode));

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var detail = new CustomerDetail
                    {
                        Bra_Code = reader["SOL_ID"].ToString() ?? string.Empty,
                        Cus_Num = reader["CIF_ID"].ToString() ?? string.Empty,
                        Avg_Sal = Convert.ToDecimal(reader["AVERAGE_SALARY"]),
                        Next_Pay_Amt = Convert.ToDecimal(reader["NEXT_PAYMENT_AMT"]),
                        Sal_Elg_Amt = Convert.ToDecimal(reader["SAL_ADVELIGIBLE_AMT"]),
                        Cus_Name = reader["CUST_NAME"].ToString() ?? string.Empty,
                        Already_Qualified = Convert.ToInt32(reader["EXISTING_QUALIFIED"])
                    };

                    return new EligibilityResponse
                    {
                        Code = "1000",
                        Cust_Det = detail,
                        Already_Qualified = detail.Already_Qualified
                    };
                }

                return new EligibilityResponse
                {
                    Code = "1001",
                    Error = "NO ELIGIBILITY FOUND",
                    Already_Qualified = 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking eligibility for {CusNum}", cusNum);
                return new EligibilityResponse
                {
                    Code = "1002",
                    Error = ex.Message,
                    Already_Qualified = 0
                };
            }
        }

        public async Task<ManualQuickCreditResponse> ProcessManualQuickCreditAsync(ManualQuickCreditRequest request)
        {
            try
            {
                using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new OracleCommand("STG.GTB_MANUAL_LOAN_ENTRY", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("INP_BRA", OracleDbType.Int32).Value = request.BraCode;
                command.Parameters.Add("INP_CIF", OracleDbType.Varchar2).Value = request.CusNum;
                command.Parameters.Add("INP_CUR", OracleDbType.Varchar2).Value = CUR;
                command.Parameters.Add("INP_SCHM", OracleDbType.Varchar2).Value = SCHM;
                command.Parameters.Add("INP_FORACID", OracleDbType.Varchar2).Value = request.Account;
                command.Parameters.Add("INP_LOAN_SCHM", OracleDbType.Varchar2).Value = LOAN_SCHM;
                command.Parameters.Add("INP_TELL_ID", OracleDbType.Int32).Value = TELL_ID;
                command.Parameters.Add("INP_salary", OracleDbType.Decimal).Value = request.SalEligible;
                command.Parameters.Add("INP_AD_USER", OracleDbType.Varchar2).Value = request.PmApprover;
                command.Parameters.Add("INP_REMARKS", OracleDbType.Varchar2).Value = request.Remarks;
                command.Parameters.Add("INP_VALIDATION_CODE", OracleDbType.Int32).Value = VALIDATION_CODE;

                var returnStatusParam = new OracleParameter("RETURN_STATUS", OracleDbType.Varchar2, 2000) { Direction = ParameterDirection.Output };
                var rejectReasonParam = new OracleParameter("OUT_REJECTION_REASON", OracleDbType.Varchar2, 2000) { Direction = ParameterDirection.Output };
                
                command.Parameters.Add(returnStatusParam);
                command.Parameters.Add(rejectReasonParam);

                await command.ExecuteNonQueryAsync();

                string result = returnStatusParam.Value?.ToString() ?? "";
                string rejectReason = rejectReasonParam.Value?.ToString() ?? "";

                if (result == "0")
                {
                    return new ManualQuickCreditResponse { Code = "1000", Message = "SUCCESS" };
                }

                string errorMessage = string.IsNullOrWhiteSpace(result) 
                    ? "An error occurred. Please try again later" 
                    : GetBasisError(result);

                return new ManualQuickCreditResponse
                {
                    Code = "1001",
                    Reject_Reason = rejectReason,
                    Error = errorMessage
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in manual quick credit for {CusNum}", request.CusNum);
                return new ManualQuickCreditResponse { Code = "1002", Error = ex.Message };
            }
        }

        public async Task<QuickCreditHRUndertakingResponse> ProcessHRUndertakingAsync(QuickCreditHRUndertakingRequest request)
        {
            try
            {
                using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new OracleCommand("GTB_LOAN_HR_UNDERTAKING", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("inp_bra", OracleDbType.Int32).Value = request.BraCode;
                command.Parameters.Add("inp_cus", OracleDbType.Varchar2).Value = request.CusNum;
                command.Parameters.Add("inp_tell_id", OracleDbType.Varchar2).Value = request.TellId;
                command.Parameters.Add("inp_salary", OracleDbType.Decimal).Value = request.SalEligible;
                command.Parameters.Add("inp_ad_user", OracleDbType.Varchar2).Value = request.PmApprover;
                command.Parameters.Add("inp_remarks", OracleDbType.Varchar2).Value = request.Remarks;
                command.Parameters.Add("inp_validation_code", OracleDbType.Int32).Value = request.ValidationCode;

                var returnStatusParam = new OracleParameter("RETURN_STATUS", OracleDbType.Varchar2, 2000) { Direction = ParameterDirection.Output };
                var rejectReasonParam = new OracleParameter("OUT_REJECTION_REASON", OracleDbType.Varchar2, 2000) { Direction = ParameterDirection.Output };
                
                command.Parameters.Add(returnStatusParam);
                command.Parameters.Add(rejectReasonParam);

                await command.ExecuteNonQueryAsync();

                string result = returnStatusParam.Value?.ToString() ?? "";
                string rejectReason = rejectReasonParam.Value?.ToString() ?? "";

                if (result == "0")
                {
                    return new QuickCreditHRUndertakingResponse { Code = "1000", Message = "SUCCESS" };
                }

                string errorMessage = string.IsNullOrWhiteSpace(result) 
                    ? "An error occurred. Please try again later" 
                    : GetBasisError(result);

                return new QuickCreditHRUndertakingResponse
                {
                    Code = "1001",
                    Reject_Reason = rejectReason,
                    Error = errorMessage
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in HR undertaking for {CusNum}", request.CusNum);
                return new QuickCreditHRUndertakingResponse { Code = "1002", Error = ex.Message };
            }
        }

        private string GetBasisError(string errorCodeStr)
        {
            if (!int.TryParse(errorCodeStr.Trim(), out int errorCode))
                return $"Unknown error ({errorCodeStr})";

            var mapping = new Dictionary<int, string>
            {
                { 1, "General error" },
                { 2, "Validation failed" },
                { 100, "Invalid customer number" },
                { 101, "Insufficient eligibility" },
                { 102, "Customer already has active loan" },
                { 103, "Branch code not found" },
                { 104, "Invalid teller ID" },
                { 105, "Approver not authorized" }
            };

            return mapping.TryGetValue(errorCode, out string? msg) ? msg : $"BASIS Error {errorCode}";
        }
    }
}
