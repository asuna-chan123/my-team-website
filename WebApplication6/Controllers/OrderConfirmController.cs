using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication6.Models;
using WebApplication6.ViewModels;
using WebApplication6.ViewModels.CartViewModels;

namespace WebApplication6.Controllers
{
    public class OrderConfirmController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        // Helper: Lấy dịch vụ giỏ hàng
        private CartService GetCartService()
        {
            return new CartService(Session);
        }

        // GET: OrderConfirm
        // Hiển thị trang thông báo thành công (Thay thế cho trang Checkout cũ)
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                // Nếu không có ID, thử tạo đơn hàng tự động từ giỏ hàng hiện tại
                return RedirectToAction("AutoCreate");
            }

            // Lấy thông tin đơn hàng để hiển thị
            var order = db.OrderProes.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // Action mới: Tự động tạo đơn hàng (Bỏ qua bước điền form)
        public ActionResult AutoCreate()
        {
            var cart = GetCartService().GetCart();
            if (cart.Items.Count() == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            try
            {
                // 1. Kiểm tra xem đã có khách hàng trong Session chưa
                int customerId;
                if (Session["IDCus"] != null)
                {
                    customerId = (int)Session["IDCus"];
                }
                else
                {
                    // Nếu chưa có, tạo khách hàng mới (Khách vãng lai)
                    var customer = new Customer();
                    customer.NameCus = "Khách vãng lai";
                    customer.PhoneCus = "0000000000";

                    db.Customers.Add(customer);
                    db.SaveChanges();

                    customerId = customer.IDCus;
                    Session["IDCus"] = customerId; // Lưu vào Session
                }

                // 2. Tạo đơn hàng
                var order = new OrderPro();
                order.DateOrder = DateTime.Now;
                order.AddressDeliverry = "Nhận tại cửa hàng"; // Mặc định
                order.IDCus = customerId;
                
                db.OrderProes.Add(order);
                db.SaveChanges();

                // 3. Lưu chi tiết
                foreach (var item in cart.Items)
                {
                    var detail = new OrderDetail();
                    detail.IDOrder = order.ID;
                    detail.IDProduct = item.ProductID;
                    detail.Quantity = item.Quantity;
                    detail.UnitPrice = (double)item.UnitPrice;
                    db.OrderDetails.Add(detail);
                }
                db.SaveChanges();

                // 4. Xóa giỏ
                GetCartService().ClearCart();

                // 5. Chuyển hướng về trang Index (hiển thị Success)
                return RedirectToAction("Index", new { id = order.ID });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction("Index", "Cart");
            }
        }

        // GET: OrderConfirm/Success/5
        // Giữ lại action này nếu cần, hoặc có thể xóa nếu đã dùng Index làm Success
        public ActionResult Success(int? id)
        {
            if (id == null) return RedirectToAction("Index", "Home");
            var order = db.OrderProes.Find(id);
            return View(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}