using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplication6.Models;

namespace WebApplication6.Areas.Admin.Controllers
{
    public class CustomerController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        // GET: Admin/Customer
        public ActionResult Index(string search)
        {
            var customers = db.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                customers = customers.Where(c =>
                    c.NameCus.Contains(search) ||
                    c.PhoneCus.Contains(search) ||
                    c.EmailCus.Contains(search) ||
                    c.UserName.Contains(search));
            }

            ViewBag.Search = search;
            return View(customers.OrderBy(c => c.IDCus).ToList());
        }

        // GET: Admin/Customer/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var customer = db.Customers.Find(id);
            if (customer == null) return HttpNotFound();

            return View(customer);
        }

        // GET: Admin/Customer/Create
        // GET: Admin/Customer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Customer customer)
        {
            // ----- VALIDATE CƠ BẢN -----
            if (string.IsNullOrWhiteSpace(customer.NameCus))
                ModelState.AddModelError("NameCus", "Vui lòng nhập tên khách hàng.");

            if (string.IsNullOrWhiteSpace(customer.PhoneCus))
                ModelState.AddModelError("PhoneCus", "Vui lòng nhập số điện thoại.");

            if (!string.IsNullOrEmpty(customer.EmailCus) &&
                !customer.EmailCus.Contains("@"))
            {
                ModelState.AddModelError("EmailCus", "Email không hợp lệ.");
            }

            // ----- CHECK TRÙNG DỮ LIỆU -----
            // SĐT
            if (!string.IsNullOrWhiteSpace(customer.PhoneCus))
            {
                bool phoneExists = db.Customers
                                     .Any(c => c.PhoneCus == customer.PhoneCus);
                if (phoneExists)
                    ModelState.AddModelError("PhoneCus", "Số điện thoại này đã tồn tại.");
            }

            // Email (nếu có nhập)
            if (!string.IsNullOrWhiteSpace(customer.EmailCus))
            {
                bool emailExists = db.Customers
                                     .Any(c => c.EmailCus == customer.EmailCus);
                if (emailExists)
                    ModelState.AddModelError("EmailCus", "Email này đã được sử dụng.");
            }

            // UserName (nếu có nhập)
            if (!string.IsNullOrWhiteSpace(customer.UserName))
            {
                bool userExists = db.Customers
                                    .Any(c => c.UserName == customer.UserName);
                if (userExists)
                    ModelState.AddModelError("UserName", "Tên tài khoản đã tồn tại.");
            }

            // Nếu có lỗi => quay lại view
            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            // ----- LƯU KHÁCH HÀNG -----
            customer.IsLocked = customer.IsLocked; // giữ nguyên checkbox
            db.Customers.Add(customer);
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        // GET: Admin/Customer/Edit/5
        // GET: Admin/Customer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var customer = db.Customers.Find(id);
            if (customer == null) return HttpNotFound();

            return View(customer);
        }

        // POST: Admin/Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Customer customer)
        {
            // --------- VALIDATE CƠ BẢN ---------
            if (string.IsNullOrWhiteSpace(customer.NameCus))
                ModelState.AddModelError("NameCus", "Vui lòng nhập tên khách hàng.");

            if (string.IsNullOrWhiteSpace(customer.PhoneCus))
                ModelState.AddModelError("PhoneCus", "Vui lòng nhập số điện thoại.");

            if (!string.IsNullOrEmpty(customer.EmailCus) &&
                !customer.EmailCus.Contains("@"))
            {
                ModelState.AddModelError("EmailCus", "Email không hợp lệ.");
            }

            // --------- CHECK TRÙNG (BỎ QUA CHÍNH NÓ) ----------
            // SĐT
            if (!string.IsNullOrWhiteSpace(customer.PhoneCus))
            {
                bool phoneExists = db.Customers
                    .Any(c => c.IDCus != customer.IDCus &&
                              c.PhoneCus == customer.PhoneCus);
                if (phoneExists)
                    ModelState.AddModelError("PhoneCus", "Số điện thoại này đã tồn tại.");
            }

            // Email
            if (!string.IsNullOrWhiteSpace(customer.EmailCus))
            {
                bool emailExists = db.Customers
                    .Any(c => c.IDCus != customer.IDCus &&
                              c.EmailCus == customer.EmailCus);
                if (emailExists)
                    ModelState.AddModelError("EmailCus", "Email này đã được sử dụng.");
            }

            // UserName
            if (!string.IsNullOrWhiteSpace(customer.UserName))
            {
                bool userExists = db.Customers
                    .Any(c => c.IDCus != customer.IDCus &&
                              c.UserName == customer.UserName);
                if (userExists)
                    ModelState.AddModelError("UserName", "Tên tài khoản đã tồn tại.");
            }

            if (!ModelState.IsValid)
            {
                // có lỗi -> trả lại view cùng ModelState
                return View(customer);
            }

            // --------- UPDATE DB ----------
            var dbCustomer = db.Customers.Find(customer.IDCus);
            if (dbCustomer == null) return HttpNotFound();

            dbCustomer.NameCus = customer.NameCus;
            dbCustomer.PhoneCus = customer.PhoneCus;
            dbCustomer.EmailCus = customer.EmailCus;
            dbCustomer.UserName = customer.UserName;
            dbCustomer.Password = customer.Password;
            dbCustomer.Gender = customer.Gender;
            dbCustomer.Birthday = customer.Birthday;
            dbCustomer.IsLocked = customer.IsLocked;

            db.SaveChanges();

            return RedirectToAction("Index");
        }


        // GET: Admin/Customer/Delete/5  (khóa / mở khóa)
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var customer = db.Customers.Find(id);
            if (customer == null) return HttpNotFound();

            return View(customer);
        }

        // POST: Admin/Customer/Delete/5
        // thay vì xóa cứng sẽ toggle IsLocked
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var customer = db.Customers.Find(id);
            if (customer == null) return HttpNotFound();

            customer.IsLocked = !customer.IsLocked;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
