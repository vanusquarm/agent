using Oracle.ManagedDataAccess.Client;
using Statement.Web.Interfaces;
using System.Configuration;

namespace Statement.Web.Services
{
    public class BasisFallbackService : IBasisFallbackService
    {
        private readonly ILogger<BasisFallbackService> _logger;
        private readonly string _connectionString;

        public BasisFallbackService(ILogger<BasisFallbackService> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionString = configuration.GetConnectionString("BasisConn") ?? throw new InvalidOperationException("Database connection string is not configured.");

        }

        public async Task<string> GetFinacleAccountFromBasisAccount(string basisAccount)
        {
            string accountNum = string.Empty;
            try
            {
                using (OracleConnection oraconn = new OracleConnection(_connectionString))
                {
                    string query = "";

                    if (basisAccount.Length == 12)
                    {
                        query = "select * from banksys.new_account_structure where bra_code =" + basisAccount.Substring(0, 3) + " and cus_num = " + basisAccount.Substring(3, 6) + " and cur_code = " + basisAccount.Substring(9, 1) + " and led_code = " + basisAccount.Substring(10, 1) + " and SUB_ACCT_CODE = " + basisAccount.Substring(11, 1);
                    }
                    else if (basisAccount.Length > 12)
                    {
                        query = "select * from banksys.new_account_structure where bra_code =" + basisAccount.Substring(0, 3) + " and cus_num = " + basisAccount.Substring(3, 6) + " and cur_code = " + basisAccount.Substring(9, 1) + " and led_code = " + basisAccount.Substring(10, 2) + " and SUB_ACCT_CODE = " + basisAccount.Substring(12, 1);
                    }
                    else if (basisAccount.Contains("/"))
                    {
                        string[] fullacctkey = basisAccount.Split('/');
                        query = "select * from banksys.new_account_structure where bra_code =" + fullacctkey[0] + " and cus_num = " + fullacctkey[1] + " and cur_code = " + fullacctkey[2] + " and led_code = " + fullacctkey[3] + " and SUB_ACCT_CODE = " + fullacctkey[4];
                    }

                    await oraconn.OpenAsync().ConfigureAwait(false);

                    using (OracleCommand cmd = new OracleCommand(query, oraconn))
                    using (OracleDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            accountNum = reader["NEW_ACCOUNT"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}: An error occurred while reading oracle db for finacle acc. -- {ex.StackTrace}");
            }

            return accountNum;
        }

    }
}

