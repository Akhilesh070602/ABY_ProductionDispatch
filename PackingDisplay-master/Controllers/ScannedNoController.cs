//using Microsoft.AspNetCore.Mvc;
//using PackingDisplay.Services;

//namespace PackingDisplay.Controllers
//{
//    [ApiController]
//    [Route("api/ScannedNoController")]
//    public class ScannedNoController : ControllerBase
//    {
//        private readonly ScannedNoService _service;

//        public ScannedNoController(ScannedNoService service)
//        {
//            _service = service;
//        }

//        // ✅ API: Increment Scan
//        [HttpPost("increment-scan")]
//        public IActionResult IncrementScan()
//        {
//            var result = _service.IncrementScan();

//            if (result)
//                return Ok(new { message = "Scan count updated" });

//            return StatusCode(500, "Failed to update scan count");
//        }

//        // ✅ API: Get Scan Count
//        [HttpGet("get-scan-count")]
//        public IActionResult GetScanCount()
//        {
//            int count = _service.GetScanCount();
//            return Ok(count);
//        }


//        [HttpPost("increment-confirm")]
//        public IActionResult IncrementConfirm()
//        {
//            var result = _service.IncrementConfirm();

//            if (result)
//                return Ok();

//            return StatusCode(500, "Failed");
//        }


//        [HttpGet("get-confirm-count")]
//        public IActionResult GetConfirmCount()
//        {
//            int count = _service.GetConfirmCount();
//            return Ok(count);
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using PackingDisplay.Services;

namespace PackingDisplay.Controllers
{
    [ApiController]
    [Route("api/ScannedNoController")]
    public class ScannedNoController : ControllerBase
    {
        private readonly ScannedNoService _service;
        private readonly LogService _logService;

        public ScannedNoController(ScannedNoService service, LogService logService)
        {
            _service = service;
            _logService = logService;
        }

        // ✅ Increment Scan
        [HttpPost("increment-scan")]
        public IActionResult IncrementScan()
        {
            try
            {
                _logService.LogInfo("API HIT - IncrementScan", "", "IncrementScan", "ScannedNoController");

                var result = _service.IncrementScan();

                if (!result)
                    return StatusCode(500, "Failed to update scan count");

                return Ok(new { message = "Scan count updated" });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, "", "IncrementScan", "ScannedNoController");
                return StatusCode(500, "Error occurred");
            }
        }

        // ✅ Get Scan Count
        [HttpGet("get-scan-count")]
        public IActionResult GetScanCount()
        {
            try
            {
                _logService.LogInfo("API HIT - GetScanCount", "", "GetScanCount", "ScannedNoController");

                int count = _service.GetScanCount();

                return Ok(count);
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, "", "GetScanCount", "ScannedNoController");
                return StatusCode(500, "Error occurred");
            }
        }

        // ✅ Increment Confirm
        [HttpPost("increment-confirm")]
        public IActionResult IncrementConfirm()
        {
            try
            {
                _logService.LogInfo("API HIT - IncrementConfirm", "", "IncrementConfirm", "ScannedNoController");

                var result = _service.IncrementConfirm();

                if (!result)
                    return StatusCode(500, "Failed");

                return Ok();
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, "", "IncrementConfirm", "ScannedNoController");
                return StatusCode(500, "Error occurred");
            }
        }

        // ✅ Get Confirm Count
        [HttpGet("get-confirm-count")]
        public IActionResult GetConfirmCount()
        {
            try
            {
                _logService.LogInfo("API HIT - GetConfirmCount", "", "GetConfirmCount", "ScannedNoController");

                int count = _service.GetConfirmCount();

                return Ok(count);
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, "", "GetConfirmCount", "ScannedNoController");
                return StatusCode(500, "Error occurred");
            }
        }
    }
}