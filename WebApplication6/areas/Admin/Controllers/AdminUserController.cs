using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplication6.Models;

namespace WebApplication6.Areas.Admin.Controllers
{
    public class AdminUserController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        // CHẶN: nếu chưa login admin thì về trang Login
        private bool IsLoggedIn()
        {
            return Session["AdminUser"] != null;
        }

        private ActionResult RequireLogin()
        {
            return RedirectToAction("Login", "Auth", new { area = "Admin" });
        }

        public ActionResult Index()
        {
            if (!IsLoggedIn()) return RequireLogin();

            var list = db.AdminUsers.ToList();
            return View(list);
        }

        public ActionResult Details(int? id)
        {
            if (!IsLoggedIn()) return RequireLogin();

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            AdminUser user = db.AdminUsers.Find(id);
            if (user == null) return HttpNotFound();

            return View(user);
        }

        public ActionResult Create()
        {
            if (!IsLoggedIn()) return RequireLogin();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserName,RoleUser,PasswordUser")] AdminUser user)
        {
            if (!IsLoggedIn()) return RequireLogin();

            if (ModelState.IsValid)
            {
                db.AdminUsers.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult Edit(int? id)
        {
            if (!IsLoggedIn()) return RequireLogin();

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            AdminUser user = db.AdminUsers.Find(id);
            if (user == null) return HttpNotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserName,RoleUser,PasswordUser")] AdminUser user)
        {
            if (!IsLoggedIn()) return RequireLogin();

            if (ModelState.IsValid)
            {
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult Delete(int? id)
        {
            if (!IsLoggedIn()) return RequireLogin();

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            AdminUser user = db.AdminUsers.Find(id);
            if (user == null) return HttpNotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!IsLoggedIn()) return RequireLogin();

            AdminUser user = db.AdminUsers.Find(id);
            db.AdminUsers.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
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
