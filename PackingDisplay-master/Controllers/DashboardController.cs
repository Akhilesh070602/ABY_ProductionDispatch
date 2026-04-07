using Microsoft.AspNetCore.Mvc;
using PackingDisplay.Services;

namespace PackingDisplay.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _service;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(DashboardService service, ILogger<DashboardController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data = await _service.GetDashboard();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Dashboard API");
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }
    }
}