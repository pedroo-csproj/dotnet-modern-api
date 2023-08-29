using Microsoft.AspNetCore.Mvc;

namespace DotNETModernAPI.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    [HttpGet]
    public IActionResult TestEndpoint() =>
        NoContent();
}
