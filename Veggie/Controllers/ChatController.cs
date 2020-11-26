using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Veggie.System;
using VeggieBack.Models;
using Veggie.Services;
using System.Text.Json;

namespace Veggie.Controllers {
    public class ChatController : Controller {

        // GET: ChatController
        public ActionResult Index() {
            return View("Chat");
        }

        [HttpPost]
        public ActionResult SendMessage (IFormCollection collection) {
            try {
                
                return View();
            }catch {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult CreateConversation(IFormCollection collection) {
            try {
                var conversatioComplete = create(null, collection);
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(conversatioComplete);
                var converstation = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = APIConnection.WebApliClient.PostAsync("api/createConversation", converstation).Result;
                return RedirectToAction("Index", "Chat");
            }
            catch {
                return RedirectToAction("Index", "Chat");
            }
        }
        public ActionResult SearchUser(string username) {
            var userLogin = username;
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(userLogin);
            var user = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = APIConnection.WebApliClient.PostAsync("api/findUserByUsernameExist", user).Result;
            if (response.IsSuccessStatusCode) {
                var result = response.Content.ReadAsStringAsync().Result;
                var search = JsonSerializer.Deserialize<User>(result);
                Storage.Instance.searchUsers = search;
            }
            return RedirectToAction("Index", "Chat");
        }

        public void starConversation(string idUsername, string sendUser) {
            var userLogin = new Entry { 
                actualUser = idUsername,
                sendUser = sendUser
            };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(userLogin);
            var user = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = APIConnection.WebApliClient.PostAsync("api/createConversation", user).Result;
            if (response.IsSuccessStatusCode) {
                var result = response.Content.ReadAsStringAsync().Result;
                var search = JsonSerializer.Deserialize<User>(result);
                Storage.Instance.searchUsers = search;
            }
        }
        public string contact(string user)
        {
            return "Manuel";
        }


        public Conversation create(User user, IFormCollection collection) {
            Conversation conversation = new Conversation();
            conversation.userOne = user;
            user.username = collection["user"];
            conversation.userTwo = user;
            conversation.messages = new Dictionary<string, Message>();
            return conversation;
        }
       
    }
}
