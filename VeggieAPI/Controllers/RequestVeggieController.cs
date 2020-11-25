using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using VeggieBack.Models;

namespace VeggieAPI.Controllers {

    [Route("api/")]
    [ApiController]
    public class RequestVeggieController : ControllerBase {

        [HttpGet("home")]
        public ActionResult Get() {
            return Ok();
        }

        [HttpPost ("findUserByUsername")]
        public ActionResult findUser([FromBody] string username){
            User newUser = new User();
            newUser.username = findUserDataBaseByUsername(username).username;
            if(newUser != null) {
                newUser.password = null;
                return Ok(newUser);
            }else {
                return StatusCode(500, "InternalServerError");
            }
        } 

        [HttpPost("findUserByEmail")]
        public ActionResult findUserByEmail([FromBody] string email) {
            User newUser = new User();
            newUser.username = findUserDataBaseByEmail(email).username;
            if (newUser != null){
                newUser.password = null;
                return Ok(newUser);
            } else {
                return StatusCode(500, "InternalServerError");
            }
        }

        [HttpPost("createUser")]
        public ActionResult createUser([FromBody] User user) {
            if (user.nameUser != null) {
                if (createNewUser(user)) {
                    return Ok();
                } else{
                    return StatusCode(500, "InternalServerError");
                }
            } else{
                return StatusCode(500, "InternalServerError");
            }
        }

        [HttpPost("login")]
        public ActionResult login([FromBody] User user) {
            if (user.emailUser != null) {
                if (loginUser(user)) {
                    return Ok();
                } else {
                    return StatusCode(500);
                }
            } else {
                return StatusCode(500, "InternalServerError");
            }
        }

        [HttpPost("createConversation")]
        public ActionResult createConversation([FromBody] Conversation conversation) {
            try {
                conversation.userTwo = findUserDataBaseByUsername(conversation.userTwo.username);
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
                Models.MongoHelper.users_collection = Models.MongoHelper.database.GetCollection<VeggieBack.Models.User>("user");
                Models.MongoHelper.users_collection.InsertOneAsync(user);
                return true;
            } catch {
                return false;
            }
        }

        public bool createNewConversation(Conversation conversation) {
            try {
                Models.MongoHelper.ConnectToMongoService();
                Models.MongoHelper.conversations_collection = Models.MongoHelper.database.GetCollection<VeggieBack.Models.Conversation>("conversation");
                Models.MongoHelper.conversations_collection.InsertOneAsync(conversation);
                return true;
            } catch {
                return false;
            }
        }

        public User findUserDataBaseByUsername(string username) {
            try {
                Models.MongoHelper.ConnectToMongoService();
                Models.MongoHelper.users_collection = Models.MongoHelper.database.GetCollection<VeggieBack.Models.User>("user");
                return Models.MongoHelper.users_collection.Find(Builders<VeggieBack.Models.User>.Filter.Eq("username", username)).FirstOrDefault();
            } catch {
                return null;
            }
        }

        public User findUserDataBaseByEmail(string email) {
            try {
                Models.MongoHelper.ConnectToMongoService();
                Models.MongoHelper.users_collection = Models.MongoHelper.database.GetCollection<VeggieBack.Models.User>("user");
                return Models.MongoHelper.users_collection.Find(Builders<VeggieBack.Models.User>.Filter.Eq("emailUser", email)).FirstOrDefault(); ;
            } catch {
                return null;
            }
        }

        public bool loginUser(User user) {
            try {
                Models.MongoHelper.ConnectToMongoService();
                Models.MongoHelper.users_collection = Models.MongoHelper.database.GetCollection<VeggieBack.Models.User>("user");
                var filter = Builders<VeggieBack.Models.User>.Filter.Eq("emailUser", user.emailUser);
                var result = Models.MongoHelper.users_collection.Find(filter).FirstOrDefault();
                if (result == null) {
                    return false;
                }
                if (user.password.Equals(result.password)) {
                    return true;
                } else {
                    return false;
                }
            } catch {
                return false;
            }
        }
           
     }
}

