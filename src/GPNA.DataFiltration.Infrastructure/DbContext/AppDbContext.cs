using GPNA.DataFiltration.Application;
using Microsoft.EntityFrameworkCore;

namespace GPNA.DataFiltration.Infrastructure
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public DbSet<FilterPool> FilterPool { get; set; }
        public DbSet<FilterConfig> FilterConfig { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }
    }
}
