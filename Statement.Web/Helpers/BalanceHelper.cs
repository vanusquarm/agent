using Oracle.ManagedDataAccess.Client;
using Serilog;
using System.Globalization;

namespace Statement.Web.Helpers
{
    public static class BalanceHelper
    {
        private static readonly DateTime CutoverDate = new DateTime(2025, 5, 21, 0, 0, 0, DateTimeKind.Utc);

        private static string FormatOracleDate(DateTime date)
        {
            return date.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture).ToUpper();
        }

        public static async Task<decimal> GetOpeningBalanceAsync(string foracid, DateTime startDate, OracleConnection finacleConn)
        {
            try
            {
                string query;
                OracleConnection conn;
                string formattedDate = FormatOracleDate(startDate);

                // If startDate is before cutover, use BASIS; otherwise, use Finacle
                if (startDate < CutoverDate)
                {
                    query = @"
                    SELECT available_amt AS BALANCE
                    FROM (
                        SELECT available_amt
                        FROM tbaadm.bth
                        WHERE tran_date <= TO_DATE(:startDate, 'DD-MON-YYYY')
                        AND foracid = :foracid
                        ORDER BY entry_date DESC, tran_id DESC, PART_TRAN_SRL_NUM DESC
                    )
                    WHERE ROWNUM = 1";
                    conn = finacleConn;
                }
                else
                {
                    //query = @"
                    //SELECT NVL(a.tran_date_bal, 0) AS BALANCE
                    //FROM tbaadm.eab a
                    //JOIN tbaadm.gam b ON a.acid = b.acid
                    //WHERE TRIM(b.foracid) = :foracid
                    //AND a.eod_date = (
                    //    SELECT MAX(c.eod_date)
                    //    FROM tbaadm.eab c
                    //    JOIN tbaadm.gam d ON c.acid = d.acid
                    //    WHERE TRIM(d.foracid) = :foracid
                    //    AND c.eod_date <= TO_DATE(:startDate, 'DD-MON-YYYY')
                    //)";
                    query = @"SELECT NVL(a.tran_date_bal, 0) AS BALANCE
                    FROM tbaadm.eab a
                    JOIN tbaadm.gam b ON a.acid = b.acid
                    WHERE TRIM(b.foracid) = :foracid
                    AND a.eod_date = (
                         SELECT MAX(c.eod_date)
                         FROM tbaadm.eab c
                         JOIN tbaadm.gam d ON c.acid = d.acid
                         WHERE TRIM(d.foracid) = :foracid
                         AND (c.eod_date+1) <= TO_DATE(:startDate, 'DD-MON-YYYY')
                    )";
                    conn = finacleConn;
                }

                using (var cmd = new OracleCommand(query, conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add(new OracleParameter(":foracid", OracleDbType.Varchar2)).Value = foracid;
                    cmd.Parameters.Add(new OracleParameter(":startDate", OracleDbType.Varchar2)).Value = formattedDate;

                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        decimal balance = await dr.ReadAsync() && dr["BALANCE"] != DBNull.Value
                            ? Convert.ToDecimal(dr["BALANCE"])
                            : 0M;

                        Log.Information("Retrieved opening balance {Balance} for account {Foracid}", balance, foracid);
                        return balance;
                    }
                }
            }
            catch (OracleException ex)
            {
                Log.Error(ex, "Oracle error fetching opening balance for account {Foracid}", foracid);
                throw new ApplicationException("Database error fetching opening balance", ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error fetching opening balance for account {Foracid}", foracid);
                throw;
            }
        }

        public static async Task<decimal> GetClosingBalanceAsync(string foracid, DateTime endDate, OracleConnection finacleConn)
        {
            try
            {
                string query;
                OracleConnection conn;
                //string formattedDate = FormatOracleDate(endDate.AddDays(1));
                string formattedDate = FormatOracleDate(endDate);

                // If endDate is before cutover, or startDate is before cutover (for mixed periods), use BASIS
                // Assuming startDate is available in the calling context; here we only check endDate
                // If startDate is needed, it should be passed and checked
                if (endDate < CutoverDate)
                {
                    query = @"
                    SELECT available_amt AS BALANCE
                    FROM (
                        SELECT available_amt
                        FROM tbaadm.bth
                        WHERE tran_date <= TO_DATE(:endDate, 'DD-MON-YYYY')
                        AND foracid = :foracid
                        ORDER BY entry_date DESC, tran_id DESC, PART_TRAN_SRL_NUM DESC
                    )
                    WHERE ROWNUM = 1";
                    conn = finacleConn;
                }
                else
                {
                    query = @"
                    SELECT NVL(a.tran_date_bal, 0) AS BALANCE
                    FROM tbaadm.eab a
                    JOIN tbaadm.gam b ON a.acid = b.acid
                    WHERE TRIM(b.foracid) = :foracid
                    AND a.eod_date = (
                        SELECT MAX(c.eod_date)
                        FROM tbaadm.eab c
                        JOIN tbaadm.gam d ON c.acid = d.acid
                        WHERE TRIM(d.foracid) = :foracid
                        AND c.eod_date <= TO_DATE(:endDate, 'DD-MON-YYYY')
                    )";
                    conn = finacleConn;
                }

                Log.Information("Executing GetClosingBalanceAsync for account {Foracid}, date {EndDate}, using {Database}",
                    foracid, formattedDate, endDate < CutoverDate ? "BASIS" : "Finacle");

                using (var cmd = new OracleCommand(query, conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add(new OracleParameter(":foracid", OracleDbType.Varchar2)).Value = foracid;
                    cmd.Parameters.Add(new OracleParameter(":endDate", OracleDbType.Varchar2)).Value = formattedDate;

                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        decimal balance = await dr.ReadAsync() && dr["BALANCE"] != DBNull.Value
                            ? Convert.ToDecimal(dr["BALANCE"])
                            : 0M;

                        Log.Information("Retrieved closing balance {Balance} for account {Foracid}", balance, foracid);
                        return balance;
                    }
                }
            }
            catch (OracleException ex)
            {
                Log.Error(ex, "Oracle error fetching closing balance for account {Foracid}", foracid);
                throw new ApplicationException("Database error fetching closing balance", ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error fetching closing balance for account {Foracid}", foracid);
                throw;
            }
        }
    }
}
