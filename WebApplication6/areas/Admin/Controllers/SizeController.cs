using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplication6.Models;

namespace WebApplication6.Areas.Admin.Controllers
{
    public class SizeController : Controller
    {
        private readonly DBSportStoreEntities db = new DBSportStoreEntities();

        public ActionResult Index(string search)
        {
            var query = db.Sizes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(s => s.SizeName.Contains(search));
            }

            var list = query
                .OrderBy(s => s.IsDeleted)
                .ThenBy(s => s.SizeID)
                .ToList();

            ViewBag.Search = search;
            return View(list);
        }

        public ActionResult Create()
        {
            var model = new Size { IsDeleted = false };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Size size)
        {
            var name = (size.SizeName ?? "").Trim();

            // kiểm tra trùng (kể cả size đã IsDeleted = true)
            bool exists = db.Sizes.Any(s => s.SizeName == name);

            if (exists)
            {
                ModelState.AddModelError("SizeName",
                    "Tên size đã tồn tại, vui lòng chọn tên khác.");
            }

            if (!ModelState.IsValid)
                return View(size);

            size.SizeName = name;
            size.IsDeleted = false;
            db.Sizes.Add(size);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var size = db.Sizes.Find(id);
            if (size == null) return HttpNotFound();

            return View(size);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Size size)
        {
            var name = (size.SizeName ?? "").Trim();

            bool exists = db.Sizes.Any(s =>
                s.SizeID != size.SizeID &&
                s.SizeName == name);

            if (exists)
            {
                ModelState.AddModelError("SizeName",
                    "Tên size đã tồn tại, vui lòng chọn tên khác.");
            }

            if (!ModelState.IsValid)
                return View(size);

            var dbSize = db.Sizes.Find(size.SizeID);
            if (dbSize == null) return HttpNotFound();

            dbSize.SizeName = name;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var size = db.Sizes.Find(id);
            if (size == null) return HttpNotFound();

            return View(size);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var size = db.Sizes.Find(id);
            if (size == null) return HttpNotFound();

            size.IsDeleted = !size.IsDeleted; // toggle
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
