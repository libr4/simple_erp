using Microsoft.AspNetCore.Mvc;
using Backend.Services;
using Backend.DTOs;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/v1/comissao")]
    public class CommissionController : ControllerBase
    {
        private readonly ICommissionService _svc;

        public CommissionController(ICommissionService svc)
        {
            _svc = svc;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] CommissionRequest req)
        {
            var res = _svc.CalculateCommission(req);
            return Ok(res);
        }
    }
}
