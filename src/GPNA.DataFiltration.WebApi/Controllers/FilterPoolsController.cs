using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GPNA.DataFiltration.Application;
using Microsoft.AspNetCore.Mvc;

namespace GPNA.DataFiltration.WebApi
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("api/v1/[controller]")]
    public class FilterPoolsController : ControllerBase
    {
        private readonly IFilterPoolRepo _pools;
        private readonly IMapper _mapper;

        public FilterPoolsController(
            IFilterPoolRepo pools,
            IMapper mapper)
        {
            _pools = pools;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FilterPoolDto>>> GetAll()
        {
            var pools = await _pools.GetAllAsync();
            var poolsDto = _mapper.Map<IEnumerable<FilterPool>, IEnumerable<FilterPoolDto>>(pools);
            return Ok(poolsDto);
        }

        [HttpGet]
        [Route("{id:long}")]
        public async Task<ActionResult<FilterPoolDto>> GetById(long id)
        {
            var pool = await _pools.GetByIdAsync(id);
            var poolDto = _mapper.Map<FilterPool, FilterPoolDto>(pool);
            return Ok(poolDto);
        }

        [HttpPost]
        public async Task<IActionResult> Add(FilterPoolAddModel model)
        {
            var pool = await _pools.GetByIdAsync(model.Id);
            if (pool != null)
            {
                return BadRequest();
            }

            var newPool = _mapper.Map<FilterPoolAddModel, FilterPool>(model);
            await _pools.AddAsync(newPool);

            return CreatedAtAction(
                nameof(GetById),
                new { newPool.Id },
                newPool);
        }

        [HttpDelete]
        [Route("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var pool = await _pools.GetByIdAsync(id);
            if (pool == null)
            {
                return BadRequest();
            }

            await _pools.DeleteAsync(pool);

            return NoContent();
        }
    }
}
