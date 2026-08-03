using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AdminController : ControllerBase
{
    [HttpGet("health")]
    public ActionResult GetHealth()
    {
        return Ok(new { Status = "Healthy" });
    }
}
