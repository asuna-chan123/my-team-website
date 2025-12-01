using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using PagedList;
using WebApplication6.Models;
using WebApplication6.ViewModels;

namespace WebApplication6.Controllers
{
    public class HomeController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        public ActionResult Index(
            int? category,
            int? page,
            string searchString,
            double? min,
            double? max)
        {
            // ====== QUERY SẢN PHẨM ======
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

            // ====== CAROUSEL 1 & 2 ======
            var carousel1Vm = BuildCarousel("carousel-1");
            var carousel2Vm = BuildCarousel("carousel-2");

            // ====== BANNER TOP & MIDDLE ======
            var topBanner = db.Banners
                              .FirstOrDefault(b => b.Code == "home_top" && b.IsActive);

            var middleBanner = db.Banners
                                 .FirstOrDefault(b => b.Code == "home_middle" && b.IsActive);

            ViewBag.TopBanner = topBanner;
            ViewBag.MiddleBanner = middleBanner;

            // ====== VIEWMODEL ======
            var vm = new ProductListViewModel
            {
                Products = pagedProducts,
                Carousel1 = carousel1Vm,
                Carousel2 = carousel2Vm
            };

            // Dùng view ~/Views/Home/Index.cshtml
            return View(vm);
        }

        // Hàm dùng chung để build carousel
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
