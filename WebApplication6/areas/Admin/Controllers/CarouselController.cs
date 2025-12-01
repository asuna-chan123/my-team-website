using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplication6.Models;

namespace WebApplication6.Areas.Admin.Controllers
{
    public class CarouselController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        public ActionResult Index()
        {
            // có thể đếm luôn số item trong từng carousel
            var list = db.Carousels
                         .Include(c => c.CarouselItems)
                         .OrderBy(c => c.Code)
                         .ToList();

            return View(list);
        }

        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var carousel = db.Carousels
                             .Include(c => c.CarouselItems)
                             .FirstOrDefault(c => c.CarouselID == id);

            if (carousel == null) return HttpNotFound();

            return View(carousel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var carousel = db.Carousels.Find(id);
            if (carousel == null) return HttpNotFound();

            return View(carousel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CarouselID,Code,Name,IsActive")] Carousel carousel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(carousel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(carousel);
        }

    }
}
