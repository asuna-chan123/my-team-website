using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
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
        public ActionResult Index(string gender, string category, string priceSort, string keyword)
        {
            // 1) Query gốc
            var q = db.Products
                      .Include(p => p.Category)
                      .Where(p => !p.IsDeleted);

            // ====== LỌC THEO TỪ KHÓA (NAMEPRO) ======
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                // Tùy bạn, có thể thêm DescriptionPro nếu muốn
                q = q.Where(p => p.NamePro.Contains(keyword));
                // hoặc: q = q.Where(p => p.NamePro.Contains(keyword) || p.DecriptionPro.Contains(keyword));
            }

            System.Diagnostics.Debug.WriteLine($"param gender={gender}");
            System.Diagnostics.Debug.WriteLine("Distinct genders in DB: " +
                string.Join(", ", db.Products
                                    .Select(p => p.Gender)
                                    .Distinct()
                                    .Where(x => x != null)
                                    .ToList()));

            // 2) Map gender từ param (male/female/unisex -> Nam/Nữ/Unisex)
            string mappedGender = null;
            if (!string.IsNullOrEmpty(gender))
            {
                if (gender.Equals("male", StringComparison.OrdinalIgnoreCase)) mappedGender = "Nam";
                else if (gender.Equals("female", StringComparison.OrdinalIgnoreCase)) mappedGender = "Nữ";
                else if (gender.Equals("unisex", StringComparison.OrdinalIgnoreCase)) mappedGender = "Unisex";
                else mappedGender = gender;
            }

            System.Diagnostics.Debug.WriteLine($"gender param={gender} mapped={mappedGender}");

            // 3) Lọc theo giới tính (Nam => Nam + Unisex, Nữ => Nữ + Unisex)
            if (!string.IsNullOrEmpty(mappedGender))
            {
                if (mappedGender.Equals("Nam", StringComparison.OrdinalIgnoreCase))
                {
                    q = q.Where(p => p.Gender == "Nam" || p.Gender == "Unisex");
                }
                else if (mappedGender.Equals("Nữ", StringComparison.OrdinalIgnoreCase))
                {
                    q = q.Where(p => p.Gender == "Nữ" || p.Gender == "Unisex");
                }
                else if (mappedGender.Equals("Unisex", StringComparison.OrdinalIgnoreCase))
                {
                    q = q.Where(p => p.Gender == "Unisex");
                }
                else
                {
                    q = q.Where(p => p.Gender == mappedGender);
                }
            }

            // 4) Lọc theo category
            if (!string.IsNullOrEmpty(category))
            {
                int catId;
                if (int.TryParse(category, out catId))
                {
                    // category là IDCate -> lọc đúng CateID
                    q = q.Where(p => p.CateID == catId);
                }
                else
                {
                    // category là tên -> lọc theo NameCate
                    var cLower = category.ToLower();
                    q = q.Where(p => p.Category != null &&
                                     p.Category.NameCate != null &&
                                     p.Category.NameCate.ToLower() == cLower);
                }
            }

            // 5) Sắp xếp giá
            if (!string.IsNullOrEmpty(priceSort))
            {
                if (priceSort == "asc")
                    q = q.OrderBy(p => p.Price);
                else if (priceSort == "desc")
                    q = q.OrderByDescending(p => p.Price);
            }

            // 6) Cuối cùng mới ToList()
            var list = q.ToList();

            // 7) Tạo ViewModel
            var vm = new ProductListViewModel
            {
                Products = list,
                Gender = gender,          // giữ lại param gốc (male/female/unisex)
                Category = category,
                PriceSort = priceSort,
                // nếu bạn có field Keyword trong VM thì set luôn:
                Keyword = keyword,
                AllCategories = db.Categories.Where(c => !c.IsDeleted).ToList()
            };

            return View(vm);
        }

        // ================================
        // 📌 2. Trang chi tiết sản phẩm
        // ================================
        public ActionResult Details(int id)
        {
            var product = db.Products
                .Include(p => p.Category)
                .Include(p => p.ProductVariants.Select(v => v.Size))  // 👉 lấy kèm Size
                .FirstOrDefault(p => p.ProductID == id);

            if (product == null) return HttpNotFound();

            // ----- ẢNH THEO MÀU (giữ như bạn đang có) -----
            var images = db.ProductImages
                           .Where(i => i.ProductID == id)
                           .OrderBy(i => i.SortOrder)
                           .ToList();

            var imagesByColor = images
                .GroupBy(i => i.ColorID ?? 0)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ImageUrl).ToList()
                );

            ViewBag.ImagesByColor = imagesByColor;
            ViewBag.DefaultColor = imagesByColor.Keys.FirstOrDefault();

            var colorNames = db.Colors.ToDictionary(c => c.ColorID, c => c.ColorName);
            ViewBag.ColorNames = colorNames;

            // ----- STOCK THEO MÀU + SIZE -----
            var variantStocks = product.ProductVariants
                .Where(v => v.IsDeleted == false)
                .Select(v => new
                {
                    v.ColorID,
                    SizeName = v.Size.SizeName,
                    v.StockQty
                })
                .ToList();

            ViewBag.VariantStocks = variantStocks;

            return View(product);
        }

        // ================================
        // 📌 3. Phân trang + lọc sản phẩm
        // ================================
        public ActionResult ProductList(
        int? category,
        int? page,
        string searchString,
        double? min,
        double? max)
        {
            var query = db.Products
                          .Include(p => p.Category)
                          .AsQueryable();

            // Lọc theo danh mục
            if (category.HasValue)
            {
                query = query.Where(p => p.CateID == category.Value);
            }

            // Lọc theo tên
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var keyword = searchString.Trim();
                query = query.Where(p => p.NamePro.Contains(keyword));
            }

            // Lọc theo khoảng giá
            if (min.HasValue)
            {
                query = query.Where(p => p.Price >= (decimal)min.Value);
            }
            if (max.HasValue)
            {
                query = query.Where(p => p.Price <= (decimal)max.Value);
            }

            const int pageSize = 6;
            int pageNumber = page ?? 1;

            var pagedProducts = query
                .OrderBy(p => p.ProductID)
                .ToPagedList(pageNumber, pageSize);


            // Lấy 2 carousel dùng chung 1 hàm
            var carousel1Vm = BuildCarousel("carousel-1");
            var carousel2Vm = BuildCarousel("carousel-2");

            // ======= LẤY BANNER ĐẦU & GIỮA TRANG TỪ DB =======
            var topBanner = db.Banners
                              .FirstOrDefault(b => b.Code == "home_top" && b.IsActive);

            var middleBanner = db.Banners
                                 .FirstOrDefault(b => b.Code == "home_middle" && b.IsActive);

            ViewBag.TopBanner = topBanner;
            ViewBag.MiddleBanner = middleBanner;
            // ==================================================

            var vm = new ProductListViewModel
            {
                Products = pagedProducts,
                Carousel1 = carousel1Vm,
                Carousel2 = carousel2Vm
            };

            return View(vm);
        }

        private CarouselViewModel BuildCarousel(string code)
        {
            var carousel = db.Carousels
                             .Include(c => c.CarouselItems)
                             .FirstOrDefault(c => c.Code == code);

            if (carousel == null)
            {
                return new CarouselViewModel
                {
                    Id = code,
                    Title = string.Empty,
                    Items = new List<CarouselItemViewModel>()
                };
            }

            return new CarouselViewModel
            {
                Id = code,
                Title = carousel.Name,
                Items = carousel.CarouselItems
                                .OrderBy(i => i.Order)
                                .Select(i => new CarouselItemViewModel
                                {
                                    img = i.ImageUrl,
                                    price = i.Title,
                                    desc = i.Description,
                                    height = i.Height,
                                    link = i.Link
                                })
                                .ToList()
            };
        }



    }
}
