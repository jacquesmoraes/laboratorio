using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route ("errors")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : BaseApiController
    {

        [Route ("{code}")]
        
        public IActionResult Error ( int code )
        {
            return new ObjectResult ( new ApiResponse ( code ) );
        }
    }
}
