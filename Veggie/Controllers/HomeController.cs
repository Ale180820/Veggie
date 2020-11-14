using System.Diagnostics;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Veggie.Models;
using Veggie.System;
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
                var userLogin = constructObject(collection);
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(userLogin);
                var user = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = APIConnection.WebApliClient.PostAsync("api/login", user).Result;
                return View();
            }catch {
                return RedirectToAction("Error", "Home");
            }
        }

        public User constructObject(IFormCollection collection){
            var newUser = new User { 
                emailUser = collection["email"],
                password = collection["password"],
            };
            return newUser;
        }


    }
}
