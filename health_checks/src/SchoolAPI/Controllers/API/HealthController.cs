using Microsoft.AspNetCore.Mvc;

namespace SchoolAPI.Controllers.API
{
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        [HttpGet("status")]        
        public IActionResult Status() => Ok();
    }
}
