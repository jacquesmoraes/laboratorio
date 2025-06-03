using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route ("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : BaseApiController
    {

        [Route ("errors/{code}")]
        
        public IActionResult Error ( int code )
        {
            return new ObjectResult ( new ApiResponse ( code ) );
        }
    }
}
