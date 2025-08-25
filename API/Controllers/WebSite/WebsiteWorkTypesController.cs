using Applications.Contracts.WebSiteServices;
using Applications.Dtos.WebSite;

namespace API.Controllers.WebSite
{
    [Route ( "api/[controller]" )]
    [ApiController]
    public class WebsiteWorkTypesController ( IWebsiteWorkTypeService websiteWorkTypeService ) : ControllerBase
    {
        private readonly IWebsiteWorkTypeService _websiteWorkTypeService = websiteWorkTypeService;

        
        [HttpGet ( "active" )]
        public async Task<ActionResult<IEnumerable<WebsiteWorkTypeDto>>> GetActive ( )
        {
            var workTypes = await _websiteWorkTypeService.GetActiveForWebsiteAsync();
            return Ok ( workTypes );
        }

        
        [HttpGet]
        [Authorize ( Roles = "admin" )]
        public async Task<ActionResult<IEnumerable<WebsiteWorkTypeDto>>> GetAll ( )
        {
            var workTypes = await _websiteWorkTypeService.GetAllAsync();
            return Ok ( workTypes );
        }

        [HttpGet ( "{id}" )]
        [Authorize ( Roles = "admin" )]
        public async Task<ActionResult<WebsiteWorkTypeDto>> GetById ( int id )
        {
            var workType = await _websiteWorkTypeService.GetByIdAsync(id);
            return Ok ( workType );
        }

        [HttpPost]
        [Authorize ( Roles = "admin" )]
        public async Task<ActionResult<WebsiteWorkTypeDto>> Create ( CreateWebsiteWorkTypeDto dto )
        {
            var workType = await _websiteWorkTypeService.CreateAsync(dto);
            return CreatedAtAction ( nameof ( GetById ), new { id = workType.Id }, workType );
        }

        [HttpPut ( "{id}" )]
        [Authorize ( Roles = "admin" )]
        public async Task<ActionResult<WebsiteWorkTypeDto>> Update ( int id, UpdateWebsiteWorkTypeDto dto )
        {
            var workType = await _websiteWorkTypeService.UpdateAsync(id, dto);
            return Ok ( workType );
        }

        [HttpPatch ( "{id}/toggle-active" )]
        [Authorize ( Roles = "admin" )]
        public async Task<ActionResult> ToggleActive ( int id )
        {
            await _websiteWorkTypeService.ToggleActiveAsync ( id );
            return NoContent ( );
        }

        [HttpPatch ( "reorder" )]
        [Authorize ( Roles = "admin" )]
        public async Task<ActionResult> UpdateOrder ( [FromBody] List<ReorderItemDto> reorderItems )
        {
            await _websiteWorkTypeService.UpdateOrderAsync ( reorderItems );
            return NoContent ( );
        }

        [HttpDelete ( "{id}" )]
        [Authorize ( Roles = "admin" )]
        public async Task<ActionResult> DeletePermanently ( int id )
        {
            await _websiteWorkTypeService.DeletePermanentlyAsync ( id );
            return NoContent ( );
        }
    }
}