using GPNA.DataFiltration.Application;
using Microsoft.EntityFrameworkCore;

namespace GPNA.DataFiltration.Infrastructure
{
    abstract public class GenericRepo<T> : IGenericRepo<T> where T : class
    {
        protected readonly IAppDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public GenericRepo(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }
    }
}
