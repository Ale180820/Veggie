using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Veggie.System;
using VeggieBack.Models;

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
                return View();
            }catch {
                return RedirectToAction("Error", "Home");
            }
        }


        public Conversation create(User user, IFormCollection collection) {
            Conversation conversation = new Conversation();
            conversation.userOne = user;
            user.username = collection["user"];
            conversation.userTwo = user;
            return conversation;
        }
       
    }
}
