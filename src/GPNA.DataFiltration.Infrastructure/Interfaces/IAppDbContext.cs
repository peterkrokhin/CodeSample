using Microsoft.EntityFrameworkCore;

namespace GPNA.DataFiltration.Infrastructure
{
    public interface IAppDbContext
    {
        DbSet<T> Set<T>() where T : class;
    }
}
