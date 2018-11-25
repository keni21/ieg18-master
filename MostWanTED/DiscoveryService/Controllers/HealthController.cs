using Microsoft.AspNetCore.Mvc;

namespace DiscoveryService.Controllers
{
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        [HttpGet("status")]
        public IActionResult Status() => Ok();
    }
}
