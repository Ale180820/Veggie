using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VeggieAPI.Controllers;
using VeggieBack.Models;

namespace Veggie.Controllers {
    public class RegisterController : Controller {
        public IActionResult Create(){
            return View("Create");
        }

        [HttpPost]
        public ActionResult Create(IFormCollection collection) {
            RequestVeggieController request = new RequestVeggieController();
            try {
                var newUser = new User {

                    username = collection["username"],
                    password = collection["password"],
                    nameUser = collection["name"],
                    lastNameUser = collection["lastname"],
                    emailUser = collection["email"],
                };
                request.createUser(newUser);
                return View();
            }catch {
                return View();
            }
        }
    }
}
