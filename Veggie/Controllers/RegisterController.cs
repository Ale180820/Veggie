using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VeggieAPI.Controllers;
using VeggieBack.Models;

namespace Veggie.Controllers {
    public class RegisterController : Controller {

        public IActionResult Index() {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection) {
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
