using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Net.Http;
using VeggieBack.Models;
using VeggieBack.Controllers;
using System;

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


        public void cipherMessage(bool cipher, string message) {
            DiffieHellman diffie = new DiffieHellman();
            SDES sdesEncryption = new SDES();
            var result = "";
            //Agregar validacion si en la base de datos existe una lleve agregada
            int key = 0;
            //key = valor de la key en la base de datos
            if (key == 0) {
                var randomNumber = new Random();
                int value = randomNumber.Next(5, 999);
                key = diffie.GenerateKeys(value);
                result = sdesEncryption.CifradoDecifrado(message, cipher, key);
                //Enviar a la base de datos
            }
            else {
                result = sdesEncryption.CifradoDecifrado(message, cipher, key);
                //Enviar a la base de datos
            }
            
        }
    }
}
