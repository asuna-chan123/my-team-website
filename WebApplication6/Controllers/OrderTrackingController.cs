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

        // Hiển thị chi tiết đơn hàng
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Lấy đơn hàng với đầy đủ thông tin liên quan
            var order = db.OrderProes
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails.Select(d => d.Product))
                .FirstOrDefault(o => o.ID == id);

            if (order == null) return HttpNotFound();

            // Lấy thông tin khách hàng từ Session (đã đăng nhập)
            var customerName = Session["Username"]?.ToString() ?? "Khách hàng";
            var customer = db.Customers.Find((int)Session["customerID"]);

            // Map sang ViewModel
            var vm = new ShippingOrderVM
            {
                OrderID = order.ID,
                OrderDate = order.DateOrder,
                CustomerName = customerName,
                CustomerPhone = customer?.PhoneCus ?? "",
                ShippingAddress = order.AddressDeliverry,
                Status = order.Status,

                // Lấy chi tiết sản phẩm với thông tin variant
                Items = order.OrderDetails.Select(d =>
                {
                    var variant = db.ProductVariants
                        .Include(v => v.Color)
                        .Include(v => v.Size)
                        .FirstOrDefault(v => v.ProductID == d.IDProduct && !v.IsDeleted);

                    return new ShippingOrderItemViewModel
                    {
                        ProductName = d.Product?.NamePro ?? "Unknown",
                        ProductImage = "/Content/images/" + (variant?.ImagePro ?? "no-image.png"),
                        Quantity = d.Quantity ?? 0,
                        Price = (decimal)(d.UnitPrice ?? 0),
                        Color = variant?.Color?.ColorName ?? "",
                        Size = variant?.Size?.SizeName ?? ""
                    };
                }).ToList()
            };

            // Tính tổng tiền
            vm.TotalAmount = vm.Items.Sum(i => i.TotalPrice);

            return View(vm);
        }


    }
}