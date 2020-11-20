using Microsoft.AspNetCore.Mvc;

namespace Veggie.Controllers {
    public class ChatController : Controller {
        // GET: ChatController
        public ActionResult Index() {
            return View("Chat");
        }

       
    }
}
