using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GPNA.DataFiltration.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GPNA.DataFiltration.WebApi
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("api/v1/[controller]")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public class FilterPoolsController : ControllerBase
    {
        private readonly IFiltrationService _filtrationService;
        private readonly IFilterPoolRepo _pools;
        private readonly IMapper _mapper;

        public FilterPoolsController(
            IFiltrationService filtrationService,
            IFilterPoolRepo pools,
            IMapper mapper)
        {
            _filtrationService = filtrationService;
            _pools = pools;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FilterPoolDto>))]
        public async Task<IActionResult> GetAll()
        {
            _filtrationService.Stop();

            var pools = await _pools.GetAllAsync();
            var poolsDto = _mapper.Map<IEnumerable<FilterPool>, IEnumerable<FilterPoolDto>>(pools);

            _filtrationService.Start();

            return Ok(poolsDto);
        }

        [HttpGet]
        [Route("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FilterPoolDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(long id)
        {
            _filtrationService.Stop();

            var pool = await _pools.GetByIdAsync(id);
            if (pool is null)
            {
                return NotFound();
            }
            var poolDto = _mapper.Map<FilterPool, FilterPoolDto>(pool);

            _filtrationService.Start();

            return Ok(poolDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Add(FilterPoolAddModel model)
        {
            _filtrationService.Stop();

            var newPool = _mapper.Map<FilterPoolAddModel, FilterPool>(model);
            await _pools.AddAsync(newPool);

            _filtrationService.Start();

            return CreatedAtAction(
                nameof(GetById),
                new { newPool.Id },
                newPool);
        }

        [HttpDelete]
        [Route("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            _filtrationService.Stop();

            var pool = await _pools.GetByIdAsync(id);
            if (pool == null)
            {
                return NotFound();
            }

            await _pools.DeleteAsync(pool);

            _filtrationService.Start();

            return NoContent();
        }
    }
}
