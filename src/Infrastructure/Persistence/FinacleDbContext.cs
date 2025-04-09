using Microsoft.EntityFrameworkCore;

namespace Finacle.Infrastructure.Persistence
{
    public class FinacleDbContext : DbContext
    {
        public FinacleDbContext(DbContextOptions<FinacleDbContext> options) : base(options) { }
        //public DbSet<Translog> Translogs { get; set; } = null!;
        public DbSet<ApiCall> ApiCalls { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApiCall>(entity =>
            {
                entity.HasIndex(e => e.RequestId, "IX_ApiCalls")
                    .IsUnique();

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Apiservice)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("APIService");

                entity.Property(e => e.Channel)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Controller)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EndPoint)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IpAddress)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ReponseTime).HasColumnType("datetime");

                entity.Property(e => e.RequestDate).HasColumnType("datetime");

                entity.Property(e => e.RequestId)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("RequestID");

                entity.Property(e => e.RequestParam)
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.ResponseCode)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.ResponseParam)
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.UserAgent)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Userid)
                    .HasMaxLength(15)
                    .IsUnicode(false);
            });

        }    

    }

    public class ApiCall
    {
        public int Id { get; set; }
        public string? Channel { get; set; }
        public string? RequestParam { get; set; }
        public string? IpAddress { get; set; }
        public string? Controller { get; set; }
        public string? EndPoint { get; set; }
        public DateTime? RequestDate { get; set; }
        public string? ResponseCode { get; set; }
        public string? ResponseParam { get; set; }
        public DateTime? ReponseTime { get; set; }
        public required string RequestId { get; set; }
        public string? Apiservice { get; set; }
        public decimal? Amount { get; set; }
        public string? UserAgent { get; set; }
        public string? Userid { get; set; }
        public string? TraceId { get; set; }
    }

    public partial class Translog
    {
        public Guid TransLogId { get; set; }
        public string? SourceAccount { get; set; }
        public string? DestinationAccount { get; set; }
        public string? Currency { get; set; }
        public decimal? Amount { get; set; }
        public decimal? PrevBal { get; set; }
        public decimal? PostBal { get; set; }
        public string? PayerName { get; set; }
        public string? PayeeName { get; set; }
        public string? Remarks { get; set; }
        public string? Status { get; set; }
        public DateTime? RequestTime { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? ResponseTime { get; set; }
        public string? ChannelId { get; set; }
        public string? ChannelReference { get; set; }
        public string? Message { get; set; }
        public string? CallbackUrl { get; set; }
        public string? CallbackStatus { get; set; }
        public DateTime? LastUpdate { get; set; }
        public bool? ChargeCustomer { get; set; }
        public string? ServiceChargeTransactionId { get; set; }
        public string? ServiceChargeEventId { get; set; }
        public decimal? ServiceCharge { get; set; }
        public bool IsReversed { get; set; }
    }
}