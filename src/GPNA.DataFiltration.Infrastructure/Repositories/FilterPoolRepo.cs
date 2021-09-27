using System.Collections.Generic;
using System.Threading.Tasks;
using GPNA.DataFiltration.Application;
using Microsoft.EntityFrameworkCore;

namespace GPNA.DataFiltration.Infrastructure
{
    class FilterPoolRepo : GenericRepo<FilterPool>, IFilterPoolRepo
    {
        public FilterPoolRepo(IAppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<FilterPool>> GetAllIncludeFilterConfigs()
        {
            var pools = await _dbSet.Include(p => p.FilterConfigs).ToListAsync();
            return pools;
        }
    }
}
