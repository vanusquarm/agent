using Finacle.Core.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
namespace Finacle.Infrastructure.Persistence.Repositories
{
    public interface ITransferRepository
    {
        Task<TransferEntity> CreateIfNotExistsAsync(string requestId, TransferRequest request);
        Task UpdateStatusAsync(string requestId, string status, string statusCode, string responsePayload);
        Task<IntraBankTransferResponse?> GetTransactionAsync(string accountId, string entryDate, string refNo, string tranInd);
        Task<IntraBankTransferResponse?> GetTransactionAsync(string refNo);
    }

    public class TransferRepository : ITransferRepository
    {
        private readonly FinacleDbContext _context;
        private readonly IConfiguration _configuration;

        public TransferRepository(IConfiguration configuration, FinacleDbContext context)
        {
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


        public async Task<IntraBankTransferResponse?> GetTransactionAsync(string accountId, string entryDate, string refNo, string tranInd)
        {
            if (string.IsNullOrWhiteSpace(refNo))
                throw new ArgumentException("Reference number is required.", nameof(refNo));

            if (refNo.Length > 20)
                throw new ArgumentException("Reference number cannot exceed 20 characters.", nameof(refNo));

            IntraBankTransferResponse? response = null;
            DateTime date = DateTime.Parse(entryDate);

            string connectionString =
                _configuration.GetConnectionString("FinacleConn");

            using OracleConnection conn =
                new OracleConnection(connectionString);

            await conn.OpenAsync();

            string query =
                @"
                SELECT
                    tran_id,
                    tran_date,
                    value_date,
                    part_tran_type,
                    tran_amt,
                    tran_particular
                FROM
                    (
                        SELECT * FROM htd
                        UNION ALL
                        SELECT * FROM dtd
                    )
                WHERE
                    ENT_ACCOUNT_ID = :accountId
                    AND ENTRY_DATE = :entryDate
                    AND REF_NUM = :channelReference
                    AND PART_TRAN_TYPE = :tranInd
                    AND del_flg = 'N'
                ORDER BY
                    tran_date DESC";

            using OracleCommand cmd =
                new OracleCommand(query, conn);

            cmd.Parameters.Add(new OracleParameter("accountId", accountId));
            cmd.Parameters.Add(new OracleParameter("entryDate", date.ToString("dd-MMM-yyyy").ToLower()));
            cmd.Parameters.Add(new OracleParameter("channelReference", refNo));
            cmd.Parameters.Add(new OracleParameter("tranInd", tranInd));

            using OracleDataReader reader = cmd.ExecuteReader();

            if (await reader.ReadAsync())
            {
                response = new IntraBankTransferResponse
                {
                    TransactionDate = reader["tran_id"].ToString(),
                    TransactionReference = reader["tran_date"].ToString(),
                    TranStatus = ""
                };
            }

            return response;
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
            _configuration.GetConnectionString("FinacleConn");

                using OracleConnection conn =
                    new OracleConnection(connectionString);

                await conn.OpenAsync();

                string query =
                    @"
                SELECT
                    tran_id,
                    tran_date
                FROM
                    (
                        SELECT * FROM tbaadm.htd
                        UNION ALL
                        SELECT * FROM tbaadm.dtd
                    )
                WHERE
                    REF_NUM = :channelReference
                    AND PART_TRAN_TYPE = 'D'
                    AND del_flg = 'N'
                ORDER BY
                    tran_date DESC";

                using OracleCommand cmd =
                    new OracleCommand(query, conn);

                cmd.Parameters.Add(new OracleParameter("channelReference", refNo));

                using OracleDataReader reader = cmd.ExecuteReader();

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
                throw;
            }
        }
    }

}
