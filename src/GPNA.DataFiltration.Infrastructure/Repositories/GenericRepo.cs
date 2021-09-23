using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task<T> Add(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChanges();
            return entity;
        }

        public async Task Delete(T entity)
        {
            _dbSet.Remove(entity);
            await _dbContext.SaveChanges();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            var result = await _dbSet.ToListAsync();
            return result;
        }

        public async Task<T> GetById(long id)
        {
            var result = await _dbSet.FindAsync(id);
            return result;
        }

        public async Task Update(T entity)
        {
            _dbSet.Update(entity);
            await _dbContext.SaveChanges();
        }
    }
}
