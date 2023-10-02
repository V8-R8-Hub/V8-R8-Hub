using Microsoft.AspNetCore.Mvc;

namespace V8_R8_Hub.Controllers {
    [Route("api/[Controller]")]
    [ApiController]
    public class GameController : ControllerBase {
        [HttpGet("benis", Name = "HelloWorld")]
        public IActionResult HelloWorld() {
            return Ok("Hello World!");
        }
    }
}
