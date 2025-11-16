using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using WebApplication6.Models;
using WebApplication6.ViewModels;
using System.Text.RegularExpressions;

namespace WebApplication6.Controllers
{
    public class ProductsController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        // ================================
        // 📌 1. Trang danh sách sản phẩm
        // ================================
        public ActionResult Index(string gender, string category, string priceSort)
        {
            // 1) Lấy dữ liệu ban đầu (materialize để tránh lỗi LINQ-to-Entities với property không mapped)
            var q = db.Products.Include(p => p.Category);
            var list = q.ToList();
            // tạm debug (đặt ngay sau var q = db.Products.Include(...); hoặc sau ToList())
            System.Diagnostics.Debug.WriteLine($"param gender={gender}");
            System.Diagnostics.Debug.WriteLine("Distinct genders in DB: " + string.Join(", ", db.Products.Select(p => p.Gender).Distinct().Where(x => x != null).ToList()));


            // 2) Chuẩn hoá mappedGender nếu cần (map "male"/"female" -> "Nam"/"Nữ")
            string mappedGender = null;
            if (!string.IsNullOrEmpty(gender))
            {
                if (gender.Equals("male", StringComparison.OrdinalIgnoreCase)) mappedGender = "Nam";
                else if (gender.Equals("female", StringComparison.OrdinalIgnoreCase)) mappedGender = "Nữ";
                else if (gender.Equals("unisex", StringComparison.OrdinalIgnoreCase)) mappedGender = "Unisex";
                else mappedGender = gender;
            }

            System.Diagnostics.Debug.WriteLine($"gender param={gender} mapped={mappedGender} beforeCount={list.Count()}");

            // Lọc Unisex: khi user chọn "Nam" thì lấy Nam + Unisex; chọn "Nữ" lấy Nữ + Unisex
            if (!string.IsNullOrEmpty(mappedGender))
            {
                if (mappedGender.Equals("Nam", StringComparison.OrdinalIgnoreCase))
                    q = q.Where(p => p.Gender == "Nam" || p.Gender == "Unisex");
                else if (mappedGender.Equals("Nữ", StringComparison.OrdinalIgnoreCase))
                    q = q.Where(p => p.Gender == "Nữ" || p.Gender == "Unisex");
                else if (mappedGender.Equals("Unisex", StringComparison.OrdinalIgnoreCase))
                    q = q.Where(p => p.Gender == "Unisex");
                else
                    q = q.Where(p => p.Gender == mappedGender);
            }

            if (!string.IsNullOrEmpty(category))
            {
                int catId;

                if (int.TryParse(category, out catId))
                {
                    // category là IDCate → lọc theo CateID + vẫn lấy Unisex
                    list = list.Where(p =>
                           (p.CateID.HasValue && p.CateID.Value == catId)
                           || p.Gender == "Unisex"
                    ).ToList();
                }
                else
                {
                    // category là tên → lọc theo NameCate và vẫn lấy Unisex
                    var cLower = category.ToLower();
                    list = list.Where(p =>
                           (p.Category != null &&
                             p.Category.NameCate != null &&
                             p.Category.NameCate.ToLower() == cLower)
                           || p.Gender == "Unisex"
                    ).ToList();
                }
            }


            // 4) Sắp xếp giá (thực hiện trên list)
            if (!string.IsNullOrEmpty(priceSort))
            {
                if (priceSort == "asc")
                    list = list.OrderBy(p => p.Price).ToList();
                else if (priceSort == "desc")
                    list = list.OrderByDescending(p => p.Price).ToList();
            }

            // 5) Tạo ViewModel và trả về view
            var vm = new ProductListViewModel
            {
                Products = list,
                Gender = gender,
                Category = category,
                PriceSort = priceSort,
                AllCategories = db.Categories.ToList()
            };

            return View(vm);
        }


