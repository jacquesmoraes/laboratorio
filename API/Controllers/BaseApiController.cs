using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [Authorize( Roles = "admin" )]
    [Route ( "api/[controller]" )]
    [ApiController]
    public class BaseApiController : ControllerBase
    {

    }
}
