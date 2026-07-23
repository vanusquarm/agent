using Accounts.API.Entities.DTOs;
using Finacle.Core.Domain;
using Oracle.ManagedDataAccess.Client;
using Serilog;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Accounts.API.Services
{

    public interface IBasisClient
    {
        Task<FinacleAccount> ConvertToFinacleAccount(FinacleAccount data);
    }
    public class BasisClient : IBasisClient
    {
        private string _connectionString;

        public BasisClient(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("FinacleConn") ?? throw new InvalidOperationException("Database connection string is not configured.");
        }

        //public  Task<FinacleAccount> ConvertToFinacleAccount(FinacleAccount data)
        //{
        //    //string query = "";
        //    //OracleConnection conn= new OracleConnection(_connectionString);
        //    var finacleAccount = new FinacleAccount();

        //    try
        //    {
        //        // Ensure the connection is disposed/closed properly
        //        using (var conn = new OracleConnection(_connectionString))
        //        {
        //            conn.Open();

        //            string query = string.Empty;
        //            var parameters = new List<OracleParameter>();

        //            // Parse inputs before initializing the command
        //            if (data.AccountNumber.Length == 12)
        //            {
        //                query = "SELECT * FROM cifuser.new_account_structure WHERE BRA_CODE = :bracode AND CUS_NUM = :cusnum AND CUR_CODE = :curcode AND LED_CODE = :ledcode AND SUB_ACCT_CODE = :subacct";
        //                parameters.Add(new OracleParameter("bracode", OracleDbType.Varchar2) { Value = data.AccountNumber.Substring(0, 3) });
        //                parameters.Add(new OracleParameter("cusnum", OracleDbType.Varchar2) { Value = data.AccountNumber.Substring(3, 6) });
        //                parameters.Add(new OracleParameter("curcode", OracleDbType.Varchar2) { Value = data.AccountNumber.Substring(9, 1) });
        //                parameters.Add(new OracleParameter("ledcode", OracleDbType.Varchar2) { Value = data.AccountNumber.Substring(10, 1) });
        //                parameters.Add(new OracleParameter("subacct", OracleDbType.Varchar2) { Value = data.AccountNumber.Substring(11, 1) });
        //            }
        //            else if (data.AccountNumber.Length > 12 && !data.AccountNumber.Contains("/"))
        //            {
        //                query = "SELECT * FROM cifuser.new_account_structure WHERE BRA_CODE = :bracode AND CUS_NUM = :cusnum AND CUR_CODE = :curcode AND LED_CODE = :ledcode AND SUB_ACCT_CODE = :subacct";
        //                parameters.Add(new OracleParameter("bracode", OracleDbType.Varchar2) { Value = data.AccountNumber.Substring(0, 3) });
        //                parameters.Add(new OracleParameter("cusnum", OracleDbType.Varchar2) { Value = data.AccountNumber.Substring(3, 6) });
        //                parameters.Add(new OracleParameter("curcode", OracleDbType.Varchar2) { Value = data.AccountNumber.Substring(9, 1) });

        //                // Note: Substring(10, 2) takes 2 chars starting at index 10 (indexes 10 & 11)
        //                parameters.Add(new OracleParameter("ledcode", OracleDbType.Varchar2) { Value = data.AccountNumber.Substring(10, 2) });

        //                // Note: Substring(12, 1) grabs the character after the 2-digit ledcode
        //                parameters.Add(new OracleParameter("subacct", OracleDbType.Varchar2) { Value = data.AccountNumber.Substring(12, 1) });
        //            }
        //            else if (data.AccountNumber.Contains("/"))
        //            {
        //                var parts = data.AccountNumber.Split('/');
        //                if (parts.Length >= 5)
        //                {
        //                    query = "SELECT * FROM cifuser.new_account_structure WHERE BRA_CODE = :bracode AND CUS_NUM = :cusnum AND CUR_CODE = :curcode AND LED_CODE = :ledcode AND SUB_ACCT_CODE = :subacct";
        //                    parameters.Add(new OracleParameter("bracode", OracleDbType.Varchar2) { Value = parts[0] });
        //                    parameters.Add(new OracleParameter("cusnum", OracleDbType.Varchar2) { Value = parts[1] });
        //                    parameters.Add(new OracleParameter("curcode", OracleDbType.Varchar2) { Value = parts[2] });
        //                    parameters.Add(new OracleParameter("ledcode", OracleDbType.Varchar2) { Value = parts[3] });
        //                    parameters.Add(new OracleParameter("subacct", OracleDbType.Varchar2) { Value = parts[4] });
        //                }
        //            }

        //            // Only execute if a valid branch match was found
        //            if (!string.IsNullOrEmpty(query))
        //            {
        //                 using (OracleCommand cmd = new OracleCommand(query, conn))
        //                {
        //                    cmd.Parameters.AddRange(parameters.ToArray());

        //                    using (OracleDataReader reader = (OracleDataReader)cmd.ExecuteReader())
        //                    {
        //                        while ( reader.Read())
        //                        {
        //                            finacleAccount.AccountNumber = reader["NEW_ACCOUNT"]?.ToString();
        //                        }
        //                    }
        //                }
        //            }
        //        }


        //        return finacleAccount;

        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, "Oracle error fetching corresponding account ",data.AccountNumber);
        //        throw new ApplicationException("Database error fetching opening balance", ex);
        //    }

        public async Task<FinacleAccount> ConvertToFinacleAccount(FinacleAccount data)
        {
            string query = "";
            OracleConnection conn = new OracleConnection(_connectionString);
            var finacleAccount = new FinacleAccount();

            try
            {
                await conn.OpenAsync();

                query = $"SELECT * FROM cifuser.new_account_structure WHERE BRA_CODE =:bracode AND CUS_NUM =:cusnum  AND CUR_CODE = :curcode AND LED_CODE = :ledcode AND SUB_ACCT_CODE = :subacct";
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    if (data.AccountNumber.Length == 12)
                    {
                        cmd.Parameters.Add(new OracleParameter(":bracode", OracleDbType.Varchar2) { Value = data.AccountNumber.Substring(0, 3) });
                        cmd.Parameters.Add(new OracleParameter(":cusnum", OracleDbType.Varchar2) { Value = data.AccountNumber.Substring(3, 6) });
                        cmd.Parameters.Add(new OracleParameter(":curcode", OracleDbType.Varchar2) { Value = data.AccountNumber.Substring(9, 1) });
                        cmd.Parameters.Add(new OracleParameter(":ledcode", OracleDbType.Varchar2) { Value = data.AccountNumber.Substring(10, 1) });
                        cmd.Parameters.Add(new OracleParameter(":subacct", OracleDbType.Varchar2) { Value = data.AccountNumber.Substring(11, 1) });
                    }
                    else
                    {
                        cmd.Parameters.Add(new OracleParameter(":bracode", OracleDbType.Varchar2) { Value = data.AccountNumber.Substring(0, 3) });
                        cmd.Parameters.Add(new OracleParameter(":cusnum", OracleDbType.Varchar2) { Value = data.AccountNumber.Substring(3, 6) });
                        cmd.Parameters.Add(new OracleParameter(":curcode", OracleDbType.Varchar2) { Value = data.AccountNumber.Substring(9, 1) });
                        cmd.Parameters.Add(new OracleParameter(":ledcode", OracleDbType.Varchar2) { Value = data.AccountNumber.Substring(10, 2) });
                        cmd.Parameters.Add(new OracleParameter(":subacct", OracleDbType.Varchar2) { Value = data.AccountNumber.Substring(12, 1) });
                    }
                    using (OracleDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            finacleAccount.AccountNumber = reader["NEW_ACCOUNT"]?.ToString();
                        }
                    }
                }


                return finacleAccount;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Oracle error fetching corresponding account ", data.AccountNumber);
                throw new ApplicationException("Database error fetching opening balance", ex);
            }
             
        }

    }

}