        // ================================
        // 📌 2. Trang chi tiết sản phẩm
        // ================================
        public ActionResult Details(int id)
        {
            var product = db.Products.Include(p => p.Category)
                                     .FirstOrDefault(p => p.ProductID == id);

            if (product == null) return HttpNotFound();

            // --- build grouped images by color from ~/Content/images/ ---
            var imagesByColor = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            try
            {
                var imagesFolder = Server.MapPath("~/Content/images/");

                if (Directory.Exists(imagesFolder))
                {
                    // list all filenames in folder (just file names)
                    var allFiles = Directory.GetFiles(imagesFolder)
                                            .Select(Path.GetFileName)
                                            .Where(n => !string.IsNullOrEmpty(n))
                                            .ToList();

                    // derive a basePrefix from product.ImagePro or product.NamePro
                    string basePrefix = null;
                    if (!string.IsNullOrEmpty(product.ImagePro))
                    {
                        var nameNoExt = Path.GetFileNameWithoutExtension(product.ImagePro);
                        // take token before first underscore, so "OwnTheRun" from "OwnTheRun_Blue_01"
                        basePrefix = nameNoExt.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? nameNoExt;
                    }
                    else if (!string.IsNullOrEmpty(product.NamePro))
                    {
                        basePrefix = product.NamePro.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                    }

                    // matched files list
                    List<string> matched = new List<string>();
                    if (!string.IsNullOrEmpty(basePrefix))
                    {
                        // regex for start like ^OwnTheRun_ or ^OwnTheRun-
                        var startPattern = "^" + Regex.Escape(basePrefix) + @"[_\-]";
                        var startRegex = new Regex(startPattern, RegexOptions.IgnoreCase);

                        matched = allFiles.Where(f =>
                            startRegex.IsMatch(f)                                  // OwnTheRun_Blue_01 or OwnTheRun-Blue-01
                            || f.IndexOf(basePrefix, StringComparison.OrdinalIgnoreCase) >= 0 // fallback contains
                            || f.StartsWith(basePrefix, StringComparison.OrdinalIgnoreCase)    // defensive
                        ).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                    }

                    // fallback: search by name tokens if still empty
                    if (!matched.Any() && !string.IsNullOrEmpty(product.NamePro))
                    {
                        var words = product.NamePro.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                   .Where(w => w.Length > 2)
                                                   .ToArray();
                        foreach (var w in words)
                        {
                            var list = allFiles.Where(f => f.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                            foreach (var lf in list)
                                if (!matched.Contains(lf, StringComparer.OrdinalIgnoreCase))
                                    matched.Add(lf);
                        }
                    }

                    // Debug: print matched files to VS Output window for troubleshooting
                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"Matched files for product {product.ProductID} (prefix='{basePrefix}'): "
                            + (matched.Any() ? string.Join(", ", matched) : "<none>"));
                    }
                    catch { /* ignore logging errors */ }

                    // Group matched files by color token (pattern: Prefix_Color_XX.ext)
                    foreach (var fname in matched.Distinct(StringComparer.OrdinalIgnoreCase))
                    {
                        var parts = fname.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                        string color = "Default";
                        if (parts.Length >= 2)
                        {
                            color = parts[1]; // e.g. "Blue" from OwnTheRun_Blue_01.jpg
                        }

                        var virt = Url.Content("~/Content/images/" + fname);
                        if (!imagesByColor.ContainsKey(color)) imagesByColor[color] = new List<string>();
                        imagesByColor[color].Add(virt);
                    }

                    // helper: extract numeric sort key from filename virtual path
                    int GetSortKey(string virtualPath)
                    {
                        try
                        {
                            // virtualPath example: "/Content/images/OwnTheRun_Blue_01.jpg"
                            var fileName = Path.GetFileName(virtualPath); // OwnTheRun_Blue_01.jpg
                            var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName); // OwnTheRun_Blue_01
                            var parts = nameWithoutExt.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

                            // try last token first
                            var last = parts.Length > 0 ? parts.Last() : nameWithoutExt;

                            if (int.TryParse(last, out var n)) return n;

                            // try extract digits from last token
                            var digits = new string(last.Where(char.IsDigit).ToArray());
                            if (!string.IsNullOrEmpty(digits) && int.TryParse(digits, out n)) return n;

                            // try find any numeric token from the end
                            for (int i = parts.Length - 1; i >= 0; i--)
                            {
                                var p = parts[i];
                                var d = new string(p.Where(char.IsDigit).ToArray());
                                if (!string.IsNullOrEmpty(d) && int.TryParse(d, out n)) return n;
                            }

                            // no number found -> put at end
                            return int.MaxValue;
                        }
                        catch
                        {
                            return int.MaxValue;
                        }
                    }

                    // sort images inside each color group using GetSortKey
                    foreach (var key in imagesByColor.Keys.ToList())
                    {
                        var sorted = imagesByColor[key]
                            .Select(v => new { src = v, sortKey = GetSortKey(v) })
                            .OrderBy(x => x.sortKey)
                            .Select(x => x.src)
                            .ToList();

                        imagesByColor[key] = sorted;
                    }
                }
            }
            catch
            {
                // ignore filesystem errors — fallback handled below
            }

            // fallback: if nothing found, use product.ImagePro or placeholder
            if (!imagesByColor.Any())
            {
                if (!string.IsNullOrEmpty(product.ImagePro))
                {
                    imagesByColor["Default"] = new List<string> { Url.Content("~/Content/images/" + product.ImagePro) };
                }
                else
                {
                    imagesByColor["Default"] = new List<string> { Url.Content("~/Content/images/no-image.png") };
                }
            }

            // choose default color (prefer "Default")
            string defaultColor = imagesByColor.ContainsKey("Default") ? "Default" : imagesByColor.Keys.FirstOrDefault();

            // determine primary image (first of default color or first available)
            string primaryImage = imagesByColor.ContainsKey(defaultColor) && imagesByColor[defaultColor].Any()
                ? imagesByColor[defaultColor].First()
                : imagesByColor.SelectMany(kv => kv.Value).FirstOrDefault();

            // pass to view
            ViewBag.ImagesByColor = imagesByColor;
            ViewBag.DefaultColor = defaultColor ?? "Default";
            ViewBag.PrimaryImage = primaryImage ?? Url.Content("~/Content/images/no-image.png");

            // build viewmodel
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

            return View(vm);
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
