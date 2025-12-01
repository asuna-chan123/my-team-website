using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Newtonsoft.Json;
using WebApplication6.Models;
using WebApplication6.ViewModels;

namespace WebApplication6.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        public ActionResult Index(string search, int? status)
        {
            var orders = db.OrderProes
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                orders = orders.Where(o =>
                    o.ID.ToString().Contains(search) ||
                    o.Customer.NameCus.Contains(search) ||
                    o.Customer.PhoneCus.Contains(search) ||
                    o.Customer.EmailCus.Contains(search));
            }

            if (status.HasValue)
            {
                orders = orders.Where(o => o.Status == status.Value);
            }

            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(orders
                .OrderBy(o => o.DateOrder)
                .ToList());
        }
        public ActionResult Create()
        {
            var products = db.Products
                .Where(p => !p.IsDeleted)
                .ToList();

            var customers = db.Customers
                .Select(c => new
                {
                    c.IDCus,
                    c.NameCus,
                    c.PhoneCus,
                    c.EmailCus
                })
                .ToList();

            var vm = new OrderFormViewModel
            {
                Order = new OrderPro
                {
                    DateOrder = DateTime.Now,
                    Status = 2 
                },
                Details = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        Quantity = 1,
                        UnitPrice = null
                    }
                },
                ProductList = new SelectList(products, "ProductID", "NamePro")
            };

            ViewBag.ProductPrices = products
                .ToDictionary(p => p.ProductID, p => (decimal)(p.Price ?? 0));

            ViewBag.ColorList = new SelectList(db.Colors, "ColorID", "ColorName");
            ViewBag.SizeList = new SelectList(db.Sizes, "SizeID", "SizeName");

            ViewBag.CustomerJson =
                Newtonsoft.Json.JsonConvert.SerializeObject(customers);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OrderFormViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.CustomerName))
                ModelState.AddModelError("CustomerName", "Vui lòng nhập tên khách hàng.");

            if (string.IsNullOrWhiteSpace(vm.CustomerPhone))
                ModelState.AddModelError("CustomerPhone", "Vui lòng nhập số điện thoại.");

            //địa chỉ bắt buộc
            if (string.IsNullOrWhiteSpace(vm.Order.AddressDeliverry))
            {
                ModelState.AddModelError("Order.AddressDeliverry",
                    "Vui lòng nhập địa chỉ giao hàng");
            }

            // lọc các dòng detail hợp lệ
            var validDetails = vm.Details?
                .Where(d => d.IDProduct > 0 && d.Quantity > 0)
                .ToList() ?? new List<OrderDetail>();

            if (!validDetails.Any())
            {
                ModelState.AddModelError("", "Vui lòng chọn ít nhất 1 sản phẩm trong đơn");
            }

            Customer customer = null;

            //khách đã có 
            if (vm.ExistingCustomerId.HasValue)
            {
                var existed = db.Customers.Find(vm.ExistingCustomerId.Value);
                if (existed == null)
                {
                    ModelState.AddModelError("CustomerName", "Không tìm thấy khách hàng đã chọn.");
                }
                else
                {
                    // kiểm tra xem có chỉnh sửa thông tin không
                    bool edited =
                        (!string.IsNullOrWhiteSpace(vm.CustomerName) &&
                         vm.CustomerName != existed.NameCus) ||
                        (!string.IsNullOrWhiteSpace(vm.CustomerPhone) &&
                         vm.CustomerPhone != existed.PhoneCus) ||
                        (!string.IsNullOrWhiteSpace(vm.CustomerEmail) &&
                         vm.CustomerEmail != existed.EmailCus);

                    // nếu không chỉnh dùng lại khách cũ
                    if (!edited)
                        customer = existed;
                }
            }

            // Nếu customer vẫn null coi như KH mới, kiểm tra trùng & tạo mới
            if (customer == null)
            {

                if (!string.IsNullOrWhiteSpace(vm.CustomerPhone))
                {
                    bool phoneExists = db.Customers.Any(c => c.PhoneCus == vm.CustomerPhone);
                    if (phoneExists)
                        ModelState.AddModelError("CustomerPhone", "Số điện thoại này đã được dùng cho khách khác.");
                }

                if (!string.IsNullOrWhiteSpace(vm.CustomerEmail))
                {
                    bool emailExists = db.Customers.Any(c => c.EmailCus == vm.CustomerEmail);
                    if (emailExists)
                        ModelState.AddModelError("CustomerEmail", "Email này đã được sử dụng.");
                }

                if (!string.IsNullOrWhiteSpace(vm.CustomerUserName))
                {
                    bool userExists = db.Customers.Any(c => c.UserName == vm.CustomerUserName);
                    if (userExists)
                        ModelState.AddModelError("CustomerUserName", "Tên tài khoản đã tồn tại.");
                }

                // Nếu có lỗi quay lại view, không tạo KH mới
                if (!ModelState.IsValid)
                {
                    var products = db.Products.Where(p => !p.IsDeleted).ToList();
                    vm.ProductList = new SelectList(products, "ProductID", "NamePro");

                    var customers = db.Customers
                        .Select(c => new { c.IDCus, c.NameCus, c.PhoneCus, c.EmailCus })
                        .ToList();
                    ViewBag.CustomerJson =
                        Newtonsoft.Json.JsonConvert.SerializeObject(customers);

                    ViewBag.ProductPrices = products
                        .ToDictionary(p => p.ProductID, p => (decimal)(p.Price ?? 0));
                    ViewBag.ColorList = new SelectList(db.Colors, "ColorID", "ColorName");
                    ViewBag.SizeList = new SelectList(db.Sizes, "SizeID", "SizeName");

                    return View(vm);
                }

                // Không trùng thì tạo khách mới
                customer = new Customer
                {
                    NameCus = vm.CustomerName,
                    PhoneCus = vm.CustomerPhone,
                    EmailCus = vm.CustomerEmail,
                    UserName = vm.CustomerUserName, 
                    IsLocked = false
                };
                db.Customers.Add(customer);
                db.SaveChanges();
            }

            if (!ModelState.IsValid)
            {
                var products = db.Products.Where(p => !p.IsDeleted).ToList();
                vm.ProductList = new SelectList(products, "ProductID", "NamePro");

                var customers = db.Customers
                    .Select(c => new { c.IDCus, c.NameCus, c.PhoneCus, c.EmailCus })
                    .ToList();
                ViewBag.CustomerJson =
                    Newtonsoft.Json.JsonConvert.SerializeObject(customers);

                ViewBag.ProductPrices = products
                    .ToDictionary(p => p.ProductID, p => (decimal)(p.Price ?? 0));
                ViewBag.ColorList = new SelectList(db.Colors, "ColorID", "ColorName");
                ViewBag.SizeList = new SelectList(db.Sizes, "SizeID", "SizeName");

                return View(vm);
            }

            vm.Order.IDCus = customer.IDCus;
            vm.Order.Status = vm.Order.Status == 0 ? 1 : vm.Order.Status; 
            vm.Order.DateOrder = vm.Order.DateOrder ?? DateTime.Now;

            db.OrderProes.Add(vm.Order);
            db.SaveChanges();

            foreach (var d in validDetails)
            {
                d.IDOrder = vm.Order.ID;
                db.OrderDetails.Add(d);
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var order = db.OrderProes
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails.Select(d => d.Product))
                .FirstOrDefault(o => o.ID == id);

            if (order == null) return HttpNotFound();
            return View(order);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var order = db.OrderProes
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails.Select(d => d.Product))
                .FirstOrDefault(o => o.ID == id);

            if (order == null) return HttpNotFound();

            if (!order.TotalPrice.HasValue && order.OrderDetails != null && order.OrderDetails.Any())
            {
                order.TotalPrice = order.OrderDetails.Sum(d =>
                    (decimal)(d.Quantity ?? 0) * (decimal)(d.UnitPrice ?? 0));
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OrderPro order)
        {

            var dbOrder = db.OrderProes.Find(order.ID);
            if (dbOrder == null) return HttpNotFound();

            dbOrder.AddressDeliverry = order.AddressDeliverry;
            dbOrder.Status = order.Status;

            dbOrder.TotalPrice = order.TotalPrice;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var order = db.OrderProes
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails.Select(d => d.Product))
                .FirstOrDefault(o => o.ID == id);

            if (order == null) return HttpNotFound();

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var order = db.OrderProes
                .FirstOrDefault(o => o.ID == id);

            if (order == null) return HttpNotFound();

            order.Status = 4; // Đã hủy
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        private void BuildOrderCreateLists(OrderFormViewModel vm)
        {
            var products = db.Products.Where(p => !p.IsDeleted).ToList();

            vm.CustomerList = new SelectList(db.Customers.ToList(), "IDCus", "NameCus");
            vm.ProductList = new SelectList(products, "ProductID", "NamePro");

            ViewBag.ProductPrices = products
                .ToDictionary(p => p.ProductID, p => (decimal)(p.Price ?? 0));

            ViewBag.ColorList = new SelectList(db.Colors, "ColorID", "ColorName");
            ViewBag.SizeList = new SelectList(db.Sizes, "SizeID", "SizeName");

            var customers = db.Customers.Select(c => new
            {
                c.IDCus,
                c.NameCus,
                c.PhoneCus,
                c.EmailCus
            }).ToList();

            ViewBag.CustomerJson = JsonConvert.SerializeObject(customers);
        }

    }
}
