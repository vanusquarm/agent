using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Statement.Web.Helpers;
using Statement.Web.Interfaces;
using Statement.Web.Models;
using System.Data;
using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text.Json.Serialization;
using System.Web;

namespace Statement.Web.Services
{
    public class StatementClient : IStatementClient
    {
        private readonly string _connectionString;
        private readonly ILogger<StatementClient> _logger;
        private readonly HttpClient _httpClient;

        public StatementClient(ILogger<StatementClient> logger, IConfiguration configuration, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _connectionString = configuration.GetConnectionString("FinacleConn") ?? throw new InvalidOperationException("Database connection string is not configured.");
        }
        public async Task<CustomerAccountDetails> GetCustomerAccountDetailsAsync(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Account number cannot be empty", nameof(accountNumber));

            try
            {
                var query = @"
                    SELECT 
                        (SELECT emailid 
                         FROM cifuser.emaildetail 
                         WHERE cifid = cif_id AND preferred = 'Y') AS EMAIL,

                        (SELECT line1 
                         FROM cifuser.address 
                         WHERE cif_id = cifid) AS ADDRESS,

                        acct_crncy_code AS ACCOUNTCURRENCY,

                        DECODE(schm_type, 'SBA', 'SAVINGS ACCOUNT', 
                                            'CAA', 'CURRENT ACCOUNT', 
                                            schm_type) AS ACCOUNT_TYPE,

                        (SELECT description 
                         FROM cifuser.branchdetail 
                         WHERE code = sol_id) AS BRANCH,

                        ACCT_NAME AS ACCOUNTNAME

                    FROM tbaadm.gam
                    WHERE foracid = :foracid";

                await using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync();

                await using var cmd = new OracleCommand(query, connection);
                cmd.BindByName = true;
                cmd.Parameters.Add(new OracleParameter(":foracid", OracleDbType.Varchar2)
                {
                    Value = accountNumber
                });

                await using var dr = await cmd.ExecuteReaderAsync();

                if (await dr.ReadAsync())
                {
                    return new CustomerAccountDetails
                    {
                        Email = dr["EMAIL"]?.ToString(),
                        Address = dr["ADDRESS"]?.ToString(),
                        AccountCurrency = dr["ACCOUNTCURRENCY"]?.ToString(),
                        AccountType = dr["ACCOUNT_TYPE"]?.ToString(),
                        Branch = dr["BRANCH"]?.ToString(),
                        AccountName = dr["ACCOUNTNAME"]?.ToString()
                    };
                }

                return null;
            }
            catch (OracleException ex)
            {
                _logger.LogError(ex, "Oracle error while fetching account details for {AccountNumber}", accountNumber);
                throw new ApplicationException("Database error occurred while fetching account details", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching account details for {AccountNumber}", accountNumber);
                throw;
            }
        }
        public async Task<List<StatementRecord>> GetStatementRecordsAsync(OracleConnection connection, string foracid, DateTime fromDate, DateTime toDate)
        {
            var records = new List<StatementRecord>();
            try
            {
                var startDate = fromDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture).ToUpper();
                var endDate = toDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture).ToUpper();

                //string query = @"
                //SELECT 
                //    TO_CHAR(s.VALUE_DATE, 'DD-MON-YYYY') AS TransactionValueDate,
                //    TO_CHAR(s.TRAN_DATE, 'DD-MON-YYYY') AS TransactionDate,
                //    s.TRAN_PARTICULAR AS TranParticulars,
                //    s.PART_TRAN_TYPE AS TransactionType,
                //    s.TRAN_AMT AS TransactionAmount,
                //    s.available_amt AS Balance,
                //    s.TRAN_RMKS AS Remarks,
                //    s.INSTRMNT_NUM AS TransactionReference,
                //    TO_CHAR(s.PSTD_DATE, 'DD-MON-YYYY') AS PSTD_DATE
                //FROM custom.CTD_DTD_ACLI_VIEW_ALL_TWEREFOUR s
                //WHERE s.ACID IN (
                //    SELECT ACID FROM TBAADM.GAM WHERE FORACID = :Foracid
                //)
                //AND s.TRAN_DATE >= TO_DATE(:StartDate, 'DD-MON-YYYY')
                //AND s.TRAN_DATE < TO_DATE(:EndDate, 'DD-MON-YYYY') + 1
                //AND s.PSTD_FLG = 'Y'
                //AND s.DEL_FLG = 'N'
                //AND s.ENTRY_USER_ID != 'GTBGH3'
                //ORDER BY s.ENTRY_DATE ASC, s.TRAN_ID ASC, s.PART_TRAN_SRL_NUM ASC";

                string query;


                query = @"SELECT 
                        VALUE_DATE AS TransactionValueDate, 
                        TRA_DATE AS TransactionDate, 
                        DEBIT AS Debit, 
                        CREDIT AS Credit, 
                        RUNNING_BALANCE AS Balance, 
                        DESCRIPTION AS Remarks
                        FROM TABLE(custom.get_account_statement_pipelined(
                            p_foracid => :Foracid,
                            p_start_date => TO_DATE(:StartDate, 'DD-MON-YYYY'),
                            p_end_date   => TO_DATE(:EndDate, 'DD-MON-YYYY')
                        ))";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":Foracid", OracleDbType.Varchar2).Value = foracid;
                    command.Parameters.Add(":StartDate", OracleDbType.Varchar2).Value = startDate;
                    command.Parameters.Add(":EndDate", OracleDbType.Varchar2).Value = endDate;

                    using (OracleDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            records.Add(new StatementRecord
                            {
                                TransactionValueDate = reader["TransactionValueDate"]?.ToString(),
                                TransactionDate = reader["TransactionDate"]?.ToString(),
                                Balance = reader["Balance"] != DBNull.Value ? Convert.ToDecimal(reader["Balance"]) : 0m,
                                Debit = reader["Debit"] != DBNull.Value ? Convert.ToDecimal(reader["Debit"]) : 0m,
                                Credit = reader["Credit"] != DBNull.Value ? Convert.ToDecimal(reader["Credit"]) : 0m,
                                Remarks = reader["Remarks"]?.ToString(),
                            });
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                _logger.LogError(ex, "Oracle error while fetching statement records for FORACID: {Foracid}", foracid);
                throw new ApplicationException("Database error occurred while fetching transactions", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching statement records for FORACID: {Foracid}", foracid);
                throw;
            }
            return records;
        }
        public async Task<(DataTable, decimal, decimal)> GetStatementFromFinacleDbAsync(string accountNumber, DateTime fromDate, DateTime toDate, string correlationId = null)
        {
            _logger.LogInformation("Fetching statement from Oracle for {AccountNumber} from {FromDate} to {ToDate} [CorrelationID: {CorrelationId}]",
                accountNumber, fromDate, toDate, correlationId);

            try
            {
                DateTime startDate = fromDate.Date;
                DateTime endDate = toDate.Date.AddDays(1).AddSeconds(-1);
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var statementRecords = await GetStatementRecordsAsync(connection, accountNumber, startDate, endDate);
                    return await ConvertToFinacleDbDataTable(accountNumber, statementRecords, startDate, endDate);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch Oracle statement for {AccountNumber} [CorrelationID: {CorrelationId}]",
                    accountNumber, correlationId);
                throw;
            }
        }
        private async Task<(DataTable, decimal openingBalance, decimal closingBalance)> ConvertToFinacleDbDataTable(string accountNumber, List<StatementRecord> records, DateTime startDate, DateTime endDate)
        {
            var dt = new DataTable();

            dt.Columns.Add("AccountIdentification", typeof(string));
            dt.Columns.Add("AccountName", typeof(string));
            dt.Columns.Add("TransactionDate", typeof(string));
            dt.Columns.Add("Balance", typeof(decimal));
            dt.Columns.Add("Debit", typeof(decimal));
            dt.Columns.Add("Credit", typeof(decimal));
            dt.Columns.Add("CurrencyCode", typeof(string));
            dt.Columns.Add("TransactionValueDate", typeof(string));
            dt.Columns.Add("OpeningBalance", typeof(decimal));
            dt.Columns.Add("ClosingBalance", typeof(decimal));
            dt.Columns.Add("Remarks", typeof(string));

            decimal openingBalance = 0m;
            decimal closingBalance = 0m;
            using (var oraconn = new OracleConnection(System.Configuration.ConfigurationManager.ConnectionStrings["FinacleConn"].ToString()))
            {
                await oraconn.OpenAsync();

                openingBalance = await BalanceHelper.GetOpeningBalanceAsync(accountNumber, startDate, oraconn);
                closingBalance = await BalanceHelper.GetClosingBalanceAsync(accountNumber, endDate, oraconn);
                var row = dt.NewRow();
                row["ClosingBalance"] = closingBalance;
                row["OpeningBalance"] = openingBalance;

                //dt.Rows.Add(row);
                oraconn.Close();
            }

            foreach (var record in records)
            {
                var row = dt.NewRow();
                row["AccountIdentification"] = accountNumber;
                row["TransactionDate"] = record.TransactionDate;
                row["Debit"] = record.Debit;
                row["Credit"] = record.Credit;
                row["Balance"] = record.Balance;
                row["TransactionValueDate"] = record.TransactionValueDate;
                row["Remarks"] = record.Remarks ?? string.Empty;

                dt.Rows.Add(row);
            }
            return (dt, openingBalance, closingBalance);
        }
    }
}

