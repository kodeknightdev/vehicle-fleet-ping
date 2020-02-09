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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vehicle>(e =>
            {
                e.Property(v => v.Status)
                    .HasConversion<string>()
                    .HasMaxLength(16);
            });
        }
    }
}
