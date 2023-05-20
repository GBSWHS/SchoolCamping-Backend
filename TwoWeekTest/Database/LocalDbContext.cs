using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TwoWeekTest.Database
{
    public partial class LocalDbContext : DbContext
    {
        public LocalDbContext()
        {
        }

        public LocalDbContext(DbContextOptions<LocalDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Reserves> Reserves { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=127.0.0.1;database=schoolcamping;user id=local", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.6.0-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_general_ci")
                .HasCharSet("utf8mb4");
            modelBuilder.Entity<Reserves>(entity =>
            {
                entity.ToTable("reserves");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Mates)
                    .HasMaxLength(50)
                    .HasColumnName("mates");

                entity.Property(e => e.Passcode)
                    .HasMaxLength(4)
                    .HasColumnName("passcode")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.ReservedAt)
                    .HasColumnName("reserved_at")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.Teacher)
                    .HasMaxLength(4)
                    .HasColumnName("teacher");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
