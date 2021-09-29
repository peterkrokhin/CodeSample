using System.Collections.Generic;

namespace GPNA.DataFiltration.Application
{
    public interface IFilterConfigRepo: IGenericRepo<FilterConfig>
    {
        IEnumerable<FilterConfig> GetAllIncludePool();
        IEnumerable<FilterConfig> GetBySourceTopicAndWellIdAndParameterId(string sourceTopic, long wellId, long parameterId);
    }
}
