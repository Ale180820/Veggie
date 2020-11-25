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

        [HttpPost ("createUser")]
        public ActionResult createUser([FromBody] User user) {
            if (user.nameUser != null) {
                if (createNewUser(user)) {
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
        }

        [HttpPost ("createConversation")]
        public ActionResult createConversation([FromBody] Conversation conversation) {
            try {
                conversation.userTwo = findUser(conversation.userTwo.username);
                if (createNewConversation(conversation)) {
                    return Ok();
                } else {
                    return StatusCode(500, "InternalServerError");
                }

            } catch {
                return StatusCode(500, "InternalServerError");
            }
        }

        public bool createNewUser(User user) {
            try {
                Models.MongoHelper.ConnectToMongoService();
                Models.MongoHelper.users_collection = Models.MongoHelper.database.GetCollection<VeggieBack.Models.User>("users");
                Models.MongoHelper.users_collection.InsertOneAsync(user);
                return true;
            }catch {
                return false;
            }
        }

        public bool createNewConversation(Conversation conversation) {
            try {
                Models.MongoHelper.ConnectToMongoService();
                Models.MongoHelper.conversations_collection = Models.MongoHelper.database.GetCollection<VeggieBack.Models.Conversation>("conversations");
                Models.MongoHelper.conversations_collection.InsertOneAsync(conversation);
                return true;
            }catch {
                return false;
            }
        }

        public User findUser(string username) {
            try {
                Models.MongoHelper.ConnectToMongoService();
                Models.MongoHelper.users_collection = Models.MongoHelper.database.GetCollection<VeggieBack.Models.User>("users");
                return Models.MongoHelper.users_collection.Find(Builders<VeggieBack.Models.User>.Filter.Eq("username", username)).FirstOrDefault(); ;
            } catch {
                return null;
            }
        }

        public bool loginUser(User user) {
            try {
                Models.MongoHelper.ConnectToMongoService();
                Models.MongoHelper.users_collection = Models.MongoHelper.database.GetCollection<VeggieBack.Models.User>("users");
                var result = Models.MongoHelper.users_collection.Find(Builders<VeggieBack.Models.User>.Filter.Eq("emailUser", user.emailUser)).FirstOrDefault();
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
