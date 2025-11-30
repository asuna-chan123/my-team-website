using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplication6.Models;
using WebApplication6.ViewModels;

namespace WebApplication6.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        // GET: Admin/Product

        [HttpGet]
        public ActionResult Index(string search)
        {
            // ✅ Nếu bạn muốn HIỂN THỊ CẢ sản phẩm đã xóa (để gạch ngang)
            // thì bỏ Where(p => !p.IsDeleted) đi
            var products = db.Products
                             .Include(p => p.Category);

            // Nếu có từ khoá search thì lọc theo NamePro
            if (!string.IsNullOrWhiteSpace(search))
            {
                string keyword = search.Trim();
                products = products.Where(p => p.NamePro.Contains(keyword));
            }

            products = products.OrderBy(p => p.ProductID);

            ViewBag.Search = search; // để đổ lại vào ô input

            return View(products.ToList());
        }

        // GET: Admin/Product/Details/5
        public ActionResult Details(int id)
        {
            var product = db.Products
                            .Include(p => p.Category)
                            .FirstOrDefault(p => p.ProductID == id);

            if (product == null) return HttpNotFound();

            return View(product);
        }

        // ========= CREATE GET =========
        public ActionResult Create()
        {
            var vm = new ProductFormViewModel();
            vm.Product = new Product();

            for (int i = 0; i < 10; i++)
            {
                var v = new ProductVariantViewModel();
                v.SizeStocks.Add(new VariantSizeStockViewModel()); // 1 cặp size/stock trống
                v.ImageUrls.Add("");                               // 1 ô url trống
                vm.Variants.Add(v);
            }

            var categories = db.Categories.Where(c => !c.IsDeleted).ToList();
            ViewBag.CategoryList = new SelectList(categories, "IDCate", "NameCate");

            var colors = db.Colors.ToList();
            var sizes = db.Sizes.ToList();

            vm.ColorList = new SelectList(colors, "ColorID", "ColorName");
            vm.SizeList = new SelectList(sizes, "SizeID", "SizeName");

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = db.Categories.Where(c => !c.IsDeleted).ToList();
                ViewBag.CategoryList = new SelectList(categories, "IDCate", "NameCate", model.Product.CateID);

                var colors = db.Colors.ToList();
                model.ColorList = new SelectList(colors, "ColorID", "ColorName");

                var sizes = db.Sizes.ToList();
                model.SizeList = new SelectList(sizes, "SizeID", "SizeName");

                return View(model);
            }

            var product = model.Product;
            product.IsDeleted = false;
            db.Products.Add(product);
            db.SaveChanges();

            string firstProductImage = null;

            foreach (var v in model.Variants)
            {
                var validUrls = (v.ImageUrls ?? new List<string>())
                    .Where(u => !string.IsNullOrWhiteSpace(u))
                    .ToList();

                // insert ProductImage theo màu
                int sortOrder = 1;
                foreach (var url in validUrls)
                {
                    db.ProductImages.Add(new ProductImage
                    {
                        ProductID = product.ProductID,
                        ColorID = v.ColorID,
                        ImageUrl = url,
                        SortOrder = sortOrder++
                    });
                }

                if (firstProductImage == null && validUrls.Any())
                {
                    firstProductImage = validUrls.First();
                }

                // mỗi SizeStocks => 1 ProductVariant
                foreach (var ss in v.SizeStocks ?? new List<VariantSizeStockViewModel>())
                {
                    if (!ss.SizeID.HasValue || ss.SizeID.Value == 0) continue;
                    // nếu muốn: bỏ qua dòng hoàn toàn rỗng
                    if (ss.StockQty <= 0 && !v.Price.HasValue) continue;

                    var variant = new ProductVariant
                    {
                        ProductID = product.ProductID,
                        ColorID = v.ColorID,
                        SizeID = ss.SizeID,
                        Price = v.Price ?? 0,
                        StockQty = ss.StockQty,
                        ImagePro = validUrls.FirstOrDefault(), // ảnh chính theo màu
                        IsDeleted = false
                    };

                    db.ProductVariants.Add(variant);
                }
            }

            if (firstProductImage != null)
            {
                product.ImagePro = firstProductImage;
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // ========= END CREATE =========

        // GET: Admin/Product/Edit/5
        // Admin/ProductController.cs
        public ActionResult Edit(int id)
        {
            var product = db.Products
                .Include(p => p.ProductVariants)
                .Include(p => p.ProductImages)
                .SingleOrDefault(p => p.ProductID == id);

            if (product == null) return HttpNotFound();

            var vm = new ProductFormViewModel
            {
                Product = product
            };

            // dropdown
            var categories = db.Categories.Where(c => !c.IsDeleted).ToList();
            ViewBag.CategoryList = new SelectList(categories, "IDCate", "NameCate", product.CateID);

            var colors = db.Colors.ToList();
            var sizes = db.Sizes.ToList();
            vm.ColorList = new SelectList(colors, "ColorID", "ColorName");
            vm.SizeList = new SelectList(sizes, "SizeID", "SizeName");

            // gom các ProductVariant theo ColorID + Price
            var groups = product.ProductVariants
                .Where(v => !v.IsDeleted)
                .GroupBy(v => new { v.ColorID, v.Price });

            foreach (var g in groups)
            {
                var colorVm = new ProductVariantViewModel
                {
                    ProductID = product.ProductID,
                    ColorID = g.Key.ColorID,
                    Price = g.Key.Price,
                    SizeStocks = g.Select(v => new VariantSizeStockViewModel
                    {
                        SizeID = v.SizeID,
                        StockQty = v.StockQty
                    }).ToList(),
                    ImageUrls = db.ProductImages
                        .Where(pi => pi.ProductID == product.ProductID && pi.ColorID == g.Key.ColorID)
                        .OrderBy(pi => pi.SortOrder)
                        .Select(pi => pi.ImageUrl)
                        .ToList()
                };

                if (!colorVm.SizeStocks.Any())
                    colorVm.SizeStocks.Add(new VariantSizeStockViewModel());

                if (!colorVm.ImageUrls.Any())
                    colorVm.ImageUrls.Add("");

                vm.Variants.Add(colorVm);
            }

            // thêm hàng trống cho màu mới
            int extra = 10 - vm.Variants.Count;
            for (int i = 0; i < extra; i++)
            {
                var v = new ProductVariantViewModel();
                v.SizeStocks.Add(new VariantSizeStockViewModel());
                v.ImageUrls.Add("");
                vm.Variants.Add(v);
            }

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = db.Categories.Where(c => !c.IsDeleted).ToList();
                ViewBag.CategoryList = new SelectList(categories, "IDCate", "NameCate", model.Product.CateID);

                var colors = db.Colors.ToList();
                var sizes = db.Sizes.ToList();
                model.ColorList = new SelectList(colors, "ColorID", "ColorName");
                model.SizeList = new SelectList(sizes, "SizeID", "SizeName");

                return View(model);
            }

            var productDb = db.Products
                .Include(p => p.ProductVariants)
                .Include(p => p.ProductImages)
                .SingleOrDefault(p => p.ProductID == model.Product.ProductID);

            if (productDb == null) return HttpNotFound();

            // update thông tin chung
            db.Entry(productDb).CurrentValues.SetValues(model.Product);

            // xoá hết biến thể & ảnh cũ
            db.ProductVariants.RemoveRange(productDb.ProductVariants.ToList());
            db.ProductImages.RemoveRange(productDb.ProductImages.ToList());

            string firstProductImage = null;

            foreach (var v in model.Variants)
            {
                if (!v.ColorID.HasValue || v.ColorID.Value == 0) continue;

                var validUrls = (v.ImageUrls ?? new List<string>())
                    .Where(u => !string.IsNullOrWhiteSpace(u))
                    .ToList();

                // ProductImage theo màu
                int sortOrder = 1;
                foreach (var url in validUrls)
                {
                    db.ProductImages.Add(new ProductImage
                    {
                        ProductID = productDb.ProductID,
                        ColorID = v.ColorID,
                        ImageUrl = url,
                        SortOrder = sortOrder++
                    });
                }

                if (firstProductImage == null && validUrls.Any())
                    firstProductImage = validUrls.First();

                // mỗi SizeStocks => 1 ProductVariant
                foreach (var ss in v.SizeStocks ?? new List<VariantSizeStockViewModel>())
                {
                    if (!ss.SizeID.HasValue || ss.SizeID.Value == 0) continue;

                    bool emptyRow = (v.Price ?? 0) <= 0 && ss.StockQty <= 0;
                    if (emptyRow) continue;

                    db.ProductVariants.Add(new ProductVariant
                    {
                        ProductID = productDb.ProductID,
                        ColorID = v.ColorID,
                        SizeID = ss.SizeID,
                        Price = v.Price ?? 0,
                        StockQty = ss.StockQty,
                        ImagePro = validUrls.FirstOrDefault(),
                        IsDeleted = false
                    });
                }
            }

            if (!string.IsNullOrEmpty(firstProductImage))
                productDb.ImagePro = firstProductImage;

            db.SaveChanges();
            return RedirectToAction("Index");
        }



        // GET: Admin/Product/Delete/5
        public ActionResult Delete(int id)
        {
            var product = db.Products
                            .Include(p => p.Category)
                            .FirstOrDefault(p => p.ProductID == id);

            if (product == null) return HttpNotFound();

            return View(product);
        }

        // POST: Admin/Product/Delete/5 (xóa mềm)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int ProductID)
        {
            var product = db.Products.Find(ProductID);
            if (product == null) return HttpNotFound();

            product.IsDeleted = true;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
