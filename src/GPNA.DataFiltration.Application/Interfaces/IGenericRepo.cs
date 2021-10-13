using System.Collections.Generic;
using System.Threading.Tasks;

namespace GPNA.DataFiltration.Application
{
    public interface IGenericRepo<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        IEnumerable<T> GetAll();
        Task<T?> GetByIdAsync(long id);
        T? GetById(long id);
        Task UpdateAsync(T entity);
        void Update(T entity);
        Task<T> AddAsync(T entity);
        Task DeleteAsync(T entity);

    }
}
