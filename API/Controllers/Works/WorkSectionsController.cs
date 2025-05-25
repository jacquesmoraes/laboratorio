using Applications.Contracts;
using Core.FactorySpecifications;
using Core.Models.Works;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Works
{
    [Route ( "api/[controller]" )]
    [ApiController]
    public class WorkSectionsController ( IGenericService<WorkSection> sectionService ) :
        BaseApiController
    {
        private readonly IGenericService<WorkSection> _sectionService = sectionService;

        [HttpGet]
        public async Task<IActionResult> GetAll ( )
        {
            var spec = new BaseSpecification<WorkSection>();
            var sections = await _sectionService.GetAllWithSpecAsync(spec);
            return Ok ( sections );
        }

        [HttpGet ( "{id}" )]
        public async Task<IActionResult> GetById ( int id )
        {
            var spec = new WorkSectionSpecification(id);
            var section = await _sectionService.GetEntityWithSpecAsync(spec);
            if ( section == null ) return NotFound ( );
            return Ok ( section );
        }

        [HttpPost]
        public async Task<IActionResult> Create ( [FromBody] WorkSection section )
        {
            var created = await _sectionService.CreateAsync(section);
            return CreatedAtAction ( nameof ( GetById ), new { id = created.Id }, created );
        }

        [HttpPut ( "{id}" )]
        public async Task<IActionResult> Update ( int id, [FromBody] WorkSection updated )
        {
            var result = await _sectionService.UpdateAsync(id, updated);
            if ( result == null ) return NotFound ( );
            return Ok ( result );
        }

        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> Delete ( int id )
        {
            var deleted = await _sectionService.DeleteAsync(id);
            if ( deleted == null ) return NotFound ( );
            return NoContent ( );
        }

    }
}
