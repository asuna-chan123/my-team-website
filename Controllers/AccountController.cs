using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdidasStoreMVC.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/LoginSignup
        public ActionResult LoginSignup()
        {
            return View();
        }

        // POST: /Account/LoginSignup
        [HttpPost]
        public ActionResult LoginSignup(string Email)
        {
            ViewBag.Message = "Cảm ơn bạn đã đăng ký: " + Email;
            return View();
        }
    }
}