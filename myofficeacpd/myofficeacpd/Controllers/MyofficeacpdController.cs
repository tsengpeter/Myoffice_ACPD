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
        // POST api/myofficeacpd
        [HttpPost]
        public async Task<ActionResult<PostAcpdResultModel>> Create([FromBody] PostAcpdRequestModel request)
        {
            try
            {
                var result = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(GetBySid), new { sid = result.Sid }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        // PUT api/myofficeacpd/{sid}
        [HttpPut("{sid}")]
        public async Task<ActionResult<PutAcpdResultModel>> Update(string sid, [FromBody] PutAcpdRequestModel request)
        {
            try
            {
                var result = await _service.UpdateAsync(sid, request);

                if (result is null)
                    return NotFound();

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        // DELETE api/myofficeacpd/{sid}
        [HttpDelete("{sid}")]
        public async Task<IActionResult> Delete(string sid)
        {
            var success = await _service.DeleteAsync(sid);

            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
