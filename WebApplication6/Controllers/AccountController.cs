using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication6.Models;
using WebApplication6.ViewModels;

namespace WebApplication6.Controllers
{
    public class AccountController : Controller
    {
        DBSportStoreEntities db = new DBSportStoreEntities();

        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var cus = db.Customers.SingleOrDefault(x =>
                    x.UserName == model.Username && x.Password == model.Password);

                if (cus != null)
                {
                    Session["Username"] = cus.UserName;
                    Session["CustomerID"] = cus.IDCus;

                    FormsAuthentication.SetAuthCookie(cus.UserName, false);

                    return RedirectToAction("ProductList", "Products");
                }

                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng!");
            }

            return View(model);
        }

        // GET: Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var check = db.Customers.SingleOrDefault(x => x.UserName == model.Username);
                if (check != null)
                {
                    ModelState.AddModelError("", "Tên đăng nhập đã tồn tại!");
                    return View(model);
                }

                Customer cus = new Customer()
                {
                    UserName = model.Username,
                    Password = model.Password,
                    NameCus = model.FullName,
                    EmailCus = model.Email,
                    PhoneCus = model.Phone
                };

                db.Customers.Add(cus);
                db.SaveChanges();

                TempData["Success"] = "Đăng ký thành công!";
                return RedirectToAction("Login");
            }

            return View(model);
        }
    }
}
