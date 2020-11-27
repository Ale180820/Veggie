using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using VeggieAPI.Services;
using VeggieBack.Controllers;
using VeggieBack.Models;

namespace VeggieAPI.Controllers {

    [Route("api/")]
    [ApiController]
    public class RequestVeggieController : ControllerBase {

        [HttpGet("home")]
        public ActionResult Get() {
            return Ok();
        }

        #region Methods for user processes
        [HttpPost("createUser")]
        public ActionResult createUser([FromBody] User user) {
            if (user.nameUser != null) {
                if (createNewUser(user)) {
                    return Ok();
                } else {
                    return StatusCode(500, "InternalServerError");
                }
            } else {
                return StatusCode(500, "InternalServerError");
            }
        }

        [HttpPost("findUserByUsername")]
        public ActionResult findUser([FromBody] string username) {
            User newUser = new User();
            newUser.username = findUserDataBaseByUsername(username).username;
            if (newUser != null) {
                newUser.password = null;
                return Ok(newUser);
            } else {
                return StatusCode(500, "InternalServerError");
            }
        }

        [HttpPost("findUserByUsernameExist")]
        public ActionResult findUserExist([FromBody] string username) {
            User newUser = findUserDataBaseByUsername(username);
            if (newUser == null) {
                return StatusCode(500, "InternalServerError");
            }
            newUser.nameUser = null;
            newUser.password = null;
            newUser.lastNameUser = null;
            if (newUser != null) {
                return Ok(newUser);
            } else {
                return StatusCode(500, "InternalServerError");
            }
        }

        [HttpPost("findUserByEmail")]
        public ActionResult findUserByEmail([FromBody] User email) {
            User newUser = new User();
            if(findUserDataBaseByEmail(email.emailUser) != null){
                newUser._id = findUserDataBaseByEmail(email.emailUser)._id;
                return Ok(newUser);
            }else {
                return StatusCode(500, "InternalServerError");
            }
            
            
        }

        #region [User] Methods for communication with the database
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

        public User findUserById(int idUser) {
            try {
                Models.MongoHelper.ConnectToMongoService();
                Models.MongoHelper.users_collection = Models.MongoHelper.database.GetCollection<VeggieBack.Models.User>("user");
                return Models.MongoHelper.users_collection.Find(Builders<VeggieBack.Models.User>.Filter.Eq("_id", idUser)).FirstOrDefault(); ;
            } catch {
                return null;
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
                var filter = Builders<VeggieBack.Models.User>.Filter.Eq("emailUser", email);
                var result = Models.MongoHelper.users_collection.Find(filter).FirstOrDefault();
                return result;
            } catch {
                return null;
            }
        }
        #endregion


        #endregion

        #region Methods for login processes
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

        #region Internal process
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
        #endregion
        #endregion

        [HttpPost("createConversation")]
        public ActionResult createConversation([FromBody] Entry userConversation) {
            try {
                Conversation newConversation = new Conversation();
                DiffieHellman df = new DiffieHellman();
                df.GenerateKeys(104);
                newConversation.userOne = findUserById(int.Parse(userConversation.actualUser));
                newConversation.userTwo = findUserDataBaseByUsername(userConversation.sendUser);
                newConversation.firstKey = df.getPublicOne();
                newConversation.secondKey = df.getPublicTwo();
                newConversation.messages = new List<Message>();
                if (createNewConversation(newConversation)) {
                    return Ok();
                } else {
                    return StatusCode(500, "InternalServerError");
                }
            } catch {
                return StatusCode(500, "InternalServerError");
            }
        }

        [HttpPost("getConversationByUserId")]
        public ActionResult getConversationByUserId([FromBody] int idUser){
            List<Conversation> conversationsReturn = new List<Conversation>();
            List<Conversation> firstResult = getConversationByUser(idUser, true);
            List<Conversation> secondResult = getConversationByUser(idUser, false);
            if (secondResult != null && firstResult != null) {
                foreach (Conversation item in firstResult){
                    conversationsReturn.Add(item);
                }
                foreach (Conversation item in secondResult){
                    conversationsReturn.Add(item);
                }
                return Ok(conversationsReturn);
            }else {
                return Ok(conversationsReturn);
            }
        }

        [HttpPost("sendMessage")]
        public ActionResult sendMessage([FromBody] Message message) {
            try {
                if (sendMessageInConversation(message)) {
                    return Ok(decryptionMessages(findConversationById(Storage.Instance.actualConversation._id)));
                } else {
                    return StatusCode(500, "InternalServerError");
                }
            } catch {
                return StatusCode(500, "InternalServerError");
            }
        }

        

        [HttpPost("getMessagesConversation")]
        public ActionResult getConversation([FromBody] Entry userConversation){
            try{
                var firstUser = findUserById(int.Parse(userConversation.actualUser)).username;
                var secondUser = findUserDataBaseByUsername(userConversation.sendUser).username;
                List<Message> messages = getConversationMessages(firstUser, secondUser);
                if (messages.Count > 0){
                    return Ok(messages);
                }else{
                    return StatusCode(500, "InternalServerError");
                }
            }catch {
                return StatusCode(500, "InternalServerError");
            }
        }  

