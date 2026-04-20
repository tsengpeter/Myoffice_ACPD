using Microsoft.AspNetCore.Mvc;
using myofficeacpd.Models;
using myofficeacpd.Interfaces;

namespace myofficeacpd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyofficeacpdController : ControllerBase
    {
        private readonly IMyofficeacpdService _service;

        public MyofficeacpdController(IMyofficeacpdService service)
        {
            _service = service;
        }

        // GET api/myofficeacpd?status=1&stop=false&keyword=張
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetAcpdResultModel>>> GetAll([FromQuery] GetAcpdQueryModel query)
        {
            var result = await _service.GetAllAsync(query);
            return Ok(result);
        }

        // GET api/myofficeacpd/{sid}
        [HttpGet("{sid}")]
        public async Task<ActionResult<GetAcpdResultModel>> GetBySid(string sid)
        {
            var result = await _service.GetBySidAsync(sid);

            if (result is null)
                return NotFound();

            return Ok(result);
        }
    }
}
