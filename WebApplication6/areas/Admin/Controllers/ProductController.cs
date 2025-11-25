using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication6.Models;

namespace WebApplication6.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        // GET: Admin/Product
        public ActionResult Index()
        {
            var list = db.Products
                         .Include(p => p.Category) // load luôn Category
                         .ToList();

            return View(list);  // -> Areas/Admin/Views/Product/Index.cshtml
        }

        // GET: Admin/Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var product = db.Products
                            .Include(p => p.Category)
                            .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
                return HttpNotFound();

            return View(product);
        }

        // GET: Admin/Product/Create
        public ActionResult Create()
        {
            ViewBag.CateID = new SelectList(db.Categories, "IDCate", "NameCate");
            return View();
        }

        // POST: Admin/Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase ImageUpload)
        {
            // load lại dropdown nếu có lỗi
            ViewBag.CateID = new SelectList(db.Categories, "IDCate", "NameCate", product.CateID);

            if (!ModelState.IsValid)
            {
                // Trả lại view nếu validate model bị lỗi
                return View(product);
            }

            db.Products.Add(product);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Admin/Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var product = db.Products.Find(id);
            if (product == null)
                return HttpNotFound();

            ViewBag.CateID = new SelectList(db.Categories, "IDCate", "NameCate", product.CateID);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CateID = new SelectList(db.Categories, "IDCate", "NameCate", product.CateID);
                return View(product);
            }

            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");

        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var product = db.Products
                            .Include(p => p.Category)
                            .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
                return HttpNotFound();

            return View(product);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
                return HttpNotFound();

            // XÓA MỀM: chỉ đánh dấu
            product.IsDeleted = true;
            db.Entry(product).State = EntityState.Modified;
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
