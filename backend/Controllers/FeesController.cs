using Microsoft.AspNetCore.Mvc;
using Backend.Services;
using Backend.Validators;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/v1/fees")]
    public class FeesController : ControllerBase
    {
        private readonly IFeesService _svc;

        public FeesController(IFeesService svc)
        {
            _svc = svc;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Get([FromQuery] FeesQueryRequest request)
        {
            // Controller is intentionally thin: validation is handled by FluentValidation
            var res = _svc.CalculateFees(request);
            return Ok(res);
        }
    }
}
