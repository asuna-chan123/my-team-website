using System.Linq;
using System.Web.Mvc;
using WebApplication6.Models;

namespace WebApplication6.Areas.Admin.Controllers
{
    public class AuthController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return View();
            }

            var user = db.AdminUsers
                         .FirstOrDefault(u => u.UserName == username && u.PasswordUser == password);

            if (user == null)
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu.";
                return View();
            }

            Session["AdminUser"] = user;
            Session["AdminUserName"] = user.UserName;
            Session["AdminRole"] = user.RoleUser;

            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        public ActionResult Logout()
        {
            Session["AdminUser"] = null;
            Session["AdminUserName"] = null;
            Session["AdminRole"] = null;

            return RedirectToAction("Login");
        }
    }
}
