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

        // Helper: Lấy service giỏ hàng
        private CartService GetCartService()
        {
            return new CartService(Session);
        }

        // GET: OrderConfirm - Hiển thị trang xác nhận đơn hàng
        public ActionResult Index(int? id)
        {
            // Nếu không có ID đơn hàng, chuyển sang tạo đơn mới
            if (id == null)
            {
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

        // Tạo đơn hàng tự động từ giỏ hàng
        public ActionResult AutoCreate()
        {
            var cart = GetCartService().GetCart();

            // Kiểm tra giỏ hàng có sản phẩm không
            if (cart.Items.Count() == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            try
            {
                // Lấy ID khách hàng từ Session (đã đăng nhập)
                int customerId = (int)Session["IDCus"];

                // Tạo đơn hàng mới
                var order = new OrderPro
                {
                    DateOrder = DateTime.Now,
                    AddressDeliverry = "Nhận tại cửa hàng",
                    IDCus = customerId
                };

                db.OrderProes.Add(order);
                db.SaveChanges();

                // Lưu chi tiết đơn hàng và trừ tồn kho
                foreach (var item in cart.Items)
                {
                    // Thêm chi tiết đơn hàng
                    var detail = new OrderDetail
                    {
                        IDOrder = order.ID,
                        IDProduct = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = (double)item.UnitPrice
                    };
                    db.OrderDetails.Add(detail);

                    // Trừ số lượng trong kho
                    var variant = db.ProductVariants.FirstOrDefault(v =>
                        v.ProductID == item.ProductID && !v.IsDeleted);

                    if (variant != null)
                    {
                        variant.StockQty -= item.Quantity;
                        if (variant.StockQty < 0)
                        {
                            variant.StockQty = 0; // Đảm bảo không âm
                        }
                    }
                }
                db.SaveChanges();

                // Chuyển về trang xác nhận thành công
                return RedirectToAction("Index", new { id = order.ID });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction("Index", "Cart");
            }
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