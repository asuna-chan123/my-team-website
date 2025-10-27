using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdidasStoreMVC.Controllers
{
    public class CheckoutController : Controller
    {
        // GET: /Checkout/
        public ActionResult Index()
        {
            return View();
        }

        // POST: /Checkout/
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            // Ở đây bạn có thể xử lý dữ liệu thanh toán (nếu cần)
            ViewBag.Message = "Đặt hàng thành công!";
            return View();
        }
    }
}