using System.Collections.Generic;
using System.Threading.Tasks;

namespace GPNA.DataFiltration.Application
{
    public interface IFilterConfigRepo: IGenericRepo<FilterConfig>
    {
        Task<IEnumerable<FilterConfig>> GetAllIncludePool();
    }
}
