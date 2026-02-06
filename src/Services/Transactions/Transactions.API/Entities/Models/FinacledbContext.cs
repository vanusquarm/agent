using System;
using System.Collections.Generic;
using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Transactions.API.Entities.Models
{
    public partial class FinacleDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public FinacleDbContext()
        {
        }

        public FinacleDbContext(DbContextOptions<FinacleDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public virtual DbSet<Translog> Translogs { get; set; } = null!;
        public virtual DbSet<Call> ApiCalls { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Translog>(entity =>
            {
                entity.ToTable("translog");

                entity.Property(e => e.Amount).HasColumnType("decimal(15, 2)");

                entity.Property(e => e.CallbackStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CallbackUrl)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("CallbackURL");

                entity.Property(e => e.ChannelId)
                    .HasMaxLength(50)
                    .IsUnicode(false); 

                entity.Property(e => e.ChannelReference)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Currency)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.DestinationAccount)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.EntryDate).HasColumnType("datetime");

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.Message)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.PayeeName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PayerName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PostBal).HasColumnType("decimal(15, 2)");

                entity.Property(e => e.PrevBal).HasColumnType("decimal(15, 2)");

                entity.Property(e => e.Remarks)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.RequestTime).HasColumnType("datetime");

                entity.Property(e => e.ResponseTime).HasColumnType("datetime");

                entity.Property(e => e.ServiceCharge).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ServiceChargeTransactionId)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ServiceChargeEventId)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.SourceAccount)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
