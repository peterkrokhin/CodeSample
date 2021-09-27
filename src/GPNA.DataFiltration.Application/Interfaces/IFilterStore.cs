using System.Collections.Generic;
using System.Threading.Tasks;

namespace GPNA.DataFiltration.Application
{
    public interface IFilterStore
    {
        Task StartCyclicUpdate();
        public IEnumerable<FilterData>? GetFilterDataByFilterKey(FilterKey key);
    }
}
