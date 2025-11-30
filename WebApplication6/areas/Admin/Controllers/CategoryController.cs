using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplication6.Models;   // <-- namespace models của bạn

namespace WebApplication6.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        // GET: Admin/Category
        public ActionResult Index(string search)
        {
            // Lấy query gốc
            var query = db.Categories.AsQueryable();

            // Nếu có nhập từ khóa thì lọc theo tên
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.NameCate.Contains(search));
            }

            var list = query.ToList();

            // Để hiển thị lại trên ô search
            ViewBag.Search = search;

            return View(list);   // -> Areas/Admin/Views/Category/Index.cshtml
        }

        // GET: Admin/Category/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (category == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // chuẩn hóa tên
            var name = (category.NameCate ?? "").Trim();

            // check trùng tên (không phân biệt hoa thường, kể cả cái đã IsDeleted)
            bool exists = db.Categories.Any(c =>
                c.NameCate.ToLower() == name.ToLower()
            );

            if (exists)
            {
                ModelState.AddModelError("NameCate",
                    "Tên danh mục này đã tồn tại, vui lòng chọn tên khác.");
            }

            if (!ModelState.IsValid)
            {
                // trả lại view kèm lỗi
                return View(category);
            }

            category.NameCate = name;
            category.IsDeleted = false; // đảm bảo danh mục mới là đang active
            db.Categories.Add(category);
            db.SaveChanges();

            return RedirectToAction("Index");
        }



        // GET: Admin/Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var category = db.Categories.Find(id);
            if (category == null || category.IsDeleted)
                return HttpNotFound();

            return View(category);
        }

        // POST: Admin/Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            if (category == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var name = (category.NameCate ?? "").Trim();

            // check trùng với *danh mục khác*
            bool nameExists = db.Categories.Any(c =>
                c.IDCate != category.IDCate &&       // bỏ qua chính nó
                c.NameCate.ToLower() == name.ToLower()
            );

            if (nameExists)
            {
                ModelState.AddModelError("NameCate",
                    "Tên danh mục này đã tồn tại, vui lòng chọn tên khác.");
            }

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            var dbCate = db.Categories.Find(category.IDCate);
            if (dbCate == null || dbCate.IsDeleted)
                return HttpNotFound();

            dbCate.NameCate = name;
            db.SaveChanges();

            return RedirectToAction("Index");
        }



        // GET: Admin/Category/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var category = db.Categories.Find(id);
            if (category == null || category.IsDeleted)
                return HttpNotFound();

            return View(category);
        }


        // GET: Admin/Category/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var category = db.Categories.Find(id);
            if (category == null)
                return HttpNotFound();

            return View(category);
        }
        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
                return HttpNotFound();

            // XÓA MỀM
            category.IsDeleted = true;
            db.Entry(category).State = EntityState.Modified;
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
