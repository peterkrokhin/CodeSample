using GPNA.DataFiltration.Application;

namespace GPNA.DataFiltration.Infrastructure
{
    class FilterConfigRepo : GenericRepo<FilterPool>, IFilterConfigRepo
    {
        public FilterConfigRepo(IAppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
