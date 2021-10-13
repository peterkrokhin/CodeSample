using System.Collections.Generic;
using System.Linq;
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

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var result = await _dbSet.ToListAsync();
            return result;
        }

        public IEnumerable<T> GetAll()
        {
            var result = _dbSet.ToList();
            return result;
        }

        public T? GetById(long id)
        {
            var result = _dbSet.Find(id);
            return result;
        }

        public async Task<T?> GetByIdAsync(long id)
        {
            var result = await _dbSet.FindAsync(id);
            return result;
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            _dbContext.SaveChanges();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
