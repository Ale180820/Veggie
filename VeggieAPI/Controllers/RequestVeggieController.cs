using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
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
        public ActionResult createUser([FromBody] User user) {
            if (user.nameUser != null) {
                if (create(user)) {
                    return Ok();
                } else {
                    return StatusCode(500, "InternalServerError");
                }
            }else {
                return StatusCode(500, "InternalServerError");
            }
        }

        public bool create(User user) {
            try {
                Models.MongoHelper.ConnectToMongoService();
                Models.MongoHelper.users_collection = Models.MongoHelper.database.GetCollection<VeggieBack.Models.User>("user");
                Models.MongoHelper.users_collection.InsertOneAsync(user);
                return true;
            }catch {
                return false;
            }
        }


    }
}
