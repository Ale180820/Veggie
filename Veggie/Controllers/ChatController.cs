using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Veggie.APISystem;
using VeggieBack.Models;
using Veggie.Services;
using System.Text.Json;
using System;
using Microsoft.AspNetCore.Hosting;
using Veggie.Models;
using System.IO;

namespace Veggie.Controllers {
    public class ChatController : Controller {

        // GET: ChatController
        public ActionResult Index() {
            
            if (TempData["search"] != null) {
                ViewBag.Message = "Encontrado";
                ViewBag.Search = "Encontrado";
            }
            return View("nChat");
        }
        [HttpPost]
        public ActionResult FindMessages() {
            return View("FindMessages");
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
                if (response.IsSuccessStatusCode) {
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

        public FileResult Download(string fileId) {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(fileId);
            var messageS = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = APIConnection.WebApliClient.PostAsync("api/downloadM", messageS).Result;
            if (response.IsSuccessStatusCode) {
                var result = response.Content.ReadAsStringAsync().Result;
                var search = JsonSerializer.Deserialize<DownloadFile>(result);
                return File(search.file, System.Net.Mime.MediaTypeNames.Application.Octet, search.fileName);
            }
            else
            {
                return default;
            }
        }
        public Route route = new Route();
        public ChatController(IWebHostEnvironment env)
        {
            route.hostEnvironment = env;
        }

        [HttpPost]
        public ActionResult sendM(IFormFile file) {
            try {
                if (string.IsNullOrWhiteSpace(route.webRoot())) {
                    route.hostEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "Cloud");
                }

                if (!Directory.Exists(route.setCDirectory())) {
                    Directory.CreateDirectory(route.setCDirectory());
                }

                var keysF = route.setRoute(file.FileName);
                using var fileKeys = System.IO.File.Create(keysF);
                file.CopyTo(fileKeys);
                fileKeys.Flush();
                fileKeys.Close();
                byte[] bytes = System.IO.File.ReadAllBytes(route.getRoute(file.FileName));

                DateTime now = DateTime.Now;
                var messageFile = new Message {
                    receivingUser = Storage.Instance.actualConversation.sendUser,
                    sendingUser = Storage.Instance.idUser.ToString(),
                    message = "",
                    typeMessage = false,
                    messageTime = now,
                };

                var fileInfo = new VeggieBack.Models.File {
                    file = bytes,
                    fileName = file.FileName,
                };

                var messageSend = new SendMessage
                {
                    idConversation = Storage.Instance.conversationId,
                    messageSend = messageFile,
                    typeMessage = false,
                    fileSend = fileInfo
                };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(messageSend);
                var messageS = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = APIConnection.WebApliClient.PostAsync("api/sendMessage", messageS).Result;
                if (response.IsSuccessStatusCode) {
                    refreshChat();
                }
                return RedirectToAction("Index", "Chat");
            }
            catch (Exception) {
                TempData["smsFail"] = "...";
                return RedirectToAction("Index", "Chat");
            }
            
        }

        //Method for send message to others people
        public ActionResult SendMessage(string message) {
            try {
                DateTime now = DateTime.Now;
                var messageText = new Message {
                    receivingUser = Storage.Instance.actualConversation.sendUser,
                    sendingUser = Storage.Instance.idUser.ToString(),
                    message = message,
                    messageTime = now,
                    typeMessage = true
                };
                var messageSend = new SendMessage {
                    messageSend = messageText,
                    idConversation = Storage.Instance.conversationId,
                    typeMessage = true
                };
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(messageSend);
                var messageS = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = APIConnection.WebApliClient.PostAsync("api/sendMessage", messageS).Result;
                if (response.IsSuccessStatusCode) {
                    refreshChat();
                }
                return RedirectToAction("Index", "Chat");
            }
            catch (Exception) {
                TempData["smsFail"] = "...";
                return RedirectToAction("Index", "Chat");
            }
            
        }

        public void refreshChat() {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(Storage.Instance.conversationId);
            var user = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = APIConnection.WebApliClient.PostAsync("api/getAllMessage", user).Result;
            if (response.IsSuccessStatusCode) {
                var result = response.Content.ReadAsStringAsync().Result;
                var search = JsonSerializer.Deserialize<List<SendMessage>>(result);
                if (search.Count != 0) {
                    Storage.Instance.messages = search;
                }
            }
        }

        #region Messages
        //Search message
        public ActionResult searchMessage(string searchMessage) {
            try {
                FindMessage findMessage = new FindMessage {
                    idConversation = Storage.Instance.conversationId,
                    message = searchMessage
                };
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(findMessage);
                var messageS = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = APIConnection.WebApliClient.PostAsync("api/getSpecificMessage", messageS).Result;
                if (response.IsSuccessStatusCode) {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var sendMessage = JsonSerializer.Deserialize<List<Message>>(result);
                    Storage.Instance.findMessages = sendMessage;
                }
                TempData["Search"] = "...";
                return RedirectToAction("Index", "Chat");
            }
            catch (Exception) {
                TempData["smsFail"] = "...";
                return RedirectToAction("Index", "Chat");
            }
            
        }

        //Refresh chat 
        public void refresh(string actual) {
            getMessages(Storage.Instance.actualConversation.sendUser);
        }

        //Method for get (if exist) message
        public ActionResult getMessages(string usernameS) {
            try {
                var userLogin = new Entry {
                    actualUser = Storage.Instance.idUser.ToString(),
                    sendUser = usernameS
                };
                fillConversations();
                Storage.Instance.conversationId = findConversationId(usernameS);
                Storage.Instance.actualConversation = userLogin;
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(Storage.Instance.conversationId);
                var user = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = APIConnection.WebApliClient.PostAsync("api/getAllMessage", user).Result;
                if (response.IsSuccessStatusCode) {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var search = JsonSerializer.Deserialize<List<SendMessage>>(result);
                    if (search.Count != 0) {
                        Storage.Instance.messages = search;
                    }
                    else {
                        Storage.Instance.messages = search;
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

        //Find ID conversation
        public int findConversationId(string username) {
            var find = 0;
            foreach (var item in Storage.Instance.conversations) {
                if (item.userOne.username.Equals(username)) {
                    find = item._id;
                }
            }
            if (find == 0) {
                foreach (var item in Storage.Instance.conversations) {
                    if (item.userTwo.username.Equals(username)) {
                        find = item._id;
                    }
                }
                return find;
            }
            else {
                return find;
            }
        }

        //Update the conversations
        public bool fillConversations() {
            try {
                var userConver = Storage.Instance.idUser;
                var jsonU = Newtonsoft.Json.JsonConvert.SerializeObject(userConver);
                var userU = new StringContent(jsonU.ToString(), Encoding.UTF8, "application/json");
                var responseU = APIConnection.WebApliClient.PostAsync("api/getConversationByUserId", userU).Result;
                if (responseU.IsSuccessStatusCode) {
                    var resultUser = responseU.Content.ReadAsStringAsync().Result;
                    var contactsU = JsonSerializer.Deserialize<List<Conversation>>(resultUser);
                    Storage.Instance.conversations = contactsU;
                    return true;
                }
                return false;
            }
            catch (Exception) {
                return false;
            }
           
        }
        #endregion



    }
}
