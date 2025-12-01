using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication6.Models;
using WebApplication6.ViewModels;

namespace WebApplication6.Controllers
{
    public class OrderTrackingController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        // GET: OrderTracking
        // Hiển thị danh sách các đơn hàng
        public ActionResult Index()
        {
            // 1. Kiểm tra Session để lấy ID khách hàng hiện tại
            if (Session["IDCus"] == null)
            {
                // Nếu chưa có session (chưa mua hàng lần nào), trả về danh sách rỗng
                return View(new List<ShippingOrderViewModel>());
            }

            int customerId = (int)Session["IDCus"];

            // 2. Lọc đơn hàng theo ID khách hàng
            var orders = db.OrderProes
                .Include(o => o.Customer)
                .Where(o => o.IDCus == customerId)
                .OrderByDescending(o => o.DateOrder)
                .ToList();

            // Map sang ViewModel để hiển thị
            var viewModels = orders.Select(o => new ShippingOrderViewModel
            {
                OrderID = o.ID,
                OrderDate = o.DateOrder,
                CustomerName = o.Customer != null ? o.Customer.NameCus : "Khách vãng lai",
                CustomerPhone = o.Customer != null ? o.Customer.PhoneCus : "",
                ShippingAddress = o.AddressDeliverry,
                // Tính tổng tiền sơ bộ cho Index
                TotalAmount = o.OrderDetails.Sum(d => (decimal)(d.UnitPrice ?? 0) * (d.Quantity ?? 0)),
                Items = new List<ShippingOrderItemViewModel>() // Index không cần load chi tiết từng món để nhẹ
            }).ToList();

            return View(viewModels);
        }

        // Action xóa Session (Xóa lịch sử mua hàng tạm thời)
        public ActionResult ClearSession()
        {
            Session["IDCus"] = null;
            return RedirectToAction("Index");
        }

        // GET: OrderTracking/Details/5
        // Hiển thị chi tiết đơn hàng
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            OrderPro order = db.OrderProes.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            var vm = new ShippingOrderViewModel
            {
                OrderID = order.ID,
                OrderDate = order.DateOrder,
                CustomerName = order.Customer != null ? order.Customer.NameCus : "Khách vãng lai",
                CustomerPhone = order.Customer != null ? order.Customer.PhoneCus : "",
                ShippingAddress = order.AddressDeliverry,
                Items = order.OrderDetails.Select(d => new ShippingOrderItemViewModel
                {
                    ProductName = d.Product != null ? d.Product.NamePro : "Unknown",
                    ProductImage = d.Product != null ? d.Product.ImagePro : "",
                    Quantity = d.Quantity ?? 0,
                    Price = (decimal)(d.UnitPrice ?? 0)
                }).ToList()
            };

            // Tính tổng tiền chính xác từ chi tiết
            vm.TotalAmount = vm.Items.Sum(i => i.TotalPrice);

            return View(vm);
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