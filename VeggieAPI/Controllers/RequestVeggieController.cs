using Microsoft.AspNetCore.Mvc;
using VeggieBack.Models;

namespace VeggieAPI.Controllers {

    [Route("api/")]
    [ApiController]
    public class RequestVeggieController : ControllerBase {

        [HttpGet("home")]
        public ActionResult Get() {
            return Ok();
        }

        [HttpPost ("createUser")]
        public ActionResult createUser(User newUser) {
            if (newUser != null) {
                return Ok();
            }else {
                return StatusCode(500, "InternalServerError");
            }
        }


    }
}
