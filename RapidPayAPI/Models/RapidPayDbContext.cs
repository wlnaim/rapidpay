using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RapidPayAPI.Models;

public partial class RapidPayDbContext : DbContext
{
    public RapidPayDbContext()
    {
    }

    public RapidPayDbContext(DbContextOptions<RapidPayDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Card> Cards { get; set; }

    public virtual DbSet<Fee> Fees { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=RapidPayDB;Trusted_Connection=True;Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasKey(e => e.CardNumber);

            entity.Property(e => e.CardNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Fee>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Fee1)
                .HasDefaultValueSql("((1))")
                .HasColumnType("decimal(3, 2)")
                .HasColumnName("Fee");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
