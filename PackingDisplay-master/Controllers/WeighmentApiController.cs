//using Microsoft.AspNetCore.Mvc;
//using PackingDisplay.Services;

//namespace PackingDisplay.Controllers
//{
//    [ApiController]
//    [Route("api/weighment")]
//    public class WeighmentApiController : ControllerBase
//    {
//        [HttpGet("weight")]
//        public IActionResult GetWeight()
//        {
//            try
//            {
//                var service = new WeighmentService();
//                var weight = service.ReadWeight();

//                return Ok(weight);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }
//    }
//}


using Microsoft.AspNetCore.Mvc;
using PackingDisplay.Services;

namespace PackingDisplay.Controllers
{
    [ApiController]
    [Route("api/weighment")]
    public class WeighmentApiController : ControllerBase
    {
        private readonly WeighmentService _service;
        private readonly LogService _logService;

        public WeighmentApiController(WeighmentService service, LogService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpGet("weight")]
        public IActionResult GetWeight()
        {
            try
            {
                // ✅ API HIT
                _logService.LogInfo("API HIT - GetWeight", "", "GetWeight", "WeighmentController");

                var result = _service.ReadWeight();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logService.LogError(
                    ex,
                    "",
                    "GetWeight",
                    "WeighmentController",
                    "Weight fetch request"
                );

                return StatusCode(500, "ERROR: " + ex.Message);
            }
        }
    }
}