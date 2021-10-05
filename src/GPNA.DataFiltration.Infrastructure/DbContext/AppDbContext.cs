using System.Threading.Tasks;
using GPNA.DataFiltration.Application;
using Microsoft.EntityFrameworkCore;

namespace GPNA.DataFiltration.Infrastructure
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public DbSet<FilterPool> FilterPools { get; set; } = null!;
        public DbSet<FilterConfig> FilterConfigs { get; set; } = null!;
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }

        public async Task SaveChangesAsync()
        {
            await base.SaveChangesAsync();
        }

        public new void SaveChanges()
        {
            base.SaveChanges();
        }
    }
}
