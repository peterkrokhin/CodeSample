using System.Collections.Generic;
using System.Threading.Tasks;

namespace GPNA.DataFiltration.Application
{
    public interface IFilterPoolRepo: IGenericRepo<FilterPool>
    {
        Task<IEnumerable<FilterPool>> GetAllIncludeFilterConfigs();
    }
}
