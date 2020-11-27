using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Veggie.System;
using VeggieBack.Models;
using Veggie.Services;
using System.Text.Json;
using System;

namespace Veggie.Controllers {
    public class ChatController : Controller {

        // GET: ChatController
        public ActionResult Index() {
            if (TempData["smsFail"] != null) {
                ViewBag.Message = "Ha ocurrido un error en la ejecución, intentelo nuevamente.";
            }
            return View("nChat");
        }

        [HttpPost]
        public ActionResult SendMessage (IFormCollection collection) {
            try {
                return View();
            }catch {
                return RedirectToAction("Index", "Chat");
            }
        }
        #region Search users
        //Search and star conversation with a user
        public ActionResult SearchUser(string username) {
            try {
                var userLogin = username;
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(userLogin);
                var user = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = APIConnection.WebApliClient.PostAsync("api/findUserByUsernameExist", user).Result;
                if (response.IsSuccessStatusCode)
                {
                    var resultUser = response.Content.ReadAsStringAsync().Result;
                    var searchUser = JsonSerializer.Deserialize<User>(resultUser);
                    Storage.Instance.searchUsers = searchUser;
                    var starCon = new Entry {
                        actualUser = Storage.Instance.idUser.ToString(),
                        sendUser = Storage.Instance.searchUsers.username
                    };
                    if (conversationExist(starCon) != "false") {
                        addContact(starCon, searchUser);
                    }
                    else {
                        return RedirectToAction("Index", "Chat");
                    }

                    return RedirectToAction("Index", "Chat");
                }
                else {
                    TempData["smsFail"] = "...";
                    return RedirectToAction("Index", "Chat");
                }
            }
            catch (Exception) {
                TempData["smsFail"] = "...";
                return RedirectToAction("Index", "Chat");
            }
            
        }

        //If exist a conversation between the users
        public string conversationExist(Entry starCon) {
            var jsonU = Newtonsoft.Json.JsonConvert.SerializeObject(starCon);
            var userU = new StringContent(jsonU.ToString(), Encoding.UTF8, "application/json");
            var responseU = APIConnection.WebApliClient.PostAsync("api/findConversationByUsers", userU).Result;
            if (responseU.IsSuccessStatusCode) {
                var resultConversation = responseU.Content.ReadAsStringAsync().Result;
                return resultConversation;
            }
            else {
                return "false";
            }
        }

        //Add contact to contact's List
        public void addContact(Entry starCon, User searchUser) {
            var json2 = Newtonsoft.Json.JsonConvert.SerializeObject(starCon);
            var user2 = new StringContent(json2.ToString(), Encoding.UTF8, "application/json");
            var response2 = APIConnection.WebApliClient.PostAsync("api/createConversation", user2).Result;
            var newUser = new Contacts {
                username = searchUser.username,
                email = searchUser.emailUser
            };
            if (response2.IsSuccessStatusCode) {
                Storage.Instance.contacts.Add(newUser);
            }
        }
        #endregion

        //Method for send message to others people
        public ActionResult SendMessage(string message) {
            DateTime now = DateTime.Now;
            var userLogin = new Message {
                receivingUser = Storage.Instance.searchUsers.username,
                sendingUser = Storage.Instance.idUser.ToString(),
                message = message,
                messageTime = now,
                typeMessage = true
            };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(userLogin);
            var messageSend = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = APIConnection.WebApliClient.PostAsync("api/sendMessage", messageSend).Result;
            if (response.IsSuccessStatusCode) {
                var result = response.Content.ReadAsStringAsync().Result;
                var sendMessage = JsonSerializer.Deserialize<List<Message>>(result);
                Storage.Instance.messages = sendMessage;
            }
            return RedirectToAction("Index", "Chat");
        }

        //Search message
        public void searchMessage(string searchMessage) {

        }

        //Refresh chat 
        public ActionResult refresh(string id) {
            return RedirectToAction("Index", "Chat");
        }

        //Method for get (if exist) message
        public ActionResult getMessages(string usernameS) {
            var userLogin = new Entry { 
                actualUser = Storage.Instance.idUser.ToString(),
                sendUser = usernameS
            };
            Storage.Instance.conversationId = findConversationId(usernameS);
            Storage.Instance.actualConversation = userLogin;
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(Storage.Instance.conversationId);
            var user = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = APIConnection.WebApliClient.PostAsync("api/getAllMessage", user).Result;
            if (response.IsSuccessStatusCode) {
                var result = response.Content.ReadAsStringAsync().Result;
                if (result != "false") {
                    var search = JsonSerializer.Deserialize<List<Message>>(result);
                    Storage.Instance.messages = search;
                }
                return RedirectToAction("Index", "Chat");
            }
            else {
                TempData["smsFail"] = "...";
                return RedirectToAction("Index", "Chat");
            }
        }
        public int findConversationId(string username) {
            var find = 0;
            var find2 = 0;
            find = Storage.Instance.conversations.Find(x => x.userOne.username == username)._id;
            find2 = Storage.Instance.conversations.Find(x => x.userTwo.username == username)._id;
            if (find != 0) {
                return find;
            }
            else if (find2!=0) {
                return find2;
            }
            else {
                return 0;
            }
        }
    }
}
