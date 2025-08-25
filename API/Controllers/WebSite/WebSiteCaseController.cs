using Applications.Contracts.WebSiteServices;
using Applications.Dtos.WebSite;

namespace API.Controllers.WebSite
{
    [Route ( "api/[controller]" )]
    [ApiController]
    public class WebsiteCasesController ( IWebsiteCaseService websiteCaseService ) : ControllerBase
    {
        private readonly IWebsiteCaseService _websiteCaseService = websiteCaseService;

        
        [HttpGet ( "homepage" )]
        public async Task<ActionResult<IEnumerable<WebsiteCaseDto>>> GetForHomepage ( )
        {
            var cases = await _websiteCaseService.GetActiveForHomepageAsync();
            return Ok ( cases );
        }

        [HttpGet ( "{id}/details" )]
        public async Task<ActionResult<WebsiteCaseDetailsDto>> GetDetails ( int id )
        {
            var caseDetails = await _websiteCaseService.GetDetailsByIdAsync(id);
            return Ok ( caseDetails );
        }

        
        [HttpGet]
        [Authorize ( Roles = "admin" )]
        public async Task<ActionResult<IEnumerable<WebsiteCaseAdminDto>>> GetAll ( )
        {
            var cases = await _websiteCaseService.GetAllAsync();
            return Ok ( cases );
        }

        [HttpGet ( "active" )]
        [Authorize ( Roles = "admin" )]
        public async Task<ActionResult<IEnumerable<WebsiteCaseAdminDto>>> GetActive ( )
        {
            var cases = await _websiteCaseService.GetActiveAsync();
            return Ok ( cases );
        }

        [HttpGet ( "inactive" )]
        [Authorize ( Roles = "admin" )]
        public async Task<ActionResult<IEnumerable<WebsiteCaseAdminDto>>> GetInactive ( )
        {
            var cases = await _websiteCaseService.GetInactiveAsync();
            return Ok ( cases );
        }

        [HttpGet ( "{id}" )]
        [Authorize ( Roles = "admin" )]
        public async Task<ActionResult<WebsiteCaseDetailsDto>> GetById ( int id )
        {
            var caseDetails = await _websiteCaseService.GetByIdAsync(id);
            return Ok ( caseDetails );
        }

        [HttpPost]
        [Authorize ( Roles = "admin" )]
        public async Task<ActionResult<WebsiteCaseDetailsDto>> Create ( CreateWebsiteCaseDto dto )
        {
            var caseDetails = await _websiteCaseService.CreateAsync(dto);
            return CreatedAtAction ( nameof ( GetById ), new { id = caseDetails.Id }, caseDetails );
        }

        [HttpPut ( "{id}" )]
        [Authorize ( Roles = "admin" )]
        public async Task<ActionResult<WebsiteCaseDetailsDto>> Update ( int id, UpdateWebsiteCaseDto dto )
        {
            var caseDetails = await _websiteCaseService.UpdateAsync(id, dto);
            return Ok ( caseDetails );
        }

        [HttpPatch ( "{id}/activate" )]
        [Authorize ( Roles = "admin" )]
        public async Task<ActionResult> Activate ( int id )
        {
            await _websiteCaseService.ActivateCaseAsync ( id );
            return NoContent ( );
        }

        [HttpPatch ( "{id}/deactivate" )]
        [Authorize ( Roles = "admin" )]
        public async Task<ActionResult> Deactivate ( int id )
        {
            await _websiteCaseService.DeactivateCaseAsync ( id );
            return NoContent ( );
        }

        [HttpPatch ( "{id}/toggle-active" )]
        [Authorize ( Roles = "admin" )]
        public async Task<ActionResult> ToggleActive ( int id )
        {
            await _websiteCaseService.ToggleActiveAsync ( id );
            return NoContent ( );
        }

        [HttpPatch ( "reorder" )]
        [Authorize ( Roles = "admin" )]
        public async Task<ActionResult> UpdateOrder ( [FromBody] List<ReorderItemDto> reorderItems )
        {
            await _websiteCaseService.UpdateOrderAsync ( reorderItems );
            return NoContent ( );
        }

        [HttpDelete ( "{id}" )]
        [Authorize ( Roles = "admin" )]
        public async Task<ActionResult> DeletePermanently ( int id )
        {
            await _websiteCaseService.DeleteCasePermanentlyAsync ( id );
            return NoContent ( );
        }
    }
}