        [HttpPost("findConversationByUsers")]
        public ActionResult findConversation([FromBody] Entry userConversation) {
            try {
                var firstUser = findUserById(int.Parse(userConversation.actualUser)).username;
                var secondUser = findUserDataBaseByUsername(userConversation.sendUser).username;
                if (findConversation(firstUser, secondUser)){
                    return Ok(false);
                }else if (findConversation(secondUser, firstUser)){
                    return Ok(false);
                }else {
                    return Ok(true);
                }
            }catch {
                return StatusCode(500, "InternalServerError");
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
        
        public bool sendMessageInConversation(Message message) {
            try {
                SDES encryption = new SDES(); //Objeto que ecriptará el mensaje.
                DiffieHellman df = new DiffieHellman();
                message.message = encryption.CifradoDecifrado(message.message, true, df.getPrivateKey(Storage.Instance.actualConversation.firstKey, Storage.Instance.actualConversation.secondKey));
                Models.MongoHelper.ConnectToMongoService();
                Models.MongoHelper.conversations_collection = Models.MongoHelper.database.GetCollection<VeggieBack.Models.Conversation>("conversation");
                var filter = Builders<VeggieBack.Models.Conversation>.Filter.Eq("_id", Storage.Instance.actualConversation);
                var result = Models.MongoHelper.conversations_collection.Find(filter).FirstOrDefault();
                    result.messages.Add(message);
                var update = Builders<VeggieBack.Models.Conversation>.Update.Set("messages", result.messages);
                var resultOperation = Models.MongoHelper.conversations_collection.UpdateOneAsync(filter, update);    
                    return true;
            } catch {
                return false;
            }
        }

        public List<Message> findConversationById(int idConversation){
            try {
                Models.MongoHelper.ConnectToMongoService();
                Models.MongoHelper.conversations_collection = Models.MongoHelper.database.GetCollection<VeggieBack.Models.Conversation>("conversation");
                var filter = Builders<VeggieBack.Models.Conversation>.Filter.Eq("_id", idConversation);
                var result = Models.MongoHelper.conversations_collection.Find(filter).FirstOrDefault();
                return result.messages;
            }
            catch {
                return null;
            }
        }

        public List<Message> getConversationMessages(string userOne, string userTwo) {
            try{
                Models.MongoHelper.ConnectToMongoService();
                Models.MongoHelper.conversations_collection = Models.MongoHelper.database.GetCollection<VeggieBack.Models.Conversation>("conversation");
                var filter = Builders<VeggieBack.Models.Conversation>.Filter.Eq("userOne.username", userOne);
                var filterTwo = Builders<VeggieBack.Models.Conversation>.Filter.Eq("userTwo.username", userTwo);
                var result = Models.MongoHelper.conversations_collection.Find(filter).FirstOrDefault();
                var resultTwo = Models.MongoHelper.conversations_collection.Find(filterTwo).FirstOrDefault();
                if (resultTwo != null && result != null){
                    List<Message> messages = result.messages;
                    return messages;
                }else{
                    return null;
                }
            }catch{
                return null;
            }
        }

        public List<Conversation> getConversationByUser(int idUser, bool status){
            try {
                string typeFilter = string.Empty;
                Models.MongoHelper.ConnectToMongoService();
                Models.MongoHelper.conversations_collection = Models.MongoHelper.database.GetCollection<VeggieBack.Models.Conversation>("conversation");
                if (status){
                    typeFilter = "userOne._id";
                }else {
                    typeFilter = "userTwo._id";
                      
                }
                var filter = Builders<VeggieBack.Models.Conversation>.Filter.Eq(typeFilter, idUser);
                var result = Models.MongoHelper.conversations_collection.Find(filter);
                return (List<Conversation>) result;
            }
            catch {
                return null;
            }
        }

        public bool findConversation(string userOne, string userTwo){
            try {
                Models.MongoHelper.ConnectToMongoService();
                Models.MongoHelper.conversations_collection = Models.MongoHelper.database.GetCollection<VeggieBack.Models.Conversation>("conversation");
                var filter = Builders<VeggieBack.Models.Conversation>.Filter.Eq("userOne.username", userOne);
                var filterTwo = Builders<VeggieBack.Models.Conversation>.Filter.Eq("userTwo.username", userTwo);
                var result = Models.MongoHelper.conversations_collection.Find(filter).FirstOrDefault();
                var resultTwo = Models.MongoHelper.conversations_collection.Find(filterTwo).FirstOrDefault();
                if (resultTwo != null && result != null) {
                    return true;
                }else {
                    return false;
                }
            }catch {
                return false;
            }
        }

        public List<Message> decryptionMessages(List<Message> messages){
            List<Message> decryptMessage = new List<Message>();
            foreach (var message in messages) {
                SDES encryption = new SDES(); //Objeto que ecriptará el mensaje.
                DiffieHellman df = new DiffieHellman();
                message.message = encryption.CifradoDecifrado(message.message, false, df.getPrivateKey(Storage.Instance.actualConversation.firstKey, Storage.Instance.actualConversation.secondKey));
                decryptMessage.Add(message);
            }
            return decryptMessage;
        }
     }
}

