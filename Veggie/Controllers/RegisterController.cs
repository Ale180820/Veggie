using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Veggie.Controllers {
    public class RegisterController : Controller {

        public IActionResult Index() {
            return View("Register");
        }

        public IActionResult Registers(FormCollection collection) {
            return View("Index");
        }
    }
}
