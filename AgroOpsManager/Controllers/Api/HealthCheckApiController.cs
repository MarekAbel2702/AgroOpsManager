using Microsoft.AspNetCore.Mvc;

namespace AgroOpsManager.Web.Controllers.Api
{

    [ApiController]
    [Route("api/health-check")]
    public class HealthCheckApiController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Status = "Healthy",
                Application = "AgroOps Manager",
                TimestampUtc = DateTime.UtcNow
            });
        }
    }
}
