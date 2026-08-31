using Finacle.Core.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

namespace Finacle.Infrastructure.Persistence.Repositories
{
    public interface ITransferRepository
    {
        Task<TransferEntity> CreateIfNotExistsAsync(string requestId, TransferRequest request);
        Task UpdateStatusAsync(string requestId, string status, string statusCode, string responsePayload);
        Task<IntraBankTransferResponse?> GetTransactionAsync(string refNo);
        Task<PagedResult<AdapterTransactionPage>> GetAccountTransactionsAsync(
            string accountId,
            int pageNumber,
            int pageSize,
            DateTime? fromDate = null,
            DateTime? toDate = null);
    }

    public class TransferRepository : ITransferRepository
    {
        private readonly ILogger<TransferRepository> _logger;
        private readonly FinacleDbContext _context;
        private readonly IConfiguration _configuration;

        public TransferRepository(ILogger<TransferRepository> logger,IConfiguration configuration, FinacleDbContext context)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        public async Task<TransferEntity> CreateIfNotExistsAsync(
            string requestId,
            TransferRequest request)
        {
            return await _context.Transfers
                .FromSqlRaw(
                    @"EXEC sp_CreateTransferIfNotExists 
                    @RequestId,
                    @SourceAccount,
                    @DestinationAccount,
                    @Amount,
                    @Currency,
                    @Remarks,
                    @ChannelReference,
                    @ChannelId,
                    @DocNum,
                    @UserPartTransactionCode,
                    @ChargeEventId",

                    new SqlParameter("@RequestId", requestId),
                    new SqlParameter("@SourceAccount", request.SourceAccountId),
                    new SqlParameter("@DestinationAccount", request.DestinationAccountId),
                    new SqlParameter("@Amount", request.Amount),
                    new SqlParameter("@Currency", request.Currency),
                    new SqlParameter("@Remarks", (object?)request.Remarks ?? DBNull.Value),
                    new SqlParameter("@ChannelReference", request.ChannelReference),
                    new SqlParameter("@ChannelId", request.ChannelId),
                    new SqlParameter("@DocNum", (object?)request.DocNum ?? DBNull.Value),
                    new SqlParameter("@UserPartTransactionCode", (object?)request.UserPartTransactionCode ?? DBNull.Value),
                    new SqlParameter("@ChargeEventId", (object?)request.ChargeEventId ?? DBNull.Value)
                )
                .AsNoTracking()
                .FirstAsync();
        }

        public async Task UpdateStatusAsync(
            string requestId,
            string status,
            string statusCode,
            string responsePayload)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"EXEC sp_UpdateTransferStatus 
                @RequestId,
                @Status,
                @StatusCode,
                @ResponsePayload",

