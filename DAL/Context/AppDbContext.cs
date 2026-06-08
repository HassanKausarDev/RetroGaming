// ============================================================
// FILE: RetroGaming.DAL/Context/AppDbContext.cs
// ============================================================
using Microsoft.EntityFrameworkCore;
using RetroGaming.DAL.Entities;

namespace RetroGaming.DAL.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Manufacturer> Manufacturers => Set<Manufacturer>();
        public DbSet<GameConsole> Consoles => Set<GameConsole>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ── Manufacturer ───────────────────────────────────────────────
            modelBuilder.Entity<Manufacturer>(entity =>
            {
                entity.ToTable("Manufacturers",
                    tb => tb.UseSqlOutputClause(false));

                // ConsoleCount is NOT a real database column.
                // It comes from stored procedure GROUP BY results.
                entity.Ignore(m => m.ConsoleCount);

                entity.HasIndex(m => m.Name).IsUnique();
                entity.Property(m => m.Name).HasMaxLength(100).IsRequired();
                entity.Property(m => m.Country).HasMaxLength(100).IsRequired();
                entity.Property(m => m.City).HasMaxLength(100).IsRequired();
                entity.Property(m => m.Latitude).HasColumnType("decimal(9,6)");
                entity.Property(m => m.Longitude).HasColumnType("decimal(9,6)");
                entity.Property(m => m.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(m => m.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // ── GameConsole ────────────────────────────────────────────────
            modelBuilder.Entity<GameConsole>(entity =>
            {
                entity.ToTable("Consoles",
                    tb => tb.UseSqlOutputClause(false));

                // ManufacturerName is NOT a real database column.
                // It comes from the JOIN in stored procedure results.
                entity.Ignore(c => c.ManufacturerName);

                entity.HasIndex(c => new { c.ManufacturerId, c.Name }).IsUnique();
                entity.Property(c => c.Name).HasMaxLength(100).IsRequired();
                entity.Property(c => c.UnitsSoldMillions).HasColumnType("decimal(10,2)");
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(c => c.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(c => c.Manufacturer)
                      .WithMany(m => m.Consoles)
                      .HasForeignKey(c => c.ManufacturerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}