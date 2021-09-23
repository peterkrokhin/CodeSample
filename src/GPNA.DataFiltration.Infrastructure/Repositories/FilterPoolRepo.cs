using GPNA.DataFiltration.Application;

namespace GPNA.DataFiltration.Infrastructure
{
    class FilterPoolRepo : GenericRepo<FilterPool>, IFilterPoolRepo
    {
        public FilterPoolRepo(IAppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
