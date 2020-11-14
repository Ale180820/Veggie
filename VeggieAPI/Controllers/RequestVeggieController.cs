using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
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

        [HttpPost ("create")]
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

        [HttpPost ("login")]
        public ActionResult login([FromBody] User user) {
            if (user.emailUser != null) {
                if (loginUser(user)) {
                    return Ok();
                }else {
                    return StatusCode(500);
                }
            }else {
                return StatusCode(500, "InternalServerError");
            }

        } //12545698

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

        public bool loginUser(User user) {
            try {
                Models.MongoHelper.ConnectToMongoService();
                Models.MongoHelper.users_collection = Models.MongoHelper.database.GetCollection<VeggieBack.Models.User>("user");
                var filter = Builders<VeggieBack.Models.User>.Filter.Eq("emailUser", user.emailUser);
                var result = Models.MongoHelper.users_collection.Find(filter).FirstOrDefault();
                if (user.password.Equals(result.password)) {
                    return true;
                }else {
                    return false;
                }
            }catch {
                return false;
            }
        }
    }
}
