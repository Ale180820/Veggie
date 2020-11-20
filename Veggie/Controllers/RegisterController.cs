using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using Veggie.System;
using VeggieBack.Controllers;
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
                var response = APIConnection.WebApliClient.PostAsync("api/create", user).Result;
                if (response.IsSuccessStatusCode) {
                    return RedirectToAction("Index", "Chat");
                }else {
                    return RedirectToAction("Error", "Home");
                }
            }catch {
                return View();
            }
        }

        //Construye el objeto con lo que se encuentra en los componentes
        public User constructObject(IFormCollection collection) {
            CesarCipher encryption = new CesarCipher();
            var newUser = new User {
                username = collection["username"],
                password = encryption.Encryption(collection["password"]),
                nameUser = collection["name"],
                lastNameUser = collection["lastname"],
                emailUser = collection["email"],
            };
            newUser._id = newUser.generateId();
            newUser.statusUser = "Avaiable";
            return newUser;
        }
    }
}
