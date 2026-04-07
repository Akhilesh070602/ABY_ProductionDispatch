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

        public DispatchApiController(DispatchService service)
        {
            _service = service;
        }
        [HttpGet("material-image")]
        public IActionResult GetMaterialImage(string material)
        {
            var image = _service.GetMaterialImage(material);

            return Ok(new
            {
                image = image ?? ""

            });
        }
        // 🔥 GET FROM SAP
        [HttpGet("po")]
        public IActionResult GetPO(string code)
        {
            try
            {
                Console.WriteLine("API HIT: " + code);

                var data = _service.GetDispatchData(code);

                if (data == null)
                    return NotFound();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        // for get image 

        // 🔥 SAVE (optional DB)
        [HttpPost("save")]
        public IActionResult Save([FromBody] DispatchSaveDto model)
        {
            try
            {
                _service.SaveActualWeight(model);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}