using System;
using System.Linq;
using System.Web.Mvc;
using WebApplication6.Models;
using WebApplication6.ViewModels.CartViewModels;

namespace WebApplication6.Controllers
{
    public class OrderController : Controller
    {
        DBSportStoreEntities db = new DBSportStoreEntities();

        // GET: /Order/Checkout
        public ActionResult Checkout()
        {
            //if (Session["CustomerID"] == null)
            //    return RedirectToAction("Login", "Account");

            return View();
        }

        // POST: /Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(string FullName, string Address, string Phone)
        {
            if (Session["CustomerID"] == null)
                return RedirectToAction("Login", "Account");

            int customerId = (int)Session["CustomerID"];

            var cart = Session["Cart"] as Cart;
            if (cart == null || !cart.Items.Any())
                return RedirectToAction("Index", "Cart");

            // Lấy customer trong DB để update thông tin
            var customer = db.Customers.Find(customerId);
            if (customer != null)
            {
                customer.NameCus = FullName;
                customer.PhoneCus = Phone;
                db.SaveChanges();
            }

            // 1. Tạo OrderPro
            OrderPro order = new OrderPro()
            {
                IDCus = customerId,
                DateOrder = DateTime.Now,
                AddressDeliverry = Address   // ✔ đúng tên trường
            };

            db.OrderProes.Add(order);
            db.SaveChanges();

            // 2. Thêm OrderDetails
            foreach (var item in cart.Items)
            {
                OrderDetail detail = new OrderDetail()
                {
                    IDOrder = order.ID,
                    IDProduct = item.ProductID,
                    Quantity = item.Quantity,
                    UnitPrice = (double)item.UnitPrice
                };
                db.OrderDetails.Add(detail);
            }

            db.SaveChanges();

            // Xóa giỏ
            Session["Cart"] = null;

            // Chuyển qua OrderConfirm
            return RedirectToAction("Index", "OrderConfirm", new { id = order.ID });
        }

    }
}
