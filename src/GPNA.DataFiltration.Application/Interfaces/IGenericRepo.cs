using System.Collections.Generic;
using System.Threading.Tasks;

namespace GPNA.DataFiltration.Application
{
    public interface IGenericRepo<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(long id);
        Task Update(T entity);
        Task<T> Add(T entity);
        Task Delete(T entity);

    }
}
