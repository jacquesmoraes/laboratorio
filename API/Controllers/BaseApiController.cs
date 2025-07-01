namespace API.Controllers
{

    //[Authorize ( Roles = "admin" )]
    [Route ( "api/[controller]" )]
    [ApiController]
    public class BaseApiController : ControllerBase
    {

    }
}
