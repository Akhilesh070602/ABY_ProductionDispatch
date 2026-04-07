using Microsoft.AspNetCore.Mvc;
using PackingDisplay.Models;
using PackingDisplay.Services;

namespace PackingDisplay.Controllers
{
    [ApiController]
    [Route("api/dispatch")]
    public class DispatchApiController : ControllerBase
    {
        private readonly DispatchService _service;
        private readonly LogService _logService;

        public DispatchApiController(DispatchService service, LogService logService)
        {
            _service = service;
            _logService = logService;
        }

        [HttpGet("material-image")]
        public IActionResult GetMaterialImage(string material)
        {
            try
            {
                var image = _service.GetMaterialImage(material);

                _logService.LogSuccess("", "GetMaterialImage", "DispatchService", $"Material={material}");

                return Ok(new { image = image ?? "" });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, "", "GetMaterialImage", "DispatchService", $"Material={material}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("po")]
        public IActionResult GetPO(string code)
        {
            try
            {
                var data = _service.GetDispatchData(code);

                if (data == null)
                {
                    _logService.LogError(new Exception("No data found"), code, "GetPO", "DispatchService", $"PO={code}");
                    return NotFound();
                }

                _logService.LogSuccess(code, "GetPO", "DispatchService", $"PO={code}");

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, code, "GetPO", "DispatchService", $"PO={code}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("save")]
        public IActionResult Save([FromBody] DispatchSaveDto model)
        {
            try
            {
                _service.SaveActualWeight(model);

                _logService.LogSuccess(model.AUFNR, "SaveActualWeight", "DispatchService", $"PO={model.AUFNR}");

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logService.LogError(ex, model?.AUFNR, "SaveActualWeight", "DispatchService", $"PO={model?.AUFNR}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}