using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using WebApplication6.Models;
using WebApplication6.ViewModels;

namespace WebApplication6.Controllers
{
    public class ProductsController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        // ================================
        // 📌 1. Trang danh sách sản phẩm
        // ================================
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category);
            return View(products.ToList()); // ✅ Trả về List<Product>
        }

        // ================================
        // 📌 2. Trang chi tiết sản phẩm
        // ================================
        public ActionResult Details(int id)
        {
            var product = db.Products.Include(p => p.Category)
                                     .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            var vm = new ProductDetailViewModel
            {
                ProductID = product.ProductID,
                NamePro = product.NamePro,
                DecriptionPro = product.DecriptionPro,
                Price = product.Price,
                ImagePro = product.ImagePro,
                BannerVideoUrl = "https://player.vimeo.com/video/1009385367",
                FeatureImageUrl = "https://cdn.example.com/yeezy1.jpg",
                CategoryName = product.Category != null ? product.Category.NameCate : "Chưa có loại"
            };

            return View(vm); // ✅ ViewModel riêng cho trang chi tiết
        }

        // ================================
        // 📌 3. Tạo mới sản phẩm
        // ================================
        public ActionResult Create()
        {
            ViewBag.CateID = new SelectList(db.Categories, "IDCate", "NameCate");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,NamePro,DecriptionPro,CateID,Price,ImagePro")] Product product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = System.IO.Path.GetFileName(ImageFile.FileName);
                    string path = Server.MapPath("~/Content/images/" + fileName);
                    ImageFile.SaveAs(path);
                    product.ImagePro = fileName;
                }

                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CateID = new SelectList(db.Categories, "IDCate", "NameCate", product.CateID);
            return View(product);
        }

        // ================================
        // 📌 4. Chỉnh sửa sản phẩm
        // ================================
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Product product = db.Products.Find(id);
            if (product == null)
                return HttpNotFound();

            ViewBag.CateID = new SelectList(db.Categories, "IDCate", "NameCate", product.CateID);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,NamePro,DecriptionPro,CateID,Price,ImagePro")] Product product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                var oldProduct = db.Products.AsNoTracking().FirstOrDefault(p => p.ProductID == product.ProductID);

                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = System.IO.Path.GetFileName(ImageFile.FileName);
                    string path = Server.MapPath("~/Content/images/" + fileName);
                    ImageFile.SaveAs(path);
                    product.ImagePro = fileName;
                }
                else
                {
                    product.ImagePro = oldProduct.ImagePro;
                }

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CateID = new SelectList(db.Categories, "IDCate", "NameCate", product.CateID);
            return View(product);
        }

        // ================================
        // 📌 5. Xóa sản phẩm
        // ================================
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Product product = db.Products.Find(id);
            if (product == null)
                return HttpNotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // ================================
        // 📌 6. Phân trang + lọc sản phẩm
        // ================================
        public ActionResult ProductList(int? category, int? page, string SearchString, double min = double.MinValue, double max = double.MaxValue)
        {
            var products = db.Products.Include(p => p.Category);

            if (category != null)
                products = products.Where(p => p.CateID == category);

            if (!String.IsNullOrEmpty(SearchString))
                products = products.Where(p => p.NamePro.Contains(SearchString.Trim()));

            if (min >= 0 && max > 0)
                products = products.Where(p => (double)p.Price >= min && (double)p.Price <= max);

            int pageSize = 4;
            int pageNumber = (page ?? 1);

            return View(products.OrderBy(p => p.ProductID).ToPagedList(pageNumber, pageSize));
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
