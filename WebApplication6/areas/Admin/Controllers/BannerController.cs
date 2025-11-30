using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplication6.Models;

namespace WebApplication6.Areas.Admin.Controllers
{
    public class BannerController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        // GET: Admin/Banner
        public ActionResult Index(string search)
        {
            var query = db.Banners.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(b =>
                    b.Title.Contains(search) ||
                    b.Subtitle.Contains(search) ||
                    b.Code.Contains(search));
            }

            var list = query
                .OrderBy(b => b.SortOrder)
                .ThenBy(b => b.Id)
                .ToList();

            ViewBag.Search = search;
            return View(list);
        }

        // GET: Admin/Banner/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var banner = db.Banners.Find(id);
            if (banner == null) return HttpNotFound();

            return View(banner);
        }

        // GET: Admin/Banner/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var banner = db.Banners.Find(id);
            if (banner == null) return HttpNotFound();

            return View(banner);
        }

        // POST: Admin/Banner/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Banner banner)
        {
            // chuẩn hóa
            banner.Code = (banner.Code ?? "").Trim();
            banner.Title = (banner.Title ?? "").Trim();

            // validate cơ bản
            if (string.IsNullOrWhiteSpace(banner.Code))
                ModelState.AddModelError("Code", "Vui lòng nhập mã code.");

            if (string.IsNullOrWhiteSpace(banner.Title))
                ModelState.AddModelError("Title", "Vui lòng nhập tiêu đề.");

            // không cho trùng Code với banner khác
            bool codeExists = db.Banners.Any(b =>
                b.Id != banner.Id && b.Code == banner.Code);

            if (codeExists)
            {
                ModelState.AddModelError("Code", "Mã code đã tồn tại, vui lòng chọn mã khác.");
            }

            if (!ModelState.IsValid)
                return View(banner);

            var dbBanner = db.Banners.Find(banner.Id);
            if (dbBanner == null) return HttpNotFound();

            dbBanner.Code = banner.Code;
            dbBanner.Title = banner.Title;
            dbBanner.Subtitle = banner.Subtitle;
            dbBanner.ImageUrl = banner.ImageUrl;
            dbBanner.VideoUrl = banner.VideoUrl;
            dbBanner.ButtonText = banner.ButtonText;
            dbBanner.ButtonLink = banner.ButtonLink;
            dbBanner.IsActive = banner.IsActive;
            dbBanner.SortOrder = banner.SortOrder;

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
