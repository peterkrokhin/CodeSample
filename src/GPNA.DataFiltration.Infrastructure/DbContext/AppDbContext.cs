using System.Threading.Tasks;
using GPNA.DataFiltration.Application;
using Microsoft.EntityFrameworkCore;

namespace GPNA.DataFiltration.Infrastructure
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public DbSet<FilterPool> FilterPools { get; set; }
        public DbSet<FilterConfig> FilterConfigs { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }

        public new async Task SaveChanges()
        {
            await base.SaveChangesAsync();
        }
    }
}
