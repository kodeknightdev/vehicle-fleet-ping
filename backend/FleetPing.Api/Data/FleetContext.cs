using FleetPing.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetPing.Api.Data
{
    public class FleetContext : DbContext
    {
        public FleetContext(DbContextOptions<FleetContext> options) : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<MaintenanceLog> MaintenanceLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vehicle>(e =>
            {
                e.Property(v => v.Status)
                    .HasConversion<string>()
                    .HasMaxLength(16);

                e.HasIndex(v => v.Plate).IsUnique();
            });

            modelBuilder.Entity<MaintenanceLog>(e =>
            {
                e.Property(m => m.Type)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                e.HasOne(m => m.Vehicle)
                    .WithMany(v => v.MaintenanceLogs)
                    .HasForeignKey(m => m.VehicleId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(m => new { m.VehicleId, m.Date });
            });
        }
    }
}
