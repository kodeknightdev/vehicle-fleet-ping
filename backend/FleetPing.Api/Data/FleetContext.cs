using Microsoft.EntityFrameworkCore;

namespace FleetPing.Api.Data
{
    public class FleetContext : DbContext
    {
        public FleetContext(DbContextOptions<FleetContext> options) : base(options)
        {
        }
    }
}