                new SqlParameter("@RequestId", requestId),
                new SqlParameter("@Status", status),
                new SqlParameter("@StatusCode", statusCode),
                new SqlParameter("@ResponsePayload",
                    responsePayload ?? (object)DBNull.Value));
        }

        public async Task<IntraBankTransferResponse?> GetTransactionAsync(string refNo)
        {
            if (string.IsNullOrWhiteSpace(refNo))
                throw new ArgumentException("Reference number is required.", nameof(refNo));

            if (refNo.Length > 20)
                throw new ArgumentException("Reference number cannot exceed 20 characters.", nameof(refNo));

            IntraBankTransferResponse? response = null;

            try
            {
                string connectionString =
            _configuration.GetConnectionString("FinacleConn") ?? throw new InvalidOperationException("Connection string 'FinacleConn' is not configured");

                using OracleConnection conn =
                    new OracleConnection(connectionString);

                await conn.OpenAsync();

                string query =
                    @"
                SELECT
                    tran_id,
                    tran_date
                FROM
                    tbaadm.dtd
                WHERE
                    REF_NUM = :channelReference
                    AND PART_TRAN_TYPE = 'D'
                    AND del_flg = 'N'
                ORDER BY
                    tran_date DESC";

                using OracleCommand cmd =
                    new OracleCommand(query, conn);

                cmd.Parameters.Add(new OracleParameter("channelReference", refNo));

                using OracleDataReader reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    response = new IntraBankTransferResponse
                    {
                        TransactionReference = reader["tran_id"]?.ToString(),
                        TransactionDate = Convert.ToDateTime(reader["tran_date"]).ToString("yyyy-MM-dd"),
                        TranStatus = ""
                    };
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An exception occurred at GetTransactionAsync with message: {ex.Message}, stackTrace: {ex.StackTrace}");
                return default;
            }
        }

        public async Task<PagedResult<AdapterTransactionPage>> GetAccountTransactionsAsync(
            string accountId,
            int pageNumber,
            int pageSize,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var offset = (pageNumber - 1) * pageSize;
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            var startDate = (fromDate ?? (toDate ?? DateTime.UtcNow).AddDays(-90)).ToString("dd-MMM-yyyy").ToUpper();
            var endDate = (toDate ?? DateTime.UtcNow).ToString("dd-MMM-yyyy").ToUpper();

            const string sql = @"
            SELECT 
                VALUE_DATE AS TransactionValueDate,
                TRA_DATE AS TransactionDate,
                DEBIT AS Debit,
                CREDIT AS Credit,
                RUNNING_BALANCE AS Balance,
                DESCRIPTION AS Remarks,
                REF_NUM AS Reference
            FROM TABLE(custom.get_account_statement_pipelined(
                p_foracid    => :Foracid,
                p_start_date => :StartDate,
                p_end_date   => :EndDate
            ))
            OFFSET (:Offset) ROWS
            FETCH NEXT (:PageSize) ROWS ONLY";

            var transactions = new List<AdapterTransactionPage>();
            string _connectionString =
                _configuration.GetConnectionString("FinacleConn") ?? 
                throw new InvalidOperationException("Connection string 'FinacleConn' is not configured");

            await using var connection = new OracleConnection(_connectionString);

            try
            {
                await connection.OpenAsync();

                await using var command = new OracleCommand(sql, connection);

                command.Parameters.Add("Foracid", OracleDbType.Varchar2).Value = accountId;
                command.Parameters.Add("StartDate", OracleDbType.Varchar2).Value = startDate;
                command.Parameters.Add("EndDate", OracleDbType.Varchar2).Value = endDate;
                command.Parameters.Add("Offset", OracleDbType.Int32).Value = offset;
                command.Parameters.Add("PageSize", OracleDbType.Int32).Value = pageSize;

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    transactions.Add(new AdapterTransactionPage
                    {
                        TransactionValueDate = reader["TransactionValueDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["TransactionValueDate"]),
                        TransactionDate = reader["TransactionDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["TransactionDate"]),
                        Debit = reader["Debit"] == DBNull.Value ? null : Convert.ToDecimal(reader["Debit"]),
                        Credit = reader["Credit"] == DBNull.Value ? null : Convert.ToDecimal(reader["Credit"]),
                        Balance = reader["Balance"] == DBNull.Value ? null : Convert.ToDecimal(reader["Balance"]),
                        Reference = reader["Reference"] == DBNull.Value ? null : reader["Reference"].ToString(),
                        Remarks = reader["Remarks"] == DBNull.Value ? null : reader["Remarks"].ToString()
                    });
                }

                return new PagedResult<AdapterTransactionPage>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Data = transactions
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"An exception occurred at GetTransactionAsync with message: {ex.Message}, stackTrace: {ex.StackTrace}");
                return default;
            }
        }
    }

    public class PagedResult<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public List<T> Data { get; set; } = new();
    }

    public class AdapterTransactionPage
    {
        public DateTime? TransactionValueDate { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Balance { get; set; }
        public string? Remarks { get; set; }
        public string? Reference { get; internal set; }
    }
}
