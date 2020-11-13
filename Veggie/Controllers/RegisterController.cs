using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using Veggie.System;
using VeggieAPI.Controllers;
using VeggieBack.Models;

namespace Veggie.Controllers
{
    public class RegisterController : Controller {

        public IActionResult Create(){
            return View("Create");
        }

        [HttpPost]
        public ActionResult Create(IFormCollection collection) {
            try {
                var userComplete = constructObject(collection);
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(userComplete);
                var user = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = APIConnection.WebApliClient.PostAsync("api/createUser", user).Result;
                if (response.IsSuccessStatusCode) {
                    return RedirectToAction("Index", "Home");
                }else {
                    return RedirectToAction("Error", "Home");
                }
            }catch {
                return View();
            }
        }


        public User constructObject(IFormCollection collection) {
            var newUser = new User {
                username = collection["username"],
                password = collection["password"],
                nameUser = collection["name"],
                lastNameUser = collection["lastname"],
                emailUser = collection["email"],
            };
            newUser.userId = newUser.generateId();
            return newUser;
        }
    }
}
