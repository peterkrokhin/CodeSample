using AutoMapper;
using GPNA.DataFiltration.Application;

namespace GPNA.DataFiltration.WebApi
{
    public class DtoProfile : Profile
    {
        public DtoProfile()
        {
            CreateMap<FilterPool, FilterPoolDto>();
            CreateMap<FilterConfig, FilterConfigDto>();
            CreateMap<FilterPoolAddModel, FilterPool>();
        }
    }
}
