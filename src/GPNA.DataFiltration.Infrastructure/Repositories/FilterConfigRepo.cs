using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GPNA.DataFiltration.Application;
using Microsoft.EntityFrameworkCore;

namespace GPNA.DataFiltration.Infrastructure
{
    class FilterConfigRepo : GenericRepo<FilterConfig>, IFilterConfigRepo
    {
        public FilterConfigRepo(IAppDbContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<FilterConfig> GetAllIncludePool()
        {
            var filters = _dbSet.Include(f => f.FilterPool).ToList();
            return filters;
        }

        public IEnumerable<FilterConfig> GetBySourceTopicAndWellIdAndParameterId(
            string sourceTopic, long wellId, long parameterId)
        {
            var filters = _dbSet
                .Include(f => f.FilterPool)
                .Where(f => f.FilterPool.SourceTopic == sourceTopic &
                            f.WellId == wellId &
                            f.ParameterId == parameterId)
                .ToList();

            return filters;
        }
    }
}
