using Applications.Interfaces;
using Core.FactorySpecifications;
using Core.Models.Pricing;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Pricing
{
    [Route ( "api/[controller]" )]
    [ApiController]
    public class TablePriceController ( IGenericService<TablePrice> tablePriceService ) : BaseApiController
    {
        private readonly IGenericService<TablePrice> _tablePriceService = tablePriceService;

        [HttpGet]
        public async Task<IActionResult> GetAll ( )
        {
            var spec = new BaseSpecification<TablePrice>();
            var result = await _tablePriceService.GetAllWithSpecAsync(spec);
            return Ok ( result );
        }

        [HttpGet ( "{id}" )]
        public async Task<IActionResult> GetById ( int id )
        {
            var spec = TablePriceSpecs.ByIdWithRelations(id);
            var result = await _tablePriceService.GetEntityWithSpecAsync(spec);
            if ( result == null ) return NotFound ( );
            return Ok ( result );
        }

        [HttpPost]
        public async Task<IActionResult> Create ( [FromBody] TablePrice tablePrice )
        {
            var created = await _tablePriceService.CreateAsync(tablePrice);
            return CreatedAtAction ( nameof ( GetById ), new { id = created.Id }, created );
        }

        [HttpPut ( "{id}" )]
        public async Task<IActionResult> Update ( int id, [FromBody] TablePrice updated )
        {
            var result = await _tablePriceService.UpdateAsync(id, updated);
            if ( result == null ) return NotFound ( );
            return Ok ( result );
        }

        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> Delete ( int id )
        {
            var result = await _tablePriceService.DeleteAsync(id);
            if ( result == null ) return NotFound ( );
            return NoContent ( );
        }
    }

}
