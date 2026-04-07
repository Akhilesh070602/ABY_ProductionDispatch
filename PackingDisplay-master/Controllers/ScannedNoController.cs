using Microsoft.AspNetCore.Mvc;
using PackingDisplay.Services;

namespace PackingDisplay.Controllers
{
    [ApiController]
    [Route("api/ScannedNoController")]
    public class ScannedNoController : ControllerBase
    {
        private readonly ScannedNoService _service;

        public ScannedNoController(ScannedNoService service)
        {
            _service = service;
        }

        // ✅ API: Increment Scan
        [HttpPost("increment-scan")]
        public IActionResult IncrementScan()
        {
            var result = _service.IncrementScan();

            if (result)
                return Ok(new { message = "Scan count updated" });

            return StatusCode(500, "Failed to update scan count");
        }

        // ✅ API: Get Scan Count
        [HttpGet("get-scan-count")]
        public IActionResult GetScanCount()
        {
            int count = _service.GetScanCount();
            return Ok(count);
        }


        [HttpPost("increment-confirm")]
        public IActionResult IncrementConfirm()
        {
            var result = _service.IncrementConfirm();

            if (result)
                return Ok();

            return StatusCode(500, "Failed");
        }


        [HttpGet("get-confirm-count")]
        public IActionResult GetConfirmCount()
        {
            int count = _service.GetConfirmCount();
            return Ok(count);
        }
    }
}