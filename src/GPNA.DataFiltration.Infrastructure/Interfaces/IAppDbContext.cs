using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GPNA.DataFiltration.Infrastructure
{
    public interface IAppDbContext
    {
        DbSet<T> Set<T>() where T : class;
        Task SaveChangesAsync();
        void SaveChanges();
    }
}
