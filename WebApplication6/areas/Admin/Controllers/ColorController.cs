using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplication6.Models;

namespace WebApplication6.Areas.Admin.Controllers
{
    public class ColorController : Controller
    {
        private readonly DBSportStoreEntities db = new DBSportStoreEntities();

        public ActionResult Index(string search)
        {
            var query = db.Colors.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(c =>
                    c.ColorName.Contains(search) ||
                    c.ColorCode.Contains(search));
            }

            var list = query
                .OrderBy(c => c.IsDeleted)
                .ThenBy(c => c.ColorName)
                .ToList();

            ViewBag.Search = search;
            return View(list);
        }

        public ActionResult Create()
        {
            var model = new Color
            {
                IsDeleted = false
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Color color)
        {
            // validate trùng tên / mã màu
            if (db.Colors.Any(c => !c.IsDeleted && c.ColorName == color.ColorName))
            {
                ModelState.AddModelError("ColorName",
                    "Tên màu đã tồn tại, vui lòng chọn tên khác.");
            }
            if (!string.IsNullOrWhiteSpace(color.ColorCode) &&
                db.Colors.Any(c => !c.IsDeleted && c.ColorCode == color.ColorCode))
            {
                ModelState.AddModelError("ColorCode",
                    "Mã màu đã tồn tại, vui lòng chọn mã khác.");
            }

            if (!ModelState.IsValid)
                return View(color);

            color.IsDeleted = false;
            db.Colors.Add(color);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var color = db.Colors.Find(id);
            if (color == null) return HttpNotFound();

            return View(color);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Color color)
        {
            if (db.Colors.Any(c =>
                    c.ColorID != color.ColorID &&
                    !c.IsDeleted &&
                    c.ColorName == color.ColorName))
            {
                ModelState.AddModelError("ColorName",
                    "Tên màu đã tồn tại, vui lòng chọn tên khác.");
            }

            if (!string.IsNullOrWhiteSpace(color.ColorCode) &&
                db.Colors.Any(c =>
                    c.ColorID != color.ColorID &&
                    !c.IsDeleted &&
                    c.ColorCode == color.ColorCode))
            {
                ModelState.AddModelError("ColorCode",
                    "Mã màu đã tồn tại, vui lòng chọn mã khác.");
            }

            if (!ModelState.IsValid)
                return View(color);

            var dbColor = db.Colors.Find(color.ColorID);
            if (dbColor == null) return HttpNotFound();

            dbColor.ColorName = color.ColorName;
            dbColor.ColorCode = color.ColorCode;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var color = db.Colors.Find(id);
            if (color == null) return HttpNotFound();

            return View(color);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var color = db.Colors.Find(id);
            if (color == null) return HttpNotFound();

            color.IsDeleted = !color.IsDeleted;
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
