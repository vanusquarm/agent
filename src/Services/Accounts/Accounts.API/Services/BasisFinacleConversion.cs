using Accounts.API.Entities.DTOs;
using Oracle.ManagedDataAccess.Client;
using Serilog;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Accounts.API.Services
{

    public interface IBasisClient
    {
        Task<FinacleAccount> ConvertToFinacleAccount(BasisAccount data);
    }   
    public class BasisFinacleConversion: IBasisClient
    {
        private string _connectionString;

        public BasisFinacleConversion(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("FinacleConn") ?? throw new InvalidOperationException("Database connection string is not configured.");
        }

        public  async Task<FinacleAccount> ConvertToFinacleAccount(BasisAccount data)
        {
            string query = "";
            OracleConnection conn= new OracleConnection(_connectionString);
            var finacleAccount = new FinacleAccount();

            try
            {
                await conn.OpenAsync();

                query = $"SELECT * FROM cifuser.new_account_structure WHERE BRA_CODE =:bracode AND CUS_NUM =:cusnum  AND CUR_CODE = :curcode AND LED_CODE = :ledcode AND SUB_ACCT_CODE = :subacct";
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add(new OracleParameter(":bracode", OracleDbType.Varchar2) { Value = data.BranchCode });
                    cmd.Parameters.Add(new OracleParameter(":cusnum", OracleDbType.Varchar2) { Value = data.CustomerNumber });
                    cmd.Parameters.Add(new OracleParameter(":curcode", OracleDbType.Varchar2) { Value = data.CurrencyCode });
                    cmd.Parameters.Add(new OracleParameter(":ledcode", OracleDbType.Varchar2) { Value = data.LedgerCode });
                    cmd.Parameters.Add(new OracleParameter(":subacct", OracleDbType.Varchar2) { Value = data.SubAccount });

                    using (OracleDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            finacleAccount.FinacleAccountNumber = reader["NEW_ACCOUNT"]?.ToString();
                        }
                    }
                }

                 
            return finacleAccount;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Oracle error fetching corresponding account ", data.BranchCode+data.CustomerNumber+data.CurrencyCode+data.LedgerCode+data.SubAccount);
                throw new ApplicationException("Database error fetching opening balance", ex);
            }
             

            
        }

    }
}
