using Microsoft.Data.SqlClient;
using Finacle.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Finacle.Infrastructure.Persistence.Repositories
{
    public interface ITransferRepository
    {
        Task<TransferEntity> CreateIfNotExistsAsync(string requestId, TransferRequest request);
        Task UpdateStatusAsync(string requestId, string status, string statusCode, string responsePayload);
    }

    public class TransferRepository : ITransferRepository
    {
        private readonly FinacleDbContext _context;

        public TransferRepository(FinacleDbContext context)
        {
            _context = context;
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
    }

}
