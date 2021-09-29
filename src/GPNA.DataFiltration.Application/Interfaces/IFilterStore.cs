using System.Collections.Generic;

namespace GPNA.DataFiltration.Application
{
    public interface IFilterStore
    {
        void CacheUpdate();
        public IEnumerable<FilterData>? GetFilterDataByFilterKey(FilterKey key);
        void ModifyFilterDataByFilterKey(FilterKey key, FilterData newFilterData);
        IEnumerable<string> GetSourceTopics();
        IEnumerable<string> GetGoodTopics();
        IEnumerable<string> GetBadTopics();

    }
}
