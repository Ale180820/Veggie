﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Veggie.Models;
using Veggie.APISystem;
using VeggieBack.Controllers;
using VeggieBack.Models;
using System.Text.Json;
using Veggie.Services;

namespace Veggie.Controllers {
    public class HomeController : Controller {

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        public ActionResult Index() {
            if (TempData["smsFail"] != null) {
                ViewBag.Message = "No ha sido posible iniciar sesión, intentelo nuevamente.";
            }
            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public ActionResult Login(IFormCollection collection) {
            try {
                GetID(collection["email"]);
                var userLogin = constructObject(collection);
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(userLogin);
                var user = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = APIConnection.WebApliClient.PostAsync("api/login", user).Result;
                if (response.IsSuccessStatusCode) {
                    fillConversations();
                    return RedirectToAction("Index", "Chat");

                } else {
                    TempData["smsFail"] = "No ha sido posible iniciar sesión, intentelo nuevamente.";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch {
                TempData["smsFail"] = "No ha sido posible iniciar sesión, intentelo nuevamente.";
                return RedirectToAction("Index", "Home");
            }
        }

        //Get created conversations
        public bool fillConversations() {
            var userConver = Storage.Instance.idUser;
            var jsonU = Newtonsoft.Json.JsonConvert.SerializeObject(userConver);
            var userU = new StringContent(jsonU.ToString(), Encoding.UTF8, "application/json");
            var responseU = APIConnection.WebApliClient.PostAsync("api/getConversationByUserId", userU).Result;
            if (responseU.IsSuccessStatusCode) {
                var resultUser = responseU.Content.ReadAsStringAsync().Result;
                var contactsU = JsonSerializer.Deserialize<List<Conversation>>(resultUser);
                Storage.Instance.conversations = contactsU;
                foreach (var item in Storage.Instance.conversations) {
                    if (item.userOne._id != Storage.Instance.idUser) {
                        Contacts userC = new Contacts {
                            username = item.userOne.username,
                            email = item.userOne.emailUser
                        };
                        Storage.Instance.contacts.Add(userC);
                    }
                    else if (item.userTwo._id != Storage.Instance.idUser) {
                        Contacts userC = new Contacts {
                            username = item.userTwo.username,
                            email = item.userTwo.emailUser
                        };
                        Storage.Instance.contacts.Add(userC);
                    }
                }
                return true;
            }
            return false;
        }


        public string GetID(string email) {
            var userLogin = new User {
                emailUser = email
            };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(userLogin);
            var user = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = APIConnection.WebApliClient.PostAsync("api/findUserByEmail", user).Result;
            if (response.IsSuccessStatusCode) {
                var result = response.Content.ReadAsStringAsync().Result;
                var id = JsonSerializer.Deserialize<User>(result);
                Storage.Instance.idUser = id._id;
                return id._id.ToString();
            }
            else {
                return "null";
            }
        }

        public User constructObject(IFormCollection collection){
            CesarCipher encryption = new CesarCipher();
            var newUser = new User { 
                emailUser = collection["email"],
                password = encryption.Encryption(collection["password"]),
            };
            return newUser;
        }


    }
}
