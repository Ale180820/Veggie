using System.Diagnostics;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Veggie.Models;
using Veggie.System;
using VeggieBack.Controllers;
using VeggieBack.Models;

namespace Veggie.Controllers {
    public class HomeController : Controller {

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        public IActionResult Index() {
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
                return RedirectToAction("Index", "Chat");
                var userLogin = constructObject(collection);
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(userLogin);
                var user = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = APIConnection.WebApliClient.PostAsync("api/login", user).Result;
                if (response.IsSuccessStatusCode) {
                    return RedirectToAction("Index", "Chat");
                } else {
                    //Mostrar error de datos equivocados en login.
                    return RedirectToAction("Error", "Home");
                }
            }
            catch {
                return RedirectToAction("Error", "Home");
            }
        }

        public void getUserLogin(string userLogin) {
            string id = "";
            if (userLogin != null) {
                id = userLogin;
                
            }
            else {
                id = "null";                
            }
        }

        public string contact(string user) {
            return "Manuel";
        }

        public string GetID(string email) {
            string valor = "";
            var userLogin = new User {
                emailUser = email
            };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(userLogin);
            var user = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = APIConnection.WebApliClient.PostAsync("api/findUserByEmail", user).Result;
            if (response.IsSuccessStatusCode) {
                valor = response.Content.ToString();
            }
            else {
                valor = "null";
            }
            return valor;
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
