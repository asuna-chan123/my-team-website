using System.Linq;
using System.Web.Mvc;
using WebApplication6.Models;

namespace WebApplication6.Areas.Admin.Controllers
{
    public class AuthController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        // GET: Admin/Auth/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Admin/Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return View();
            }

            // kiểm tra trong bảng AdminUser
            var user = db.AdminUsers
                         .FirstOrDefault(u => u.UserName == username && u.PasswordUser == password);

            if (user == null)
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu.";
                return View();
            }

            // Lưu vào session
            Session["AdminUser"] = user;
            Session["AdminUserName"] = user.UserName;
            Session["AdminRole"] = user.RoleUser;

            // chuyển tới trang admin home
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        // GET: Admin/Auth/Logout
        public ActionResult Logout()
        {
            Session["AdminUser"] = null;
            Session["AdminUserName"] = null;
            Session["AdminRole"] = null;

            return RedirectToAction("Login");
        }
    }
}
