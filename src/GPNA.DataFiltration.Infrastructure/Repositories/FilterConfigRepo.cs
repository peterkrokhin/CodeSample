using System.Collections.Generic;
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

        public async Task<IEnumerable<FilterConfig>> GetAllIncludePool()
        {
            var filters = await _dbSet.Include(f => f.FilterPool).ToListAsync();
            return filters;
        }
    }
}
