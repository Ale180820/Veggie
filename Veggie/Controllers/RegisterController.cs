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

        //Retorna la vista de creación
        public IActionResult Create(){
            return View("Create");
        }

        //Recibe el método de creación de usuario
        [HttpPost]
        public ActionResult Create(IFormCollection collection) {
            try {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(constructObject(collection));
                var response = APIConnection.WebApliClient.PostAsync("api/createUser", new StringContent(json.ToString(), Encoding.UTF8, "application/json")).Result;
                if (response.IsSuccessStatusCode) {
                    return RedirectToAction("Index", "Home");
                }else {
                    // ----- ERROR EN CREACIÓN DE USUARIO ------
                    // ------ [MOSTRAR NOTIFICACIÓN] ------
                    return RedirectToAction("Error", "Home"); 
                }
            }catch {
                return View();
            }
        }

        //Construye el objeto (usuario) con lo que se encuentra en los componentes
        public User constructObject(IFormCollection collection) {
            CesarCipher encryption = new CesarCipher();
            var newUser = new User {
                username = collection["username"],
                password = encryption.Encryption(collection["password"]),
                nameUser = collection["name"],
                lastNameUser = collection["lastname"],
                emailUser = collection["email"],
            };
            newUser.statusUser = "Avaiable";
            return newUser;
        }
    }
}
